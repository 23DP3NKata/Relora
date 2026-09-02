<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useI18n } from 'vue-i18n'

import { orderService } from '@/app/services/orderService'
import { auctionService } from '@/app/services/auctionService'
import { itemService } from '@/app/services/lotService'
import {
  getUserRoleInOrder,
  normalizeOrderStatus,
  normalizePaymentStatus,
} from '@/app/helpers/orderHelpers'
import { useAuthStore } from '@/stores/authStore'
import { useLocalePath } from '@/composables/useLocalePath'
import type { OrderItem, OrderUserRole } from '@/types/order'

type DataRecord = Record<string, unknown>
type OrderTab = 'buyer' | 'seller'
type OrderStatusKey =
  | 'pending'
  | 'payment_expired'
  | 'paid'
  | 'shipped'
  | 'buyer_protection'
  | 'completed'
  | 'failed'
  | 'cancelled'
  | 'unknown'

type AuctionSummary = {
  id: string
  lotId: string
  endsAt: string | null
}

type LotSummary = {
  title: string
  brand: string
  imageUrl: string
  tags: string[]
}

type OrderListRow = {
  id: string
  order: OrderItem
  title: string
  brand: string
  imageUrl: string
  tags: string[]
  transactionAt: string | null
  paidAt: string | null
  paymentDeadline: string | null
  statusName: string
  role: OrderUserRole
}

const authStore = useAuthStore()
const { user, isAuthenticated } = storeToRefs(authStore)
const { t, te, locale } = useI18n()
const localePath = useLocalePath()

const isLoading = ref(true)
const isAuthChecking = ref(true)
const errorMessage = ref('')
const rows = ref<OrderListRow[]>([])
const activeTab = ref<OrderTab>('buyer')
const failedImageIds = ref<Set<string>>(new Set())

const tabs = computed(() => [
  { id: 'buyer' as const, label: t('orders.purchasesTab') },
  { id: 'seller' as const, label: t('orders.ordersTab') },
])

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

const nullableText = (value: unknown): string | null => {
  if (typeof value !== 'string' && typeof value !== 'number') return null
  const text = String(value).trim()
  return text || null
}

const extractCollection = <T,>(source: unknown, keys: string[]): T[] => {
  if (Array.isArray(source)) return source as T[]
  if (!source || typeof source !== 'object') return []

  const record = source as DataRecord

  for (const key of keys) {
    const candidate = record[key]

    if (Array.isArray(candidate)) return candidate as T[]

    if (candidate && typeof candidate === 'object') {
      const nestedRecord = asRecord(candidate)

      for (const nestedKey of keys) {
        const nestedCandidate = nestedRecord[nestedKey]
        if (Array.isArray(nestedCandidate)) return nestedCandidate as T[]
      }
    }
  }

  return []
}

const orderId = (order: OrderItem) =>
  String(order.id ?? order.orderId ?? '').trim()

const shortReference = (value: string) =>
  value ? value.slice(0, 8).toUpperCase() : t('common.unavailable')

const formatMoney = (order: OrderItem) => {
  const rawAmount = order.totalPrice ?? order.price ?? order.amount
  const amount = typeof rawAmount === 'number' ? rawAmount : Number(rawAmount)
  const rawCurrency = String(order.currency ?? 'EUR').toUpperCase()
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

const formatDate = (value: string | null) => {
  if (!value) return ''

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return ''

  return new Intl.DateTimeFormat(locale.value, {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  }).format(date)
}

const dateTimestamp = (value: string | null) => {
  if (!value) return 0
  const timestamp = new Date(value).getTime()
  return Number.isNaN(timestamp) ? 0 : timestamp
}

const currentUserId = () => {
  const currentUser = asRecord(user.value)
  return firstText(currentUser, ['userId', 'id'])
}

const resolveOrderImage = (order: OrderItem) => {
  if (order.imageUrl) return order.imageUrl
  if (Array.isArray(order.photos) && order.photos.length > 0) {
    return order.photos[0] ?? ''
  }
  if (Array.isArray(order.media) && order.media.length > 0) {
    return order.media[0]?.url ?? ''
  }
  return ''
}

const normalizeAuction = (source: unknown): AuctionSummary | null => {
  const record = asRecord(source)
  const id = firstText(record, ['auctionId', 'id'])

  if (!id) return null

  return {
    id,
    lotId: firstText(record, ['lotId']),
    endsAt: nullableText(firstValue(record, ['endsAt', 'endedAt'])),
  }
}

const normalizeLot = (source: unknown): LotSummary => {
  const record = asRecord(source)
  const media = record.media
  let imageUrl = ''

  if (Array.isArray(media) && media.length > 0) {
    imageUrl = firstText(asRecord(media[0]), ['url'])
  }

  return {
    title: firstText(record, ['title']) || t('orders.untitledPiece'),
    brand: firstText(record, ['brand']),
    imageUrl,
    tags: [
      firstText(record, ['sizeName']),
      firstText(record, ['color']),
      firstText(record, ['conditionName']),
    ].filter(Boolean),
  }
}

const statusKey = (row: OrderListRow): OrderStatusKey => {
  const label = row.statusName.toLowerCase()

  if (/(buyerprotection|buyer protection|disputed)/.test(label)) return 'buyer_protection'
  if (/(paymentexpired|payment expired)/.test(label)) return 'payment_expired'
  if (/(cancel|refund)/.test(label)) return 'cancelled'
  if (/(fail|declin|expired)/.test(label)) return 'failed'
  if (/(delivered|completed)/.test(label)) return 'completed'
  if (/(shipped|dispatched|in transit)/.test(label)) return 'shipped'
  if (row.paidAt || /(^|\s)(paid|payment confirmed)(\s|$)/.test(label)) {
    return 'paid'
  }
  if (/(pending|awaiting|created|unpaid)/.test(label)) return 'pending'

  const orderRecord = asRecord(row.order)
  const normalizedOrder = normalizeOrderStatus(
    firstValue(orderRecord, ['orderStatus', 'status']) as
      | string
      | number
      | null
      | undefined,
  )

  if (normalizedOrder === 'paid' || normalizedOrder === 'awaiting_shipment') return 'paid'
  if (normalizedOrder === 'shipped') return 'shipped'
  if (normalizedOrder === 'completed') return 'completed'
  if (normalizedOrder === 'payment_expired') return 'payment_expired'
  if (normalizedOrder === 'buyer_protection' || normalizedOrder === 'disputed') return 'buyer_protection'
  if (normalizedOrder === 'cancelled') return 'cancelled'
  if (normalizedOrder === 'failed') return 'failed'
  if (normalizedOrder === 'unpaid') return 'pending'

  const normalizedPayment = normalizePaymentStatus(
    firstValue(orderRecord, ['paymentStatus', 'orderStatus', 'status']) as
      | string
      | number
      | null
      | undefined,
  )

  if (normalizedPayment === 'paid') return 'paid'
  if (normalizedPayment === 'pending') return 'pending'
  if (normalizedPayment === 'failed') return 'failed'
  return 'unknown'
}

const statusLabel = (row: OrderListRow) => {
  const key = statusKey(row)

  if (key === 'paid') return t('orders.status.paid')
  if (key === 'pending') return t('orders.status.pending')
  if (key === 'payment_expired') return t('orders.status.paymentExpired')
  if (key === 'shipped') return t('orders.status.shipped')
  if (key === 'buyer_protection') return t('orders.status.buyerProtection')
  if (key === 'completed') return t('orders.status.completed')
  if (key === 'failed') return t('orders.status.failed')
  if (key === 'cancelled') return t('orders.status.cancelled')
  return row.statusName || t('orders.status.unknown')
}

const statusBadgeClass = (row: OrderListRow) => {
  const key = statusKey(row)

  if (key === 'buyer_protection') {
    return 'border-orange-500/30 bg-orange-500/10 text-orange-700 dark:text-orange-300'
  }
  if (key === 'completed' || key === 'paid') {
    return 'border-emerald-500/30 bg-emerald-500/10 text-emerald-700 dark:text-emerald-300'
  }
  if (key === 'shipped') {
    return 'border-blue-500/30 bg-blue-500/10 text-blue-700 dark:text-blue-300'
  }
  if (key === 'pending') {
    return 'border-amber-500/30 bg-amber-500/10 text-amber-700 dark:text-amber-300'
  }
  if (key === 'failed' || key === 'cancelled' || key === 'payment_expired') {
    return 'border-red-500/30 bg-red-500/10 text-red-700 dark:text-red-300'
  }
  return 'border-foreground/15 bg-foreground/[0.04] text-foreground/60'
}

const statusHint = (row: OrderListRow) => {
  const key = statusKey(row)

  if (key === 'paid') {
    return row.role === 'seller'
      ? t('orders.hint.paidSeller')
      : t('orders.hint.paidBuyer')
  }
  if (key === 'pending') {
    const deadline = formatDate(row.paymentDeadline)

    if (row.role === 'seller') {
      return t('orders.hint.pendingSeller')
    }

    return deadline
      ? t('orders.hint.pendingBuyerDeadline', { date: deadline })
      : t('orders.hint.pendingBuyer')
  }
  if (key === 'shipped') {
    return row.role === 'seller'
      ? t('orders.hint.shippedSeller')
      : t('orders.hint.shippedBuyer')
  }
  if (key === 'completed') return t('orders.hint.completed')
  if (key === 'buyer_protection') {
    return row.role === 'seller'
      ? t('orders.hint.disputeSeller')
      : t('orders.hint.disputeBuyer')
  }
  if (key === 'payment_expired') {
    return row.role === 'seller'
      ? t('orders.hint.expiredSeller')
      : t('orders.hint.expiredBuyer')
  }
  if (key === 'failed') {
    return row.role === 'seller'
      ? t('orders.hint.failedSeller')
      : t('orders.hint.failedBuyer')
  }
  if (key === 'cancelled') return t('orders.hint.cancelled')
  return t('orders.hint.unknown')
}

const transactionLabel = (row: OrderListRow) => {
  const transactionDate = formatDate(row.transactionAt)

  if (transactionDate) {
    return row.role === 'seller'
      ? t('orders.transaction.sold', { date: transactionDate })
      : t('orders.transaction.purchased', { date: transactionDate })
  }

  const paymentDate = formatDate(row.paidAt)
  if (paymentDate) return t('orders.transaction.paid', { date: paymentDate })
  return t('orders.dateUnavailable')
}

const totalLabel = (role: OrderUserRole) => {
  if (role === 'seller') return t('orders.total.sale')
  return t('orders.total.order')
}

const localizeCatalogValue = (value: string) => {
  const key = `catalog.options.${value}`

  return te(key) ? t(key) : value
}

const orderedRows = computed(() =>
  [...rows.value].sort((left, right) => {
    const leftDate = left.transactionAt ?? left.paidAt
    const rightDate = right.transactionAt ?? right.paidAt
    return dateTimestamp(rightDate) - dateTimestamp(leftDate)
  }),
)

const visibleRows = computed(() =>
  orderedRows.value.filter((row) => row.role === activeTab.value),
)

const emptyState = computed(() => {
  if (activeTab.value === 'buyer') {
    return {
      eyebrow: t('orders.noPurchasesEyebrow'),
      title: t('orders.noPurchasesTitle'),
      description: t('orders.noPurchasesDescription'),
      action: t('orders.exploreAuctions'),
      to: localePath('/catalog'),
    }
  }

  return {
    eyebrow: t('orders.noOrdersEyebrow'),
    title: t('orders.noOrdersTitle'),
    description: t('orders.noOrdersDescription'),
    action: t('orders.sellItem'),
    to: localePath('/sell'),
  }
})

const markImageAsFailed = (id: string) => {
  failedImageIds.value = new Set([...failedImageIds.value, id])
}

const loadOrders = async () => {
  try {
    isLoading.value = true
    errorMessage.value = ''
    rows.value = []
    failedImageIds.value = new Set()

    const result: unknown = await orderService.getMyOrders()
    const orders = extractCollection<OrderItem>(result, [
      'orders',
      'items',
      'data',
    ])

    if (orders.length === 0) return

    const auctionResult: unknown = await auctionService
      .getAuctions()
      .catch(() => [])
    const auctionItems = extractCollection<unknown>(auctionResult, [
      'auctions',
      'items',
      'data',
    ])
    const auctionMap = new Map<string, AuctionSummary>()

    for (const item of auctionItems) {
      const auction = normalizeAuction(item)
      if (auction) auctionMap.set(auction.id, auction)
    }

    const orderAuctionIds = Array.from(
      new Set(
        orders
          .map((item) => firstText(asRecord(item), ['auctionId']))
          .filter(Boolean),
      ),
    )
    const missingAuctionIds = orderAuctionIds.filter(
      (id) => !auctionMap.has(id),
    )

    await Promise.all(
      missingAuctionIds.map(async (auctionId) => {
        try {
          const details = await auctionService.getAuctionDetails(auctionId)
          const auction = normalizeAuction(details)
          if (auction) auctionMap.set(auction.id, auction)
        } catch (error) {
          console.error(`Failed to enrich order auction ${auctionId}`, error)
        }
      }),
    )

    const lotIds = Array.from(
      new Set(
        orderAuctionIds
          .map((auctionId) => auctionMap.get(auctionId)?.lotId)
          .filter((lotId): lotId is string => Boolean(lotId)),
      ),
    )
    const lotMap = new Map<string, LotSummary>()

    await Promise.all(
      lotIds.map(async (lotId) => {
        try {
          const lot = await itemService.getLot(lotId)
          lotMap.set(lotId, normalizeLot(lot))
        } catch (error) {
          console.error(`Failed to enrich order lot ${lotId}`, error)
        }
      }),
    )

    rows.value = orders.map((order, index) => {
      const orderRecord = asRecord(order)
      const auctionId = firstText(orderRecord, ['auctionId'])
      const auction = auctionMap.get(auctionId)
      const lot = auction?.lotId ? lotMap.get(auction.lotId) : undefined
      const id = orderId(order) || `${auctionId || 'order'}-${index}`

      return {
        id,
        order,
        title:
          lot?.title ||
          firstText(orderRecord, ['title', 'lotTitle']) ||
          t('orders.untitledPiece'),
        brand: lot?.brand || firstText(orderRecord, ['brand']),
        imageUrl: lot?.imageUrl || resolveOrderImage(order),
        tags: lot?.tags ?? [],
        transactionAt:
          nullableText(firstValue(orderRecord, ['createdAt', 'createdDate'])) ??
          auction?.endsAt ??
          null,
        paidAt: nullableText(
          firstValue(orderRecord, [
            'paidAtUtc',
            'paidAt',
            'paymentCompletedAt',
          ]),
        ),
        paymentDeadline: nullableText(
          firstValue(orderRecord, ['paymentDeadlineUtc', 'paymentDeadline']),
        ),
        statusName: firstText(orderRecord, [
          'orderStatusName',
          'paymentStatusName',
          'statusName',
        ]),
        role: getUserRoleInOrder(order, currentUserId()),
      }
    })

    const hasPurchases = rows.value.some((row) => row.role === 'buyer')
    const hasOrders = rows.value.some((row) => row.role === 'seller')

    if (!hasPurchases && hasOrders) activeTab.value = 'seller'
  } catch (error) {
    console.error('Failed to load orders', error)
    rows.value = []
    errorMessage.value = t('orders.loadFailed')
  } finally {
    isLoading.value = false
  }
}

onMounted(async () => {
  try {
    if (!isAuthenticated.value) await authStore.checkAuth()
  } catch (error) {
    console.error('Failed to check authentication', error)
  } finally {
    isAuthChecking.value = false
  }

  if (!isAuthenticated.value) {
    isLoading.value = false
    return
  }

  await loadOrders()
})
</script>

<template>
  <main class="min-h-[70vh] bg-background text-foreground">
    <div class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <header class="mb-7">
        <h1 class="text-3xl font-semibold sm:text-4xl">{{ $t('orders.title') }}</h1>
        <p class="mt-2 max-w-2xl text-sm leading-6 text-foreground/65">
          {{ $t('orders.description') }}
        </p>
      </header>

      <section
        v-if="isAuthChecking"
        class="rounded-2xl border p-6"
        :aria-label="$t('orders.checkingSession')"
      >
        <div class="animate-pulse">
          <div class="h-4 w-40 rounded bg-foreground/10" />
          <div class="mt-3 h-3 w-64 max-w-full rounded bg-foreground/[0.07]" />
        </div>
      </section>

      <section
        v-else-if="!isAuthenticated"
        class="rounded-2xl border bg-background p-6"
      >
        <div class="max-w-lg">
          <h2 class="text-lg font-semibold">{{ $t('orders.signInTitle') }}</h2>
          <p class="mt-2 text-sm leading-6 text-foreground/70">
            {{ $t('orders.signInDescription') }}
          </p>
          <RouterLink
            :to="localePath('/login')"
            class="mt-5 inline-flex rounded-lg border px-4 py-2 text-sm transition hover:bg-foreground hover:text-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground focus-visible:ring-offset-2 focus-visible:ring-offset-background"
          >
            {{ $t('navigation.signIn') }}
          </RouterLink>
        </div>
      </section>

      <template v-else>
        <nav
          class="mb-6 grid w-full grid-cols-2 gap-1 rounded-2xl border bg-background p-1.5 sm:w-auto sm:max-w-md"
          :aria-label="$t('orders.filterAria')"
        >
          <button
            v-for="tab in tabs"
            :key="tab.id"
            type="button"
            :aria-pressed="activeTab === tab.id"
            :class="[
              'min-h-10 rounded-xl px-5 text-sm font-medium transition sm:min-w-44',
              'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground focus-visible:ring-offset-2 focus-visible:ring-offset-background',
              activeTab === tab.id
                ? 'bg-foreground text-background'
                : 'text-foreground/55 hover:bg-foreground/5 hover:text-foreground',
            ]"
            @click="activeTab = tab.id"
          >
            {{ tab.label }}
          </button>
        </nav>

        <div
          v-if="isLoading"
          class="space-y-3"
          aria-live="polite"
          :aria-label="$t('orders.loadingOrders')"
        >
          <div
            v-for="index in 3"
            :key="index"
            class="grid animate-pulse grid-cols-[5.75rem_minmax(0,1fr)] gap-4 rounded-2xl border p-4 sm:grid-cols-[7rem_minmax(0,1fr)] sm:gap-5 sm:p-5 lg:grid-cols-[7rem_minmax(0,1fr)_9rem_15rem_8rem] lg:items-center"
          >
            <div class="aspect-[4/5] rounded-xl bg-foreground/[0.07]" />
            <div>
              <div class="h-3 w-20 rounded bg-foreground/[0.07]" />
              <div class="mt-3 h-5 w-3/4 rounded bg-foreground/[0.07]" />
              <div class="mt-3 h-4 w-1/2 rounded bg-foreground/[0.05]" />
              <div class="mt-5 h-3 w-44 rounded bg-foreground/[0.05]" />
            </div>
            <div
              class="col-span-2 h-14 rounded-xl bg-foreground/[0.05] lg:col-span-1"
            />
            <div
              class="col-span-2 h-16 rounded-xl bg-foreground/[0.05] lg:col-span-1"
            />
            <div
              class="col-span-2 h-10 rounded-lg bg-foreground/[0.07] lg:col-span-1"
            />
          </div>
        </div>

        <div
          v-else-if="errorMessage"
          class="flex flex-col items-start justify-between gap-5 rounded-2xl border border-red-500/30 bg-red-500/10 p-6 sm:flex-row sm:items-center"
          role="alert"
        >
          <div>
            <p class="text-sm font-medium text-red-600 dark:text-red-300">
              {{ $t('orders.unableLoad') }}
            </p>
            <p class="mt-1 text-sm text-foreground/60">{{ errorMessage }}</p>
          </div>
          <button
            type="button"
            class="rounded-lg border px-4 py-2 text-sm transition hover:bg-foreground hover:text-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground"
            @click="loadOrders"
          >
            {{ $t('common.tryAgain') }}
          </button>
        </div>

        <div
          v-else-if="visibleRows.length === 0"
          class="rounded-2xl border bg-background px-6 py-12 sm:px-10"
        >
          <div class="mx-auto max-w-lg text-center">
            <p class="text-sm text-foreground/50">
              {{ emptyState.eyebrow }}
            </p>
            <h2 class="mt-2 text-xl font-semibold sm:text-2xl">
              {{ emptyState.title }}
            </h2>
            <p
              class="mx-auto mt-3 max-w-md text-sm leading-6 text-foreground/70"
            >
              {{ emptyState.description }}
            </p>
            <RouterLink
              :to="emptyState.to"
              class="mt-6 inline-flex rounded-lg border px-4 py-2 text-sm transition hover:bg-foreground hover:text-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground focus-visible:ring-offset-2 focus-visible:ring-offset-background"
            >
              {{ emptyState.action }}
            </RouterLink>
          </div>
        </div>

        <div v-else class="space-y-3">
          <article
            v-for="row in visibleRows"
            :key="row.id"
            class="group rounded-2xl border bg-background p-4 transition-colors hover:border-foreground/30 sm:p-5"
          >
            <div
              class="grid grid-cols-[5.75rem_minmax(0,1fr)] gap-4 sm:grid-cols-[7rem_minmax(0,1fr)] sm:gap-5 lg:grid-cols-[7rem_minmax(0,1fr)_9rem_15rem_8rem] lg:items-center"
            >
              <div
                class="relative aspect-[4/5] self-start overflow-hidden rounded-xl border bg-foreground/[0.04]"
              >
                <img
                  v-if="row.imageUrl && !failedImageIds.has(row.id)"
                  :src="row.imageUrl"
                  :alt="row.title"
                  class="h-full w-full object-cover transition duration-300 group-hover:scale-[1.02]"
                  loading="lazy"
                  @error="markImageAsFailed(row.id)"
                />
                <div
                  v-else
                  class="flex h-full items-center justify-center p-3 text-center"
                >
                  <span class="text-xs text-foreground/40">
                    {{ $t('orders.imageUnavailable') }}
                  </span>
                </div>
              </div>

              <div class="min-w-0 self-center">
                <p
                  class="truncate text-xs font-medium uppercase tracking-[0.14em] text-foreground/45"
                >
                  {{ row.brand || $t('orders.auctionPiece') }}
                </p>
                <h2
                  class="mt-1.5 line-clamp-2 text-base font-semibold leading-snug sm:text-lg"
                >
                  {{ row.title }}
                </h2>

                <div v-if="row.tags.length" class="mt-3 flex flex-wrap gap-1.5">
                  <span
                    v-for="tag in row.tags"
                    :key="tag"
                    class="rounded-full border px-2 py-0.5 text-[11px] text-foreground/55"
                  >
                    {{ localizeCatalogValue(tag) }}
                  </span>
                </div>

                <p class="mt-4 text-xs text-foreground/45">
                  {{ $t('orders.orderNumber', { reference: shortReference(row.id) }) }}
                  <span class="mx-1.5" aria-hidden="true">·</span>
                  {{ transactionLabel(row) }}
                </p>
              </div>

              <div
                class="col-span-2 flex items-end justify-between gap-4 border-t pt-4 lg:col-span-1 lg:block lg:border-t-0 lg:pt-0"
              >
                <p class="text-xs text-foreground/45">
                  {{ totalLabel(row.role) }}
                </p>
                <p
                  class="text-xl font-semibold tabular-nums lg:mt-1 lg:text-lg"
                >
                  {{ formatMoney(row.order) }}
                </p>
              </div>

              <div
                class="col-span-2 rounded-xl border bg-foreground/[0.02] p-3 lg:col-span-1 lg:border-0 lg:bg-transparent lg:p-0"
              >
                <span
                  :class="[
                    'inline-flex rounded-full border px-2.5 py-1 text-xs font-medium',
                    statusBadgeClass(row),
                  ]"
                >
                  {{ statusLabel(row) }}
                </span>
                <p
                  class="mt-2 line-clamp-2 text-xs leading-5 text-foreground/60"
                >
                  {{ statusHint(row) }}
                </p>
              </div>

              <RouterLink
                :to="localePath(`/orders/${row.id}`)"
                :aria-label="$t('orders.viewOrderFor', { title: row.title })"
                class="col-span-2 inline-flex min-h-10 items-center justify-center gap-2 rounded-lg bg-foreground px-4 text-sm text-background transition hover:opacity-80 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground focus-visible:ring-offset-2 focus-visible:ring-offset-background lg:col-span-1"
              >
                {{ $t('orders.details') }}
                <span aria-hidden="true">→</span>
              </RouterLink>
            </div>
          </article>
        </div>
      </template>
    </div>
  </main>
</template>
