<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import { storeToRefs } from 'pinia'
import { toast } from 'vue-sonner'
import { useI18n } from 'vue-i18n'

import { orderService } from '@/app/services/orderService'
import { auctionService } from '@/app/services/auctionService'
import { itemService } from '@/app/services/lotService'
import { mediaService } from '@/app/services/mediaService'
import {
  getUserRoleInOrder,
  normalizePaymentStatus,
} from '@/app/helpers/orderHelpers'
import { buildMediaUrl } from '@/shared/mediaUrl'
import { useAuthStore } from '@/stores/authStore'
import { useLocalePath } from '@/composables/useLocalePath'
import type {
  OpenOrderDisputePayload,
  OrderDetails,
  OrderDisputeReason,
  OrderUserRole,
  ShippingAddressPayload,
} from '@/types/order'

type DataRecord = Record<string, unknown>
type ProgressState = 'complete' | 'current' | 'upcoming' | 'issue'
type PaymentState = 'paid' | 'pending' | 'expired' | 'failed' | 'cancelled' | 'unknown'
type ShippingState = 'shipped' | 'delivered' | 'issue' | 'unknown'

type ProgressStep = {
  label: string
  description: string
  date: string
  state: ProgressState
}

type ItemDetail = {
  labelKey: string
  value: string
}

type GalleryItem = {
  url: string
  alt: string
}

const route = useRoute()
const authStore = useAuthStore()
const { user, isAuthenticated } = storeToRefs(authStore)
const { t, te, locale } = useI18n()
const localePath = useLocalePath()

const isLoading = ref(true)
const errorMessage = ref('')
const order = ref<OrderDetails | null>(null)
const lotData = ref<DataRecord>({})
const auctionData = ref<DataRecord>({})
const activeMediaIndex = ref(0)
const hasImageFailed = ref(false)
const copiedReference = ref<'order' | 'auction' | ''>('')
const isCheckoutSubmitting = ref(false)
const isShipmentSubmitting = ref(false)
const isReceiptSubmitting = ref(false)
const isDisputeSubmitting = ref(false)
const checkoutError = ref('')
const shipmentError = ref('')
const disputeError = ref('')
const shippingAddressForm = ref<ShippingAddressPayload>({
  fullName: '',
  countryCode: '',
  country: '',
  city: '',
  postalCode: '',
  addressLine1: '',
  addressLine2: '',
  phone: '',
})
const shipmentForm = ref({
  carrierName: '',
  trackingNumber: '',
})
const disputeReasonOptions: Array<{ value: OrderDisputeReason; labelKey: string }> = [
  { value: 'ItemNotShipped', labelKey: 'orderDetails.disputeReasons.ItemNotShipped' },
  { value: 'ItemNotReceived', labelKey: 'orderDetails.disputeReasons.ItemNotReceived' },
  { value: 'WrongItem', labelKey: 'orderDetails.disputeReasons.WrongItem' },
  {
    value: 'SignificantlyNotAsDescribed',
    labelKey: 'orderDetails.disputeReasons.SignificantlyNotAsDescribed',
  },
  { value: 'Damaged', labelKey: 'orderDetails.disputeReasons.Damaged' },
  { value: 'SuspectedCounterfeit', labelKey: 'orderDetails.disputeReasons.SuspectedCounterfeit' },
]
const disputeForm = ref<{
  reason: OrderDisputeReason
  description: string
}>({
  reason: 'ItemNotReceived',
  description: '',
})
const disputeEvidenceFiles = ref<File[]>([])

const asRecord = (value: unknown): DataRecord => {
  if (!value || typeof value !== 'object' || Array.isArray(value)) return {}
  return value as DataRecord
}

const firstValue = (source: DataRecord, keys: string[]): unknown => {
  for (const key of keys) {
    const value = source[key]
    if (value !== null && value !== undefined && value !== '') return value
  }

  return null
}

const firstText = (source: DataRecord, keys: string[]): string => {
  const value = firstValue(source, keys)
  if (typeof value !== 'string' && typeof value !== 'number') return ''
  return String(value).trim()
}

const formatMoney = (
  value: unknown,
  currencyValue: unknown = 'EUR',
): string => {
  const amount = typeof value === 'number' ? value : Number(value)
  const rawCurrency = String(currencyValue || 'EUR').toUpperCase()
  const currency = /^[A-Z]{3}$/.test(rawCurrency) ? rawCurrency : 'EUR'

  if (!Number.isFinite(amount)) return '—'

  try {
    return new Intl.NumberFormat(locale.value, {
      style: 'currency',
      currency,
      currencyDisplay: 'narrowSymbol',
    }).format(amount)
  } catch {
    return `${amount.toLocaleString(locale.value)} ${currency}`
  }
}

const formatDateTime = (value: unknown): string => {
  if (!value) return ''

  const date = new Date(String(value))
  if (Number.isNaN(date.getTime())) return ''

  return new Intl.DateTimeFormat(locale.value, {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(date)
}

const orderRecord = computed(() => asRecord(order.value))
const lotRecord = computed(() => lotData.value)
const auctionRecord = computed(() => auctionData.value)

const currentUserId = computed(() => {
  const currentUser = asRecord(user.value)
  return firstText(currentUser, ['userId', 'id'])
})

const role = computed<OrderUserRole>(() => {
  if (!order.value) return 'unknown'
  return getUserRoleInOrder(order.value, currentUserId.value)
})

const pageTitle = computed(() => {
  if (role.value === 'buyer') return t('orderDetails.purchaseDetails')
  if (role.value === 'seller') return t('orderDetails.saleDetails')
  return t('orderDetails.orderDetails')
})

const roleLabel = computed(() => {
  if (role.value === 'buyer') return t('orderDetails.buyer')
  if (role.value === 'seller') return t('orderDetails.seller')
  return t('common.unavailable')
})

const orderReference = computed(() =>
  firstText(orderRecord.value, ['id', 'orderId']),
)

const auctionReference = computed(
  () =>
    firstText(orderRecord.value, ['auctionId']) ||
    firstText(auctionRecord.value, ['auctionId', 'id']),
)

const lotReference = computed(
  () =>
    firstText(auctionRecord.value, ['lotId']) ||
    firstText(orderRecord.value, ['lotId']),
)

const shortReference = (value: string) =>
  value ? value.slice(0, 8).toUpperCase() : t('common.unavailable')

const lotTitle = computed(
  () =>
    firstText(lotRecord.value, ['title']) ||
    firstText(orderRecord.value, ['title', 'lotTitle']) ||
    firstText(auctionRecord.value, ['lotTitle']) ||
    t('orderDetails.untitledPiece'),
)

const lotBrand = computed(() => firstText(lotRecord.value, ['brand']))
const lotDescription = computed(() =>
  firstText(lotRecord.value, ['description']),
)

const galleryItems = computed<GalleryItem[]>(() => {
  const media = lotRecord.value.media
  const urls: string[] = []

  if (Array.isArray(media)) {
    for (const item of media) {
      const url = firstText(asRecord(item), ['url'])
      if (url && !urls.includes(url)) urls.push(url)
    }
  }

  const orderImage = firstText(orderRecord.value, ['imageUrl'])
  if (orderImage && !urls.includes(orderImage)) urls.push(orderImage)

  return urls.map((url, index) => ({
    url,
    alt: t('orderDetails.imageAlt', { title: lotTitle.value, number: index + 1 }),
  }))
})

const activeImage = computed(
  () => galleryItems.value[activeMediaIndex.value] ?? galleryItems.value[0],
)

const selectMedia = (index: number) => {
  activeMediaIndex.value = index
  hasImageFailed.value = false
}

const itemTags = computed(() =>
  [
    firstText(lotRecord.value, ['genderName']),
    firstText(lotRecord.value, ['sizeName']),
    firstText(lotRecord.value, ['conditionName']),
  ].filter(Boolean),
)

const itemDetails = computed<ItemDetail[]>(() => {
  const city = firstText(lotRecord.value, ['city'])
  const country = firstText(lotRecord.value, ['country'])
  const location = [city, country].filter(Boolean).join(', ')

  return [
    { labelKey: 'catalog.brand', value: lotBrand.value },
    {
      labelKey: 'catalog.category',
      value: firstText(lotRecord.value, ['categoryName']),
    },
    { labelKey: 'catalog.gender', value: firstText(lotRecord.value, ['genderName']) },
    { labelKey: 'catalog.size', value: firstText(lotRecord.value, ['sizeName']) },
    {
      labelKey: 'catalog.condition',
      value: firstText(lotRecord.value, ['conditionName']),
    },
    { labelKey: 'lot.color', value: firstText(lotRecord.value, ['color']) },
    { labelKey: 'lot.year', value: firstText(lotRecord.value, ['age']) },
    { labelKey: 'lot.style', value: firstText(lotRecord.value, ['style']) },
    { labelKey: 'lot.location', value: location },
  ].filter((item) => Boolean(item.value))
})

const localizeCatalogValue = (value: string) => {
  const key = `catalog.options.${value}`

  return te(key) ? t(key) : value
}

const currency = computed(
  () =>
    firstText(orderRecord.value, ['currency']) ||
    firstText(auctionRecord.value, ['currency']) ||
    firstText(lotRecord.value, ['currency']) ||
    'EUR',
)

const winningAmount = computed(
  () =>
    firstValue(orderRecord.value, ['price', 'amount']) ??
    firstValue(auctionRecord.value, ['currentPrice']),
)

const shippingAmount = computed(() =>
  firstValue(orderRecord.value, ['shippingPrice']),
)

const totalAmount = computed(
  () =>
    firstValue(orderRecord.value, ['totalPrice']) ??
    Number(winningAmount.value ?? 0) + Number(shippingAmount.value ?? 0),
)

const openingAmount = computed(() => firstValue(lotRecord.value, ['price']))

const winningPriceLabel = computed(() =>
  formatMoney(winningAmount.value, currency.value),
)

const shippingPriceLabel = computed(() =>
  formatMoney(shippingAmount.value ?? 0, currency.value),
)

const totalPriceLabel = computed(() =>
  formatMoney(totalAmount.value, currency.value),
)

const openingPriceLabel = computed(() =>
  formatMoney(openingAmount.value, currency.value),
)

const auctionStartedAt = computed(() =>
  firstValue(auctionRecord.value, ['startsAt', 'startedAt']),
)

const auctionEndedAt = computed(() =>
  firstValue(auctionRecord.value, ['endsAt', 'endedAt']),
)

const orderCreatedAt = computed(
  () =>
    firstValue(orderRecord.value, ['createdAt', 'createdDate']) ??
    auctionEndedAt.value,
)

const paidAt = computed(() =>
  firstValue(orderRecord.value, [
    'paidAtUtc',
    'paidAt',
    'paymentCompletedAt',
    'paymentDate',
  ]),
)

const paymentDeadline = computed(() =>
  firstValue(orderRecord.value, ['paymentDeadlineUtc', 'paymentDeadline']),
)

const shippedAt = computed(() =>
  firstValue(orderRecord.value, [
    'shippedAtUtc',
    'shippedAt',
    'shipmentDate',
    'dispatchedAt',
  ]),
)

const deliveredAt = computed(() =>
  firstValue(orderRecord.value, [
    'deliveredAtUtc',
    'deliveredAt',
    'completedAt',
    'deliveryDate',
  ]),
)

const deliveryIssueReportedAt = computed(() =>
  firstValue(orderRecord.value, [
    'deliveryIssueReportedAtUtc',
    'deliveryIssueReportedAt',
  ]),
)

const deliveryIssueReasonText = computed(() =>
  firstText(orderRecord.value, ['deliveryIssueReason']),
)

const disputeRecord = computed(() =>
  asRecord(firstValue(orderRecord.value, ['dispute'])),
)

const disputeId = computed(() => firstText(disputeRecord.value, ['id', 'disputeId']))

const disputeStatusName = computed(() =>
  firstText(disputeRecord.value, ['statusName', 'status']),
)

const disputeOpenedAt = computed(() =>
  firstValue(disputeRecord.value, ['openedAtUtc', 'openedAt']),
)

const disputeResolvedAt = computed(() =>
  firstValue(disputeRecord.value, ['resolvedAtUtc', 'resolvedAt']),
)

const disputeDecisionName = computed(() =>
  firstText(disputeRecord.value, ['decisionName', 'decision']),
)

const disputeDecisionReason = computed(() =>
  firstText(disputeRecord.value, ['decisionReason']),
)

const disputeDescription = computed(() =>
  firstText(disputeRecord.value, ['description']) || deliveryIssueReasonText.value,
)

const disputeReasonName = computed(() => {
  const reason = firstText(disputeRecord.value, ['reasonName', 'reason'])
  if (!reason && deliveryIssueReportedAt.value) return 'ItemNotReceived'
  return reason
})

const disputeReasonLabel = computed(() => {
  const reason = disputeReasonName.value
  const option = disputeReasonOptions.find((item) => item.value === reason)
  return option?.labelKey ? t(option.labelKey) : reason.replace(/([a-z])([A-Z])/g, '$1 $2')
})

const disputeEvidenceItems = computed(() => {
  const evidence = disputeRecord.value.evidence
  if (!Array.isArray(evidence)) return []

  return evidence
    .map((item) => asRecord(item))
    .map((item) => ({
      id: firstText(item, ['id']),
      key: firstText(item, ['key']),
      createdAtUtc: firstValue(item, ['createdAtUtc', 'createdAt']),
    }))
    .filter((item) => item.key)
})

const trackingNumber = computed(() =>
  firstText(orderRecord.value, [
    'trackingNumber',
    'shipmentTrackingNumber',
    'trackingCode',
  ]),
)

const carrierName = computed(() =>
  firstText(orderRecord.value, ['carrierName', 'shippingCarrier', 'carrier']),
)

const shipBy = computed(() => firstValue(orderRecord.value, ['shipByUtc']))

const shippingAddressRecord = computed(() =>
  asRecord(orderRecord.value.shippingAddress),
)

const canViewShippingAddress = computed(() =>
  Boolean(firstValue(orderRecord.value, ['canViewShippingAddress'])),
)

const shippingAddressLines = computed(() =>
  [
    firstText(shippingAddressRecord.value, ['fullName']),
    firstText(shippingAddressRecord.value, ['addressLine1']),
    firstText(shippingAddressRecord.value, ['addressLine2']),
    [
      firstText(shippingAddressRecord.value, ['postalCode']),
      firstText(shippingAddressRecord.value, ['city']),
    ]
      .filter(Boolean)
      .join(' '),
    [
      firstText(shippingAddressRecord.value, ['country']),
      firstText(shippingAddressRecord.value, ['countryCode']),
    ]
      .filter(Boolean)
      .join(' '),
    firstText(shippingAddressRecord.value, ['phone']),
  ].filter(Boolean),
)

const paymentState = computed<PaymentState>(() => {
  const label = firstText(orderRecord.value, [
    'paymentStatusName',
    'orderStatusName',
    'statusName',
  ]).toLowerCase()

  if (/(paymentexpired|payment expired)/.test(label)) return 'expired'
  if (/(cancel|refund)/.test(label)) return 'cancelled'
  if (/(fail|declin|expired)/.test(label)) return 'failed'
  if (paidAt.value || /(^|\s)(paid|payment confirmed)(\s|$)/.test(label)) {
    return 'paid'
  }

  if (paymentDeadline.value) {
    const deadline = new Date(String(paymentDeadline.value)).getTime()
    if (!Number.isNaN(deadline) && Date.now() >= deadline) return 'expired'
  }

  if (/(pending|awaiting|created|unpaid)/.test(label)) return 'pending'

  const normalized = normalizePaymentStatus(
    firstValue(orderRecord.value, ['paymentStatus', 'orderStatus', 'status']) as
      | string
      | number
      | null
      | undefined,
  )

  if (normalized === 'paid') return 'paid'
  if (normalized === 'pending') return 'pending'
  if (normalized === 'failed') return 'failed'
  return 'unknown'
})

const shippingState = computed<ShippingState>(() => {
  const shippingLabel = firstText(orderRecord.value, [
    'shippingStatusName',
    'shippingStatus',
    'deliveryStatusName',
  ]).toLowerCase()
  const overallLabel = firstText(orderRecord.value, [
    'orderStatusName',
    'statusName',
  ]).toLowerCase()
  const combinedLabel = `${shippingLabel} ${overallLabel}`

  if (
    deliveryIssueReportedAt.value ||
    /(buyerprotection|buyer protection|disputed|not delivered)/.test(combinedLabel)
  ) {
    return 'issue'
  }

  if (deliveredAt.value || /(delivered|completed)/.test(combinedLabel)) {
    return 'delivered'
  }

  if (
    shippedAt.value ||
    trackingNumber.value ||
    /(shipped|dispatched|in transit)/.test(combinedLabel)
  ) {
    return 'shipped'
  }

  return 'unknown'
})

const isPaid = computed(
  () =>
    paymentState.value === 'paid' ||
    shippingState.value === 'shipped' ||
    shippingState.value === 'issue' ||
    shippingState.value === 'delivered',
)

const statusLabel = computed(() => {
  if (shippingState.value === 'delivered') return t('orderDetails.status.completed')
  if (shippingState.value === 'issue') return t('orderDetails.status.disputed')
  if (shippingState.value === 'shipped') return t('orderDetails.status.inTransit')
  if (paymentState.value === 'cancelled') return t('orderDetails.status.cancelled')
  if (paymentState.value === 'expired') return t('orderDetails.status.paymentExpired')
  if (paymentState.value === 'failed') return t('orderDetails.status.paymentFailed')
  if (paymentState.value === 'paid') return t('orderDetails.status.awaitingShipment')
  if (paymentState.value === 'pending') return t('orderDetails.status.awaitingPayment')

  return (
    firstText(orderRecord.value, ['orderStatusName', 'statusName']) ||
    t('orders.status.unknown')
  )
})

const statusBadgeClass = computed(() => {
  if (shippingState.value === 'delivered') {
    return 'border-emerald-500/30 bg-emerald-500/10 text-emerald-700 dark:text-emerald-300'
  }
  if (shippingState.value === 'issue') {
    return 'border-orange-500/30 bg-orange-500/10 text-orange-700 dark:text-orange-300'
  }
  if (shippingState.value === 'shipped') {
    return 'border-blue-500/30 bg-blue-500/10 text-blue-700 dark:text-blue-300'
  }
  if (paymentState.value === 'paid') {
    return 'border-emerald-500/30 bg-emerald-500/10 text-emerald-700 dark:text-emerald-300'
  }
  if (paymentState.value === 'pending') {
    return 'border-amber-500/30 bg-amber-500/10 text-amber-700 dark:text-amber-300'
  }
  if (paymentState.value === 'expired' || paymentState.value === 'failed' || paymentState.value === 'cancelled') {
    return 'border-red-500/30 bg-red-500/10 text-red-700 dark:text-red-300'
  }
  return 'border-foreground/15 bg-foreground/[0.04] text-foreground/60'
})

const statusTitle = computed(() => {
  if (shippingState.value === 'delivered') return t('orderDetails.statusTitle.orderCompleted')
  if (shippingState.value === 'issue') {
    return role.value === 'buyer'
      ? t('orderDetails.statusTitle.problemReported')
      : t('orderDetails.statusTitle.disputeOpened')
  }
  if (shippingState.value === 'shipped') {
    return role.value === 'buyer'
      ? t('orderDetails.statusTitle.buyerShipped')
      : t('orderDetails.statusTitle.sellerShipped')
  }
  if (paymentState.value === 'cancelled') return t('orderDetails.statusTitle.orderCancelled')
  if (paymentState.value === 'expired') return t('orderDetails.statusTitle.paymentExpired')
  if (paymentState.value === 'failed') return t('orderDetails.statusTitle.paymentAttention')
  if (paymentState.value === 'pending') {
    return role.value === 'buyer'
      ? t('orderDetails.statusTitle.completePayment')
      : t('orderDetails.statusTitle.waitingBuyer')
  }
  if (paymentState.value === 'paid') {
    return role.value === 'seller'
      ? t('orderDetails.statusTitle.paymentReceived')
      : t('orderDetails.statusTitle.paymentConfirmed')
  }
  return t('orderDetails.statusTitle.orderReceived')
})

const statusDescription = computed(() => {
  if (shippingState.value === 'delivered') {
    return t('orderDetails.statusDescription.complete')
  }
  if (shippingState.value === 'issue') {
    return role.value === 'buyer'
      ? t('orderDetails.statusDescription.issueBuyer')
      : t('orderDetails.statusDescription.issueSeller')
  }
  if (shippingState.value === 'shipped') {
    return trackingNumber.value
      ? t('orderDetails.statusDescription.tracking', { tracking: trackingNumber.value })
      : t('orderDetails.statusDescription.handedOver')
  }
  if (paymentState.value === 'cancelled') {
    return t('orderDetails.statusDescription.cancelled')
  }
  if (paymentState.value === 'expired') {
    return role.value === 'buyer'
      ? t('orderDetails.statusDescription.expiredBuyer')
      : t('orderDetails.statusDescription.expiredSeller')
  }
  if (paymentState.value === 'failed') {
    return role.value === 'buyer'
      ? t('orderDetails.statusDescription.failedBuyer')
      : t('orderDetails.statusDescription.failedSeller')
  }
  if (paymentState.value === 'pending') {
    if (role.value === 'buyer') {
      const deadline = formatDateTime(paymentDeadline.value)
      return deadline
        ? t('orderDetails.statusDescription.pendingBuyerDeadline', { date: deadline })
        : t('orderDetails.statusDescription.pendingBuyer')
    }
    return t('orderDetails.statusDescription.pendingSeller')
  }
  if (paymentState.value === 'paid') {
    return role.value === 'seller'
      ? t('orderDetails.statusDescription.paidSeller')
      : t('orderDetails.statusDescription.paidBuyer')
  }
  return t('orderDetails.statusDescription.unknown')
})

const paymentLabel = computed(() => {
  if (paymentState.value === 'paid') return t('orders.status.paid')
  if (paymentState.value === 'pending') return t('orderDetails.status.awaitingPayment')
  if (paymentState.value === 'expired') return t('orderDetails.status.paymentExpired')
  if (paymentState.value === 'failed') return t('orderDetails.status.failed')
  if (paymentState.value === 'cancelled') return t('orderDetails.status.cancelled')
  return t('common.unavailable')
})

const deliveryLabel = computed(() => {
  if (shippingState.value === 'delivered') return t('orderDetails.status.delivered')
  if (shippingState.value === 'issue') return t('orderDetails.status.disputed')
  if (shippingState.value === 'shipped') return t('orderDetails.status.inTransit')
  if (isPaid.value) return t('orderDetails.status.awaitingShipment')
  if (paymentState.value === 'expired' || paymentState.value === 'failed' || paymentState.value === 'cancelled') {
    return t('orderDetails.status.notApplicable')
  }
  return t('orderDetails.status.notStarted')
})

const canPay = computed(
  () => role.value === 'buyer' && paymentState.value === 'pending',
)

const canMarkShipped = computed(
  () =>
    role.value === 'seller' &&
    paymentState.value === 'paid' &&
    shippingState.value === 'unknown',
)

const canConfirmReceived = computed(
  () => role.value === 'buyer' && shippingState.value === 'shipped',
)

const hasDisputeDetails = computed(() =>
  Boolean(disputeId.value || shippingState.value === 'issue'),
)

const isDisputeOpen = computed(() => {
  if (!hasDisputeDetails.value) return false

  const status = disputeStatusName.value.toLowerCase()
  if (!status) return shippingState.value === 'issue'

  return !/(resolved|closed)/.test(status)
})

const canOpenDispute = computed(
  () =>
    role.value === 'buyer' &&
    isPaid.value &&
    !hasDisputeDetails.value &&
    paymentState.value !== 'cancelled' &&
    paymentState.value !== 'expired' &&
    paymentState.value !== 'failed',
)

const auctionStatus = computed(
  () =>
    firstText(auctionRecord.value, ['status', 'statusName']) || t('common.unavailable'),
)

const sellerRecord = computed(() => asRecord(lotRecord.value.seller))
const buyerRecord = computed(() => asRecord(orderRecord.value.buyer))

const counterpartyLabel = computed(() => {
  if (role.value === 'buyer') return t('orderDetails.seller')
  if (role.value === 'seller') return t('orderDetails.buyer')
  return t('orderDetails.counterparty')
})

const counterpartyName = computed(() => {
  if (role.value === 'buyer') {
    return (
      firstText(sellerRecord.value, [
        'name',
        'displayName',
        'username',
        'userName',
      ]) || t('orderDetails.seller')
    )
  }

  if (role.value === 'seller') {
    const buyerName = firstText(buyerRecord.value, [
      'name',
      'displayName',
      'username',
      'userName',
    ])
    if (buyerName) return buyerName

    const buyerId = firstText(orderRecord.value, ['buyerId'])
    return buyerId ? `${t('orderDetails.buyer')} #${shortReference(buyerId)}` : t('orderDetails.buyer')
  }

  return t('common.unavailable')
})

const progressSteps = computed<ProgressStep[]>(() => {
  const paymentIssue =
    paymentState.value === 'expired' ||
    paymentState.value === 'failed' ||
    paymentState.value === 'cancelled'
  const deliveryIssue = shippingState.value === 'issue'

  return [
    {
      label: role.value === 'seller'
        ? t('orderDetails.progress.auctionSold')
        : t('orderDetails.progress.auctionWon'),
      description:
        role.value === 'seller'
          ? t('orderDetails.progress.sellerWinningBid', { amount: winningPriceLabel.value })
          : t('orderDetails.progress.buyerWinningBid', { amount: winningPriceLabel.value }),
      date:
        formatDateTime(auctionEndedAt.value || orderCreatedAt.value) ||
        t('orderDetails.progress.confirmed'),
      state: 'complete',
    },
    {
      label: t('orderDetails.progress.payment'),
      description: isPaid.value
        ? t('orderDetails.progress.paymentConfirmed')
        : paymentIssue
          ? paymentState.value === 'expired'
            ? t('orderDetails.progress.paymentExpired')
            : t('orderDetails.progress.paymentNotCompleted')
          : t('orderDetails.progress.waitingPayment'),
      date: isPaid.value
        ? formatDateTime(paidAt.value) || t('orderDetails.progress.confirmed')
        : paymentState.value === 'pending' && paymentDeadline.value
          ? t('orderDetails.progress.due', { date: formatDateTime(paymentDeadline.value) })
          : paymentIssue
            ? t('orderDetails.progress.actionRequired')
            : '',
      state: isPaid.value ? 'complete' : paymentIssue ? 'issue' : 'current',
    },
    {
      label: t('orderDetails.progress.shipment'),
      description:
        shippingState.value === 'shipped' || shippingState.value === 'delivered'
          ? t('orderDetails.progress.dispatched')
          : deliveryIssue
            ? t('orderDetails.progress.disputeReview')
          : isPaid.value
            ? role.value === 'seller'
              ? t('orderDetails.progress.sellerPrepare')
              : t('orderDetails.progress.buyerWaitingDispatch')
            : t('orderDetails.progress.afterPayment'),
      date:
        shippingState.value === 'shipped' || shippingState.value === 'delivered' || deliveryIssue
          ? formatDateTime(shippedAt.value) || t('orderDetails.progress.dispatchedDate')
          : isPaid.value
            ? t('orderDetails.progress.currentStep')
            : '',
      state:
        deliveryIssue
          ? 'issue'
          : shippingState.value === 'shipped' || shippingState.value === 'delivered'
          ? 'complete'
          : isPaid.value
            ? 'current'
            : 'upcoming',
    },
    {
      label: t('orderDetails.progress.delivery'),
      description:
        shippingState.value === 'delivered'
          ? t('orderDetails.progress.orderCompleted')
          : deliveryIssue
            ? t('orderDetails.progress.reviewOpen')
          : shippingState.value === 'shipped'
            ? t('orderDetails.progress.parcelOnWay')
            : t('orderDetails.progress.updatesAfterShipping'),
      date:
        shippingState.value === 'delivered'
          ? formatDateTime(deliveredAt.value) || t('orderDetails.progress.completed')
          : deliveryIssue
            ? formatDateTime(deliveryIssueReportedAt.value) || t('orderDetails.progress.reported')
          : shippingState.value === 'shipped'
            ? t('orderDetails.progress.inProgress')
            : '',
      state:
        deliveryIssue
          ? 'issue'
          : shippingState.value === 'delivered'
          ? 'complete'
          : shippingState.value === 'shipped'
            ? 'current'
            : 'upcoming',
    },
  ]
})

const stepCircleClass = (state: ProgressState) => {
  if (state === 'complete') {
    return 'border-foreground bg-foreground text-background'
  }
  if (state === 'current') {
    return 'border-foreground bg-background text-foreground'
  }
  if (state === 'issue') {
    return 'border-red-500 bg-red-500 text-white'
  }
  return 'border-foreground/20 bg-background text-foreground/35'
}

const connectorClass = (index: number) => {
  const current = progressSteps.value[index]
  const next = progressSteps.value[index + 1]

  if (
    current?.state === 'complete' &&
    (next?.state === 'complete' || next?.state === 'current')
  ) {
    return 'bg-foreground'
  }
  if (current?.state === 'issue') return 'bg-red-500/40'
  return 'bg-foreground/10'
}

const copyReference = async (type: 'order' | 'auction', value: string) => {
  if (!value || !navigator.clipboard) return

  try {
    await navigator.clipboard.writeText(value)
    copiedReference.value = type
  } catch (error) {
    console.error('Failed to copy reference', error)
  }
}

const submitCheckout = async () => {
  checkoutError.value = ''

  if (!orderReference.value) return

  const payload = {
    ...shippingAddressForm.value,
    fullName: shippingAddressForm.value.fullName.trim(),
    countryCode: shippingAddressForm.value.countryCode.trim().toUpperCase(),
    country: shippingAddressForm.value.country.trim(),
    city: shippingAddressForm.value.city.trim(),
    postalCode: shippingAddressForm.value.postalCode.trim(),
    addressLine1: shippingAddressForm.value.addressLine1.trim(),
    addressLine2: shippingAddressForm.value.addressLine2?.trim() || null,
    phone: shippingAddressForm.value.phone?.trim() || null,
  }

  if (
    !payload.fullName ||
    !payload.countryCode ||
    !payload.country ||
    !payload.city ||
    !payload.postalCode ||
    !payload.addressLine1
  ) {
    checkoutError.value = t('orderDetails.checkoutAddressRequired')
    return
  }

  try {
    isCheckoutSubmitting.value = true
    const checkoutUrl = await orderService.createCheckout(orderReference.value, payload)
    window.location.href = checkoutUrl
  } catch (error: any) {
    checkoutError.value =
      error?.response?.data?.message ??
      error?.response?.data ??
      t('orderDetails.checkoutFailed')
  } finally {
    isCheckoutSubmitting.value = false
  }
}

const submitShipment = async () => {
  shipmentError.value = ''

  if (!orderReference.value) return

  const carrierNameValue = shipmentForm.value.carrierName.trim()
  const trackingNumberValue = shipmentForm.value.trackingNumber.trim()

  if (!carrierNameValue || !trackingNumberValue) {
    shipmentError.value = t('orderDetails.shipmentRequired')
    return
  }

  try {
    isShipmentSubmitting.value = true
    await orderService.markOrderShipped(orderReference.value, {
      carrierName: carrierNameValue,
      trackingNumber: trackingNumberValue,
    })
    toast.success(t('orderDetails.shipmentSaved'), { position: 'bottom-right' })
    await loadOrder()
  } catch (error: any) {
    shipmentError.value =
      error?.response?.data?.message ??
      error?.response?.data ??
      t('orderDetails.shipmentFailed')
  } finally {
    isShipmentSubmitting.value = false
  }
}

const confirmReceived = async () => {
  if (!orderReference.value) return

  try {
    isReceiptSubmitting.value = true
    await orderService.confirmOrderReceived(orderReference.value)
    toast.success(t('orderDetails.orderCompletedToast'), { position: 'bottom-right' })
    await loadOrder()
  } catch (error: any) {
    toast.error(
        error?.response?.data?.message ??
        error?.response?.data ??
        t('orderDetails.receiptFailed'),
      { position: 'bottom-right' },
    )
  } finally {
    isReceiptSubmitting.value = false
  }
}

const onDisputeEvidenceSelected = (event: Event) => {
  const input = event.target as HTMLInputElement
  const files = Array.from(input.files ?? [])
  disputeEvidenceFiles.value = files.slice(0, 6)
}

const removeDisputeEvidenceFile = (index: number) => {
  disputeEvidenceFiles.value = disputeEvidenceFiles.value.filter(
    (_, fileIndex) => fileIndex !== index,
  )
}

const submitDispute = async () => {
  disputeError.value = ''

  if (!orderReference.value) return

  const description = disputeForm.value.description.trim()
  if (!description) {
    disputeError.value = t('orderDetails.problemRequired')
    return
  }

  try {
    isDisputeSubmitting.value = true
    const evidenceKeys = await Promise.all(
      disputeEvidenceFiles.value.map(async (file) => {
        const result = await mediaService.uploadPhoto(file)
        return result.key
      }),
    )
    const payload: OpenOrderDisputePayload = {
      reason: disputeForm.value.reason,
      description,
      evidenceKeys,
    }

    await orderService.openDispute(orderReference.value, payload)
    toast.success(t('orderDetails.problemReported'), { position: 'bottom-right' })
    disputeForm.value = {
      reason: 'ItemNotReceived',
      description: '',
    }
    disputeEvidenceFiles.value = []
    await loadOrder()
  } catch (error: any) {
    disputeError.value =
      error?.response?.data?.message ??
      error?.response?.data ??
      t('orderDetails.problemFailed')
  } finally {
    isDisputeSubmitting.value = false
  }
}

const loadOrder = async () => {
  try {
    isLoading.value = true
    errorMessage.value = ''
    order.value = null
    lotData.value = {}
    auctionData.value = {}
    activeMediaIndex.value = 0
    hasImageFailed.value = false
    copiedReference.value = ''

    const routeId = Array.isArray(route.params.id)
      ? route.params.id[0]
      : route.params.id
    const id = String(routeId ?? '').trim()

    if (!id) throw new Error('Order id is missing.')

    const details = await orderService.getOrderDetails(id)
    order.value = details

    const detailsRecord = asRecord(details)
    const relatedAuctionId = firstText(detailsRecord, ['auctionId'])

    if (relatedAuctionId) {
      const auction = await auctionService
        .getAuctionDetails(relatedAuctionId)
        .catch((error) => {
          console.error('Failed to load related auction', error)
          return null
        })

      auctionData.value = asRecord(auction)
    }

    const relatedLotId =
      firstText(auctionData.value, ['lotId']) ||
      firstText(detailsRecord, ['lotId'])

    if (relatedLotId) {
      const lot = await itemService.getLot(relatedLotId).catch((error) => {
        console.error('Failed to load related lot', error)
        return null
      })

      lotData.value = asRecord(lot)
    }
  } catch (error) {
    console.error('Failed to load order details', error)
    errorMessage.value = t('orderDetails.loadFailed')
  } finally {
    isLoading.value = false
  }
}

onMounted(async () => {
  try {
    if (!isAuthenticated.value) await authStore.checkAuth()
  } catch (error) {
    console.error('Failed to check authentication', error)
  }

  if (!isAuthenticated.value) {
    errorMessage.value = t('orderDetails.signInRequired')
    isLoading.value = false
    return
  }

  await loadOrder()
})
</script>

<template>
  <main class="min-h-[70vh] bg-background text-foreground">
    <div class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <RouterLink
        :to="localePath('/orders')"
        class="mb-7 inline-flex items-center gap-2 rounded-lg border px-3 py-2 text-sm transition hover:bg-foreground hover:text-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground focus-visible:ring-offset-2 focus-visible:ring-offset-background"
      >
        <span aria-hidden="true">←</span>
        {{ $t('orderDetails.backToOrders') }}
      </RouterLink>

      <div v-if="isLoading" class="space-y-5" :aria-label="$t('orderDetails.loadingOrder')">
        <div class="flex items-end justify-between gap-4">
          <div class="space-y-3">
            <div class="h-4 w-28 animate-pulse rounded bg-foreground/10" />
            <div class="h-10 w-64 animate-pulse rounded bg-foreground/10" />
            <div class="h-4 w-52 animate-pulse rounded bg-foreground/10" />
          </div>
          <div class="h-8 w-24 animate-pulse rounded-full bg-foreground/10" />
        </div>
        <div
          class="h-72 animate-pulse rounded-2xl border bg-foreground/[0.03]"
        />
        <div class="grid gap-5 lg:grid-cols-[minmax(0,1fr)_22rem]">
          <div
            class="h-[36rem] animate-pulse rounded-2xl border bg-foreground/[0.03]"
          />
          <div
            class="h-[30rem] animate-pulse rounded-2xl border bg-foreground/[0.03]"
          />
        </div>
      </div>

      <div
        v-else-if="errorMessage"
        class="flex flex-col items-start justify-between gap-5 rounded-2xl border border-red-500/30 bg-red-500/10 p-6 sm:flex-row sm:items-center"
        role="alert"
      >
        <div>
          <p class="text-sm font-medium text-red-700 dark:text-red-300">
            {{ $t('orderDetails.unableOpen') }}
          </p>
          <p class="mt-1 text-sm text-foreground/70">{{ errorMessage }}</p>
        </div>
        <button
          v-if="isAuthenticated"
          type="button"
          class="rounded-lg border px-4 py-2 text-sm transition hover:bg-foreground hover:text-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground"
          @click="loadOrder"
        >
          {{ $t('common.tryAgain') }}
        </button>
        <RouterLink
          v-else
          :to="localePath('/login')"
          class="rounded-lg border px-4 py-2 text-sm transition hover:bg-foreground hover:text-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground"
        >
          {{ $t('navigation.signIn') }}
        </RouterLink>
      </div>

      <template v-else-if="order">
        <header
          class="mb-6 flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between"
        >
          <div>
            <p
              class="text-xs font-medium uppercase tracking-[0.18em] text-foreground/45"
            >
              {{ role === 'seller' ? $t('orderDetails.yourSale') : $t('orderDetails.yourPurchase') }}
            </p>
            <h1 class="mt-2 text-3xl font-semibold sm:text-4xl">
              {{ pageTitle }}
            </h1>
            <p class="mt-2 text-sm text-foreground/60">
              {{ $t('orderDetails.orderLine', { reference: shortReference(orderReference), title: lotTitle }) }}
            </p>
          </div>

          <span
            :class="[
              'self-start rounded-full border px-3 py-1.5 text-sm font-medium sm:self-auto',
              statusBadgeClass,
            ]"
          >
            {{ statusLabel }}
          </span>
        </header>

        <section class="rounded-2xl border bg-background p-5 sm:p-7">
          <div
            class="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between"
          >
            <div class="max-w-2xl">
              <p class="text-xs font-medium text-foreground/45">
                {{ $t('orderDetails.currentStatus') }}
              </p>
              <h2 class="mt-1 text-xl font-semibold sm:text-2xl">
                {{ statusTitle }}
              </h2>
              <p class="mt-2 text-sm leading-6 text-foreground/65">
                {{ statusDescription }}
              </p>
            </div>
            <div
              class="rounded-full border bg-foreground/[0.03] px-3 py-1.5 text-xs text-foreground/60"
            >
              {{ $t('orderDetails.viewingAs', { role: roleLabel }) }}
            </div>
          </div>

          <ol class="mt-8 grid gap-0 sm:grid-cols-4">
            <li
              v-for="(step, index) in progressSteps"
              :key="step.label"
              :aria-current="step.state === 'current' ? 'step' : undefined"
              class="relative grid grid-cols-[2rem_minmax(0,1fr)] gap-3 pb-6 last:pb-0 sm:block sm:pb-0"
            >
              <div class="relative flex justify-center sm:block">
                <span
                  :class="[
                    'relative z-10 flex h-7 w-7 items-center justify-center rounded-full border text-xs font-medium',
                    stepCircleClass(step.state),
                  ]"
                >
                  <span v-if="step.state === 'complete'" aria-hidden="true">
                    ✓
                  </span>
                  <span v-else-if="step.state === 'issue'" aria-hidden="true">
                    !
                  </span>
                  <span v-else>{{ index + 1 }}</span>
                </span>
                <span
                  v-if="index < progressSteps.length - 1"
                  :class="[
                    'absolute bottom-[-1.5rem] top-7 w-px sm:bottom-auto sm:left-7 sm:right-0 sm:top-3 sm:h-px sm:w-auto',
                    connectorClass(index),
                  ]"
                  aria-hidden="true"
                />
              </div>

              <div class="sm:mt-4 sm:pr-6">
                <p
                  :class="[
                    'text-sm font-medium',
                    step.state === 'upcoming'
                      ? 'text-foreground/40'
                      : 'text-foreground',
                  ]"
                >
                  {{ step.label }}
                </p>
                <p
                  :class="[
                    'mt-1 text-xs leading-5',
                    step.state === 'upcoming'
                      ? 'text-foreground/35'
                      : 'text-foreground/60',
                  ]"
                >
                  {{ step.description }}
                </p>
                <p
                  v-if="step.date"
                  class="mt-1.5 text-[11px] text-foreground/45"
                >
                  {{ step.date }}
                </p>
              </div>
            </li>
          </ol>
        </section>

        <div class="mt-6 grid gap-5 lg:grid-cols-[minmax(0,1fr)_22rem]">
          <section class="overflow-hidden rounded-2xl border bg-background">
            <div class="grid md:grid-cols-[minmax(0,0.9fr)_minmax(0,1.1fr)]">
              <div
                class="border-b bg-foreground/[0.025] p-4 md:border-b-0 md:border-r"
              >
                <div
                  class="relative aspect-[4/5] overflow-hidden rounded-xl border bg-foreground/[0.04]"
                >
                  <img
                    v-if="activeImage && !hasImageFailed"
                    :src="activeImage.url"
                    :alt="activeImage.alt"
                    class="h-full w-full object-cover"
                    @error="hasImageFailed = true"
                  />
                  <div
                    v-else
                    class="flex h-full items-center justify-center p-6 text-center text-sm text-foreground/40"
                  >
                    {{ $t('orderDetails.imageUnavailable') }}
                  </div>
                </div>

                <div
                  v-if="galleryItems.length > 1"
                  class="mt-3 flex gap-2 overflow-x-auto pb-1"
                  :aria-label="$t('orderDetails.itemImages')"
                >
                  <button
                    v-for="(media, index) in galleryItems"
                    :key="media.url"
                    type="button"
                    :aria-label="$t('orderDetails.showImage', { number: index + 1 })"
                    :aria-pressed="activeMediaIndex === index"
                    :class="[
                      'h-16 w-14 shrink-0 overflow-hidden rounded-lg border transition',
                      activeMediaIndex === index
                        ? 'border-foreground'
                        : 'border-foreground/10 opacity-60 hover:opacity-100',
                    ]"
                    @click="selectMedia(index)"
                  >
                    <img
                      :src="media.url"
                      alt=""
                      class="h-full w-full object-cover"
                    />
                  </button>
                </div>
              </div>

              <div class="p-5 sm:p-7">
                <p
                  v-if="lotBrand"
                  class="text-xs font-medium uppercase tracking-[0.16em] text-foreground/45"
                >
                  {{ lotBrand }}
                </p>
                <h2 class="mt-2 text-2xl font-semibold sm:text-3xl">
                  {{ lotTitle }}
                </h2>

                <div v-if="itemTags.length" class="mt-4 flex flex-wrap gap-2">
                  <span
                    v-for="tag in itemTags"
                    :key="tag"
                    class="rounded-full border px-2.5 py-1 text-xs text-foreground/60"
                  >
                    {{ localizeCatalogValue(tag) }}
                  </span>
                </div>

                <div class="mt-7 border-t pt-6">
                  <h3 class="text-sm font-semibold">{{ $t('orderDetails.itemDetails') }}</h3>
                  <dl
                    v-if="itemDetails.length"
                    class="mt-4 grid grid-cols-2 gap-x-6 gap-y-5 text-sm"
                  >
                    <div v-for="detail in itemDetails" :key="detail.labelKey">
                      <dt class="text-xs text-foreground/45">
                        {{ $t(detail.labelKey) }}
                      </dt>
                      <dd class="mt-1 font-medium">{{ localizeCatalogValue(detail.value) }}</dd>
                    </div>
                  </dl>
                  <p v-else class="mt-3 text-sm text-foreground/50">
                    {{ $t('orderDetails.itemUnavailable') }}
                  </p>
                </div>

                <details v-if="lotDescription" class="group mt-7 border-t pt-5">
                  <summary
                    class="flex cursor-pointer list-none items-center justify-between gap-4 text-sm font-medium"
                  >
                    {{ $t('orderDetails.aboutPiece') }}
                    <span
                      class="text-lg font-normal text-foreground/45 transition group-open:rotate-45"
                      aria-hidden="true"
                    >
                      +
                    </span>
                  </summary>
                  <p
                    class="mt-4 max-h-48 overflow-y-auto whitespace-pre-line pr-2 text-sm leading-6 text-foreground/65"
                  >
                    {{ lotDescription }}
                  </p>
                </details>

                <RouterLink
                  v-if="lotReference"
                  :to="localePath(`/lot/${lotReference}`)"
                  class="mt-7 inline-flex rounded-lg border px-4 py-2 text-sm transition hover:bg-foreground hover:text-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground"
                >
                  {{ $t('orderDetails.viewOriginalListing') }}
                </RouterLink>
              </div>
            </div>
          </section>

          <aside class="space-y-5">
            <section class="rounded-2xl border bg-background p-5">
              <p class="text-xs text-foreground/45">{{ $t('orderDetails.orderTotal') }}</p>
              <p class="mt-1 text-3xl font-semibold tabular-nums">
                {{ totalPriceLabel }}
              </p>

              <dl class="mt-6 divide-y text-sm">
                <div class="flex items-start justify-between gap-4 py-3 first:pt-0">
                  <dt class="text-foreground/50">{{ $t('orderDetails.winningBid') }}</dt>
                  <dd class="font-medium tabular-nums">
                    {{ winningPriceLabel }}
                  </dd>
                </div>
                <div class="flex items-start justify-between gap-4 py-3">
                  <dt class="text-foreground/50">{{ $t('common.shipping') }}</dt>
                  <dd class="font-medium tabular-nums">
                    {{ shippingPriceLabel }}
                  </dd>
                </div>
                <div
                  v-if="openingAmount !== null && openingAmount !== undefined"
                  class="flex items-start justify-between gap-4 py-3"
                >
                  <dt class="text-foreground/50">{{ $t('orderDetails.openingPrice') }}</dt>
                  <dd class="font-medium tabular-nums">
                    {{ openingPriceLabel }}
                  </dd>
                </div>
                <div
                  class="flex items-start justify-between gap-4 py-3 first:pt-0"
                >
                  <dt class="text-foreground/50">{{ $t('orderDetails.auction') }}</dt>
                  <dd class="text-right font-medium">{{ auctionStatus }}</dd>
                </div>
                <div
                  v-if="auctionStartedAt"
                  class="flex items-start justify-between gap-4 py-3"
                >
                  <dt class="text-foreground/50">{{ $t('orderDetails.started') }}</dt>
                  <dd class="max-w-[11rem] text-right font-medium">
                    {{ formatDateTime(auctionStartedAt) }}
                  </dd>
                </div>
                <div
                  v-if="auctionEndedAt"
                  class="flex items-start justify-between gap-4 py-3"
                >
                  <dt class="text-foreground/50">{{ $t('orderDetails.ended') }}</dt>
                  <dd class="max-w-[11rem] text-right font-medium">
                    {{ formatDateTime(auctionEndedAt) }}
                  </dd>
                </div>
                <div class="flex items-start justify-between gap-4 py-3">
                  <dt class="text-foreground/50">{{ $t('orderDetails.progress.payment') }}</dt>
                  <dd class="text-right font-medium">{{ paymentLabel }}</dd>
                </div>
                <div
                  v-if="paidAt && isPaid"
                  class="flex items-start justify-between gap-4 py-3"
                >
                  <dt class="text-foreground/50">{{ $t('orderDetails.paidOn') }}</dt>
                  <dd class="max-w-[11rem] text-right font-medium">
                    {{ formatDateTime(paidAt) }}
                  </dd>
                </div>
                <div
                  v-else-if="paymentDeadline && paymentState === 'pending'"
                  class="flex items-start justify-between gap-4 py-3"
                >
                  <dt class="text-foreground/50">{{ $t('orderDetails.paymentDue') }}</dt>
                  <dd class="max-w-[11rem] text-right font-medium">
                    {{ formatDateTime(paymentDeadline) }}
                  </dd>
                </div>
                <div
                  class="flex items-start justify-between gap-4 py-3 last:pb-0"
                >
                  <dt class="text-foreground/50">{{ $t('orderDetails.progress.delivery') }}</dt>
                  <dd class="text-right font-medium">{{ deliveryLabel }}</dd>
                </div>
                <div
                  v-if="shipBy && isPaid && role === 'seller'"
                  class="flex items-start justify-between gap-4 py-3 last:pb-0"
                >
                  <dt class="text-foreground/50">{{ $t('orderDetails.shipBy') }}</dt>
                  <dd class="max-w-[11rem] text-right font-medium">
                    {{ formatDateTime(shipBy) }}
                  </dd>
                </div>
              </dl>
            </section>

            <section class="rounded-2xl border bg-background p-5">
              <h2 class="font-semibold">{{ $t('orderDetails.deliveryAddress') }}</h2>

              <form
                v-if="canPay"
                class="mt-5 space-y-4"
                @submit.prevent="submitCheckout"
              >
                <div v-if="checkoutError" class="rounded-lg border border-red-500/30 bg-red-500/10 px-3 py-2 text-xs text-red-700 dark:text-red-300">
                  {{ checkoutError }}
                </div>

                <input
                  v-model="shippingAddressForm.fullName"
                  class="h-10 w-full rounded-lg border bg-background px-3 text-sm outline-none focus:border-foreground/40"
                  :placeholder="$t('orderDetails.fullName')"
                />
                <input
                  v-model="shippingAddressForm.addressLine1"
                  class="h-10 w-full rounded-lg border bg-background px-3 text-sm outline-none focus:border-foreground/40"
                  :placeholder="$t('orderDetails.addressLine1')"
                />
                <input
                  v-model="shippingAddressForm.addressLine2"
                  class="h-10 w-full rounded-lg border bg-background px-3 text-sm outline-none focus:border-foreground/40"
                  :placeholder="$t('orderDetails.addressLine2')"
                />
                <div class="grid grid-cols-2 gap-3">
                  <input
                    v-model="shippingAddressForm.postalCode"
                    class="h-10 w-full rounded-lg border bg-background px-3 text-sm outline-none focus:border-foreground/40"
                    :placeholder="$t('orderDetails.postcode')"
                  />
                  <input
                    v-model="shippingAddressForm.city"
                    class="h-10 w-full rounded-lg border bg-background px-3 text-sm outline-none focus:border-foreground/40"
                    :placeholder="$t('sell.city')"
                  />
                </div>
                <div class="grid grid-cols-[5rem_minmax(0,1fr)] gap-3">
                  <input
                    v-model="shippingAddressForm.countryCode"
                    maxlength="2"
                    class="h-10 w-full rounded-lg border bg-background px-3 text-sm uppercase outline-none focus:border-foreground/40"
                    :placeholder="$t('orderDetails.countryCodePlaceholder')"
                  />
                  <input
                    v-model="shippingAddressForm.country"
                    class="h-10 w-full rounded-lg border bg-background px-3 text-sm outline-none focus:border-foreground/40"
                    :placeholder="$t('orderDetails.countryPlaceholder')"
                  />
                </div>
                <input
                  v-model="shippingAddressForm.phone"
                  class="h-10 w-full rounded-lg border bg-background px-3 text-sm outline-none focus:border-foreground/40"
                  :placeholder="$t('orderDetails.phone')"
                />

                <button
                  type="submit"
                  class="min-h-10 w-full rounded-lg bg-foreground px-3 text-sm font-medium text-background transition-opacity hover:opacity-80 disabled:cursor-wait disabled:opacity-50"
                  :disabled="isCheckoutSubmitting"
                >
                  {{ isCheckoutSubmitting ? $t('orderDetails.openCheckout') : $t('orderDetails.pay', { amount: totalPriceLabel }) }}
                </button>
              </form>

              <div v-else-if="canViewShippingAddress && shippingAddressLines.length" class="mt-5 rounded-xl bg-foreground/[0.04] p-4 text-sm leading-6 text-foreground/70">
                <p
                  v-for="line in shippingAddressLines"
                  :key="line"
                >
                  {{ line }}
                </p>
              </div>

              <p v-else class="mt-4 text-sm leading-6 text-foreground/60">
                {{ $t('orderDetails.hiddenAddress') }}
              </p>
            </section>

            <section class="rounded-2xl border bg-background p-5">
              <h2 class="font-semibold">{{ $t('orderDetails.shipment') }}</h2>

              <form
                v-if="canMarkShipped"
                class="mt-5 space-y-4"
                @submit.prevent="submitShipment"
              >
                <p v-if="shipBy" class="text-sm text-foreground/60">
                  {{ $t('orderDetails.dispatchDeadline', { date: formatDateTime(shipBy) }) }}
                </p>

                <div v-if="shipmentError" class="rounded-lg border border-red-500/30 bg-red-500/10 px-3 py-2 text-xs text-red-700 dark:text-red-300">
                  {{ shipmentError }}
                </div>

                <input
                  v-model="shipmentForm.carrierName"
                  class="h-10 w-full rounded-lg border bg-background px-3 text-sm outline-none focus:border-foreground/40"
                  :placeholder="$t('orderDetails.carrierPlaceholder')"
                />
                <input
                  v-model="shipmentForm.trackingNumber"
                  class="h-10 w-full rounded-lg border bg-background px-3 text-sm outline-none focus:border-foreground/40"
                  :placeholder="$t('orderDetails.trackingPlaceholder')"
                />
                <button
                  type="submit"
                  class="min-h-10 w-full rounded-lg bg-foreground px-3 text-sm font-medium text-background transition-opacity hover:opacity-80 disabled:cursor-wait disabled:opacity-50"
                  :disabled="isShipmentSubmitting"
                >
                  {{ isShipmentSubmitting ? $t('orderDetails.saving') : $t('orderDetails.markShipped') }}
                </button>
              </form>

              <div v-else-if="trackingNumber" class="mt-5 space-y-3 text-sm">
                <div class="flex items-start justify-between gap-4">
                  <span class="text-foreground/50">{{ $t('orderDetails.carrier') }}</span>
                  <span class="text-right font-medium">{{ carrierName || $t('orderDetails.carrierUnavailable') }}</span>
                </div>
                <div class="flex items-start justify-between gap-4">
                  <span class="text-foreground/50">{{ $t('orderDetails.tracking') }}</span>
                  <span class="text-right font-medium">{{ trackingNumber }}</span>
                </div>

                <div
                  v-if="shippingState === 'issue'"
                  class="rounded-lg border border-orange-500/30 bg-orange-500/10 px-3 py-2 text-xs leading-5 text-orange-700 dark:text-orange-300"
                >
                  <p>{{ $t('orderDetails.disputedReview') }}</p>
                </div>

                <button
                  v-if="canConfirmReceived"
                  type="button"
                  class="min-h-10 w-full rounded-lg bg-foreground px-3 text-sm font-medium text-background transition-opacity hover:opacity-80 disabled:cursor-wait disabled:opacity-50"
                  :disabled="isReceiptSubmitting"
                  @click="confirmReceived"
                >
                  {{ isReceiptSubmitting ? $t('orderDetails.confirming') : $t('orderDetails.itemReceived') }}
                </button>
              </div>

              <p v-else class="mt-4 text-sm leading-6 text-foreground/60">
                {{ $t('orderDetails.trackingPending') }}
              </p>
            </section>

            <section
              v-if="role === 'buyer' || hasDisputeDetails"
              class="rounded-2xl border bg-background p-5"
            >
              <div class="flex items-start justify-between gap-4">
                <div>
                  <h2 class="font-semibold">{{ $t('orderDetails.reportProblem') }}</h2>
                  <p class="mt-1 text-sm leading-6 text-foreground/60">
                    {{ $t('orderDetails.reportDescription') }}
                  </p>
                </div>
                <span
                  v-if="hasDisputeDetails"
                  :class="[
                    'shrink-0 rounded-full border px-2.5 py-1 text-xs font-medium',
                    isDisputeOpen
                      ? 'border-orange-500/30 bg-orange-500/10 text-orange-700 dark:text-orange-300'
                      : 'border-emerald-500/30 bg-emerald-500/10 text-emerald-700 dark:text-emerald-300',
                  ]"
                >
                  {{ isDisputeOpen ? $t('orderDetails.open') : $t('orderDetails.resolved') }}
                </span>
              </div>

              <div
                v-if="hasDisputeDetails"
                class="mt-5 space-y-4 text-sm"
              >
                <div class="rounded-xl border bg-foreground/[0.02] p-4">
                  <dl class="space-y-3">
                    <div v-if="disputeReasonLabel" class="flex items-start justify-between gap-4">
                      <dt class="text-foreground/50">{{ $t('orderDetails.reason') }}</dt>
                      <dd class="text-right font-medium">{{ disputeReasonLabel }}</dd>
                    </div>
                    <div v-if="disputeOpenedAt" class="flex items-start justify-between gap-4">
                      <dt class="text-foreground/50">{{ $t('orderDetails.opened') }}</dt>
                      <dd class="text-right font-medium">{{ formatDateTime(disputeOpenedAt) }}</dd>
                    </div>
                    <div v-if="disputeResolvedAt" class="flex items-start justify-between gap-4">
                      <dt class="text-foreground/50">{{ $t('orderDetails.resolved') }}</dt>
                      <dd class="text-right font-medium">{{ formatDateTime(disputeResolvedAt) }}</dd>
                    </div>
                  </dl>

                  <p
                    v-if="disputeDescription"
                    class="mt-4 whitespace-pre-line border-t pt-4 text-sm leading-6 text-foreground/70"
                  >
                    {{ disputeDescription }}
                  </p>
                </div>

                <div v-if="disputeEvidenceItems.length">
                  <p class="text-xs font-medium text-foreground/50">{{ $t('orderDetails.evidence') }}</p>
                  <div class="mt-2 grid grid-cols-3 gap-2">
                    <a
                      v-for="evidence in disputeEvidenceItems"
                      :key="evidence.id || evidence.key"
                      :href="buildMediaUrl({ key: evidence.key })"
                      target="_blank"
                      rel="noreferrer"
                      class="aspect-square overflow-hidden rounded-lg border bg-foreground/[0.04]"
                    >
                      <img
                        :src="buildMediaUrl({ key: evidence.key })"
                        :alt="$t('orderDetails.disputeEvidenceAlt')"
                        class="h-full w-full object-cover"
                      />
                    </a>
                  </div>
                </div>

                <div
                  v-if="!isDisputeOpen && disputeDecisionName"
                  class="rounded-lg border border-emerald-500/30 bg-emerald-500/10 px-3 py-2 text-xs leading-5 text-emerald-700 dark:text-emerald-300"
                >
                  <p class="font-medium">
                    {{ $t('orderDetails.decision', { decision: disputeDecisionName === 'CompleteOrder' ? $t('orderDetails.completeOrder') : disputeDecisionName }) }}
                  </p>
                  <p v-if="disputeDecisionReason" class="mt-1">
                    {{ disputeDecisionReason }}
                  </p>
                </div>
              </div>

              <form
                v-else-if="canOpenDispute"
                class="mt-5 space-y-4"
                @submit.prevent="submitDispute"
              >
                <div
                  v-if="disputeError"
                  class="rounded-lg border border-red-500/30 bg-red-500/10 px-3 py-2 text-xs text-red-700 dark:text-red-300"
                >
                  {{ disputeError }}
                </div>

                <select
                  v-model="disputeForm.reason"
                  class="h-10 w-full rounded-lg border bg-background px-3 text-sm outline-none focus:border-foreground/40"
                >
                  <option
                    v-for="option in disputeReasonOptions"
                    :key="option.value"
                    :value="option.value"
                  >
                    {{ $t(option.labelKey) }}
                  </option>
                </select>

                <textarea
                  v-model="disputeForm.description"
                  rows="4"
                  class="w-full rounded-lg border bg-background px-3 py-2 text-sm outline-none focus:border-foreground/40"
                  :placeholder="$t('orderDetails.describeProblem')"
                />

                <label class="block">
                  <span class="text-xs font-medium text-foreground/60">
                    {{ $t('orderDetails.photoEvidence') }}
                  </span>
                  <input
                    type="file"
                    accept="image/*"
                    multiple
                    class="mt-2 block w-full text-sm file:mr-3 file:rounded-lg file:border file:bg-background file:px-3 file:py-2 file:text-sm file:text-foreground"
                    @change="onDisputeEvidenceSelected"
                  />
                </label>

                <div v-if="disputeEvidenceFiles.length" class="space-y-2">
                  <div
                    v-for="(file, index) in disputeEvidenceFiles"
                    :key="`${file.name}-${file.size}-${index}`"
                    class="flex items-center justify-between gap-3 rounded-lg bg-foreground/[0.04] px-3 py-2 text-xs"
                  >
                    <span class="truncate">{{ file.name }}</span>
                    <button
                      type="button"
                      class="shrink-0 rounded border px-2 py-1 text-foreground/60 transition hover:text-foreground"
                      @click="removeDisputeEvidenceFile(index)"
                    >
                      {{ $t('orderDetails.remove') }}
                    </button>
                  </div>
                </div>

                <button
                  type="submit"
                  class="min-h-10 w-full rounded-lg border border-orange-500/40 px-3 text-sm font-medium text-orange-700 transition hover:bg-orange-500/10 disabled:cursor-wait disabled:opacity-50 dark:text-orange-300"
                  :disabled="isDisputeSubmitting"
                >
                  {{ isDisputeSubmitting ? $t('sell.submitting') : $t('orderDetails.reportProblem') }}
                </button>
              </form>

              <p v-else class="mt-4 text-sm leading-6 text-foreground/60">
                {{ $t('orderDetails.problemsAfterPayment') }}
              </p>
            </section>

            <section class="rounded-2xl border bg-background p-5">
              <h2 class="font-semibold">{{ $t('orderDetails.transaction') }}</h2>

              <dl class="mt-5 space-y-5 text-sm">
                <div class="flex items-start justify-between gap-4">
                  <dt class="text-foreground/50">{{ counterpartyLabel }}</dt>
                  <dd class="text-right font-medium">{{ counterpartyName }}</dd>
                </div>
                <div class="flex items-start justify-between gap-4">
                  <dt class="text-foreground/50">{{ $t('orderDetails.yourRole') }}</dt>
                  <dd class="font-medium">{{ roleLabel }}</dd>
                </div>

                <div class="border-t pt-4">
                  <dt class="text-xs text-foreground/45">{{ $t('orderDetails.orderReference') }}</dt>
                  <dd class="mt-2 flex items-center justify-between gap-3">
                    <span class="font-medium">
                      #{{ shortReference(orderReference) }}
                    </span>
                    <button
                      type="button"
                      class="rounded-lg border px-2.5 py-1.5 text-xs transition hover:bg-foreground hover:text-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground"
                      @click="copyReference('order', orderReference)"
                    >
                      {{ copiedReference === 'order' ? $t('orderDetails.copied') : $t('orderDetails.copy') }}
                    </button>
                  </dd>
                </div>

                <div v-if="auctionReference">
                  <dt class="text-xs text-foreground/45">{{ $t('orderDetails.auctionReference') }}</dt>
                  <dd class="mt-2 flex items-center justify-between gap-3">
                    <span class="font-medium">
                      #{{ shortReference(auctionReference) }}
                    </span>
                    <button
                      type="button"
                      class="rounded-lg border px-2.5 py-1.5 text-xs transition hover:bg-foreground hover:text-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground"
                      @click="copyReference('auction', auctionReference)"
                    >
                      {{ copiedReference === 'auction' ? $t('orderDetails.copied') : $t('orderDetails.copy') }}
                    </button>
                  </dd>
                </div>
              </dl>
            </section>
          </aside>
        </div>
      </template>
    </div>
  </main>
</template>
