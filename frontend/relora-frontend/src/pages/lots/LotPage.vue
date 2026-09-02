<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useI18n } from 'vue-i18n'

import { setPageTitle } from '@/app/seo'
import { itemService } from '@/app/services/lotService'
import { auctionService } from '@/app/services/auctionService'
import {
  auctionRealtimeService,
  type AuctionEndedEvent,
  type AuctionStartedEvent,
  type BidPlacedEvent,
} from '@/app/services/auctionRealtimeService'
import { bidService, type Bid } from '@/app/services/bidService'
import { useAuthStore } from '@/stores/authStore'
import type { AuctionDetails } from '@/types/auction'
import type { Lot } from '@/types/lot'

type Point = {
  x: number
  y: number
}

const MIN_MODAL_ZOOM = 1
const MAX_MODAL_ZOOM = 5
const MODAL_ZOOM_STEP = 0.5
const MAGNIFIER_SIZE = 220
const MAGNIFIER_ZOOM = 2

const route = useRoute()
const authStore = useAuthStore()
const { user, isAuthenticated } = storeToRefs(authStore)
const { t, te, locale } = useI18n()

const lot = ref<Lot | null>(null)
const auction = ref<AuctionDetails | null>(null)
const bids = ref<Bid[]>([])

const isLoading = ref(true)
const isBidLoading = ref(false)
const isBidSubmitting = ref(false)

const errorMessage = ref('')
const bidError = ref('')
const bidSuccess = ref('')
const bidAmount = ref<number | null>(null)

const selectedImageIndex = ref(0)
const isGalleryOpen = ref(false)

const galleryRoot = ref<HTMLElement | null>(null)
const galleryStage = ref<HTMLElement | null>(null)
const galleryImage = ref<HTMLImageElement | null>(null)

const modalZoom = ref(MIN_MODAL_ZOOM)
const modalPosition = ref<Point>({ x: 0, y: 0 })
const isDraggingImage = ref(false)
const dragStartPointer = ref<Point>({ x: 0, y: 0 })
const dragStartPosition = ref<Point>({ x: 0, y: 0 })

const isMagnifierEnabled = ref(true)
const isMagnifierVisible = ref(false)
const magnifierStyle = ref<Record<string, string>>({})

const currentTime = ref(Date.now())
let timer: number | undefined
let latestLotRequestId = 0
let previousBodyOverflow = ''
let previouslyFocusedElement: HTMLElement | null = null
let subscribedAuctionId: string | null = null
let subscribedLotId: string | null = null
let unsubscribeBidPlaced: (() => void) | null = null
let unsubscribeAuctionEnded: (() => void) | null = null
let unsubscribeAuctionStarted: (() => void) | null = null
let realtimeRefreshPromise: Promise<void> | null = null
let realtimeRefreshQueued = false

const lotIdParam = computed(() =>
  String(route.params.lotId ?? route.params.id ?? ''),
)

const isOwner = computed(
  () => !!lot.value?.seller?.id && lot.value.seller.id === user.value?.userId,
)

const normalizedLotStatus = computed(() => {
  const raw = String(lot.value?.statusName ?? lot.value?.status ?? '').toLowerCase()

  if (raw.includes('draft')) return 'draft'
  if (raw.includes('pend')) return 'pending'
  if (raw.includes('reject')) return 'rejected'
  if (raw.includes('sold')) return 'sold'
  if (raw.includes('expir')) return 'expired'
  if (raw.includes('active')) return 'active'
  if (raw.includes('listed')) return 'listed'
  if (raw.includes('publish') || raw.includes('approve')) return 'approved'

  return 'unknown'
})

const isPublicLot = computed(() =>
  ['approved', 'listed', 'sold'].includes(normalizedLotStatus.value),
)

const canSeeLot = computed(() => isPublicLot.value || isOwner.value)

const visibleLotTitle = computed(() => {
  if (!canSeeLot.value) {
    return ''
  }

  return lot.value?.title?.trim() ?? ''
})

const isAuctionActive = computed(
  () => auction.value?.status?.toLowerCase() === 'active',
)

const canBid = computed(
  () =>
    isAuthenticated.value &&
    !!auction.value?.auctionId &&
    isAuctionActive.value,
)

const sortedBids = computed(() =>
  [...bids.value].sort(
    (a, b) =>
      new Date(b.placedAt).getTime() - new Date(a.placedAt).getTime(),
  ),
)

const imageCount = computed(() => lot.value?.media?.length ?? 0)
const hasMultipleImages = computed(() => imageCount.value > 1)

const selectedImage = computed(
  () => lot.value?.media?.[selectedImageIndex.value]?.url ?? '',
)

const modalImageStyle = computed(() => ({
  transform: `translate3d(${modalPosition.value.x}px, ${modalPosition.value.y}px, 0) scale(${modalZoom.value})`,
  transformOrigin: 'center center',
}))

const galleryHint = computed(() => {
  if (modalZoom.value > MIN_MODAL_ZOOM) {
    return t('lot.galleryDragZoomReset')
  }

  if (isMagnifierEnabled.value) {
    return t('lot.galleryInspectZoom')
  }

  return t('lot.galleryZoomBrowse')
})

const auctionTimeLeft = computed(() => {
  if (!auction.value?.endsAt) {
    return {
      total: 0,
      days: 0,
      hours: 0,
      minutes: 0,
      seconds: 0,
    }
  }

  const diff = new Date(auction.value.endsAt).getTime() - currentTime.value
  const total = Math.max(0, diff)

  return {
    total,
    days: Math.floor(total / (1000 * 60 * 60 * 24)),
    hours: Math.floor((total / (1000 * 60 * 60)) % 24),
    minutes: Math.floor((total / (1000 * 60)) % 60),
    seconds: Math.floor((total / 1000) % 60),
  }
})

const isAuctionEnded = computed(
  () => !!auction.value?.endsAt && auctionTimeLeft.value.total <= 0,
)

const formatMoney = (
  amount: number | null | undefined,
  currency: string | null | undefined,
) => {
  if (amount === null || amount === undefined) return '—'

  return new Intl.NumberFormat(locale.value, {
    style: 'currency',
    currency: currency ?? 'EUR',
    maximumFractionDigits: 2,
  }).format(amount)
}

const formatDate = (value: string | null | undefined) => {
  if (!value) return '—'

  return new Date(value).toLocaleString(locale.value)
}

const localizeCatalogValue = (value: string | null | undefined) => {
  if (!value) return '—'

  const key = `catalog.options.${value}`

  return te(key) ? t(key) : value
}

const localizeMessageKey = (
  key: string | null | undefined,
  fallback: string | null | undefined,
) => {
  if (key && te(key)) {
    return t(key)
  }

  return fallback?.trim() || localizeCatalogValue(null)
}

const formatYear = (value: number | null | undefined) => {
  return value
    ? String(value)
    : localizeCatalogValue(null)
}

const categoryBreadcrumb = computed(() => {
  const currentLot = lot.value

  if (!currentLot) {
    return []
  }

  const values = [
    currentLot.department
      ? localizeMessageKey(
          `departments.${currentLot.department}`,
          currentLot.departmentName ?? currentLot.department,
        )
      : localizeCatalogValue(currentLot.genderName),
    localizeMessageKey(
      currentLot.rootCategoryNameKey,
      currentLot.rootCategorySlug ?? currentLot.categoryName,
    ),
    localizeMessageKey(
      currentLot.categoryNameKey,
      currentLot.categorySlug ?? currentLot.categoryName,
    ),
  ]

  return values.filter(
    (value, index) =>
      value !== localizeCatalogValue(null) &&
      values.indexOf(value) === index,
  )
})

const primaryColorLabel = computed(() => {
  const primaryColor = lot.value?.primaryColor

  if (primaryColor) {
    return localizeMessageKey(
      primaryColor.nameKey,
      primaryColor.code,
    )
  }

  return lot.value?.color
    ? lot.value.color
    : localizeCatalogValue(null)
})

const secondaryColorsLabel = computed(() => {
  const colors = lot.value?.secondaryColors ?? []

  return colors
    .map(color =>
      localizeMessageKey(
        color.nameKey,
        color.code,
      ),
    )
    .join(', ')
})

const materialsLabel = computed(() => {
  const materials = lot.value?.materials ?? []

  return materials
    .map(material => {
      const name = localizeMessageKey(
        material.nameKey,
        material.otherName ?? material.code,
      )

      return material.percentage !== null && material.percentage !== undefined
        ? `${name} ${material.percentage}%`
        : name
    })
    .join(', ')
})

const proofStatusLabel = computed(() => {
  const status = lot.value?.proofStatus

  if (!status || status === 'NotProvided') {
    return ''
  }

  const key = `lot.proofStatus.${status}`

  return te(key) ? t(key) : status
})

const detailRows = computed(() => {
  const currentLot = lot.value

  if (!currentLot) {
    return []
  }

  return [
    {
      label: t('catalog.department'),
      value: currentLot.department
        ? localizeMessageKey(
            `departments.${currentLot.department}`,
            currentLot.departmentName ?? currentLot.department,
          )
        : localizeCatalogValue(currentLot.genderName),
    },
    {
      label: t('catalog.category'),
      value: categoryBreadcrumb.value.slice(1).join(' / '),
    },
    {
      label: t('lot.modelName'),
      value: currentLot.modelName,
    },
    {
      label: t('catalog.size'),
      value: localizeCatalogValue(currentLot.sizeName),
    },
    {
      label: t('catalog.condition'),
      value: localizeCatalogValue(currentLot.conditionName),
    },
    {
      label: t('lot.primaryColor'),
      value: primaryColorLabel.value,
    },
    {
      label: t('lot.secondaryColors'),
      value: secondaryColorsLabel.value,
    },
    {
      label: t('lot.materials'),
      value: materialsLabel.value,
    },
    {
      label: t('lot.productionYear'),
      value: currentLot.productionYear
        ? formatYear(currentLot.productionYear)
        : '',
    },
    {
      label: t('lot.acquisitionYear'),
      value: currentLot.acquisitionYear
        ? formatYear(currentLot.acquisitionYear)
        : '',
    },
    {
      label: t('lot.vintage'),
      value: currentLot.isVintage
        ? t('lot.vintageConfirmed')
        : '',
    },
    {
      label: t('lot.proofOfOrigin'),
      value: proofStatusLabel.value,
    },
    {
      label: t('lot.style'),
      value: currentLot.style,
    },
    {
      label: t('lot.year'),
      value: currentLot.age,
    },
    {
      label: t('lot.location'),
      value: currentLot.city && currentLot.country
        ? `${currentLot.city}, ${currentLot.country}`
        : currentLot.country,
    },
  ].filter(row => Boolean(row.value?.trim()))
})

const measurementRows = computed(() =>
  (lot.value?.measurements ?? []).map(measurement => ({
    key: measurement.key,
    label: localizeMessageKey(
      measurement.labelKey,
      measurement.key,
    ),
    value: `${measurement.value} ${measurement.unit || 'cm'}`,
  })),
)

const countdownStyle = (value: number) => ({
  '--value': value,
})

const clamp = (value: number, minimum: number, maximum: number) =>
  Math.min(Math.max(value, minimum), maximum)

const clampModalPosition = (
  position: Point,
  zoom = modalZoom.value,
): Point => {
  const stage = galleryStage.value
  const image = galleryImage.value

  if (!stage || !image || zoom <= MIN_MODAL_ZOOM) {
    return { x: 0, y: 0 }
  }

  const imageWidth = image.offsetWidth
  const imageHeight = image.offsetHeight
  const stageWidth = stage.clientWidth
  const stageHeight = stage.clientHeight

  const maxX = Math.max(0, (imageWidth * zoom - stageWidth) / 2)
  const maxY = Math.max(0, (imageHeight * zoom - stageHeight) / 2)

  return {
    x: clamp(position.x, -maxX, maxX),
    y: clamp(position.y, -maxY, maxY),
  }
}

const hideMagnifier = () => {
  isMagnifierVisible.value = false
}

const resetModalView = (enableMagnifier = true) => {
  modalZoom.value = MIN_MODAL_ZOOM
  modalPosition.value = { x: 0, y: 0 }
  isDraggingImage.value = false
  isMagnifierEnabled.value = enableMagnifier
  hideMagnifier()
}

const preloadNeighbouringImages = () => {
  if (!lot.value?.media?.length || typeof Image === 'undefined') return

  const total = lot.value.media.length
  const indexes = [
    (selectedImageIndex.value + 1) % total,
    selectedImageIndex.value === 0
      ? total - 1
      : selectedImageIndex.value - 1,
  ]

  indexes.forEach((index) => {
    const url = lot.value?.media?.[index]?.url

    if (!url) return

    const image = new Image()
    image.src = url
  })
}

const selectImage = (index: number) => {
  if (!imageCount.value) return

  selectedImageIndex.value = clamp(index, 0, imageCount.value - 1)
  resetModalView(true)

  nextTick(preloadNeighbouringImages)
}

const openGallery = async (index = selectedImageIndex.value) => {
  if (isGalleryOpen.value || !selectedImage.value || !imageCount.value) return

  selectedImageIndex.value = clamp(index, 0, imageCount.value - 1)
  resetModalView(true)

  previouslyFocusedElement = document.activeElement as HTMLElement | null
  previousBodyOverflow = document.body.style.overflow
  document.body.style.overflow = 'hidden'

  isGalleryOpen.value = true

  await nextTick()
  galleryRoot.value?.focus()
  preloadNeighbouringImages()
}

const closeGallery = () => {
  isGalleryOpen.value = false
  resetModalView(true)
  document.body.style.overflow = previousBodyOverflow
  previouslyFocusedElement?.focus()
  previouslyFocusedElement = null
}

const nextImage = () => {
  if (!imageCount.value) return

  selectImage((selectedImageIndex.value + 1) % imageCount.value)
}

const previousImage = () => {
  if (!imageCount.value) return

  selectImage(
    selectedImageIndex.value === 0
      ? imageCount.value - 1
      : selectedImageIndex.value - 1,
  )
}

const setModalZoom = (
  requestedZoom: number,
  focalClientPoint?: Point,
) => {
  const nextZoom = clamp(
    requestedZoom,
    MIN_MODAL_ZOOM,
    MAX_MODAL_ZOOM,
  )
  const previousZoom = modalZoom.value

  if (nextZoom === previousZoom) return

  if (nextZoom === MIN_MODAL_ZOOM) {
    modalZoom.value = MIN_MODAL_ZOOM
    modalPosition.value = { x: 0, y: 0 }
    isDraggingImage.value = false
    isMagnifierEnabled.value = true
    hideMagnifier()
    return
  }

  const stage = galleryStage.value

  if (!stage) {
    modalZoom.value = nextZoom
    isMagnifierEnabled.value = false
    hideMagnifier()
    return
  }

  const stageRect = stage.getBoundingClientRect()
  const stageCenter = {
    x: stageRect.width / 2,
    y: stageRect.height / 2,
  }

  const focalPoint = focalClientPoint
    ? {
        x: focalClientPoint.x - stageRect.left,
        y: focalClientPoint.y - stageRect.top,
      }
    : stageCenter

  const scaleRatio = nextZoom / previousZoom

  const nextPosition = {
    x:
      focalPoint.x -
      stageCenter.x -
      (focalPoint.x - stageCenter.x - modalPosition.value.x) *
        scaleRatio,
    y:
      focalPoint.y -
      stageCenter.y -
      (focalPoint.y - stageCenter.y - modalPosition.value.y) *
        scaleRatio,
  }

  modalZoom.value = nextZoom
  modalPosition.value = clampModalPosition(nextPosition, nextZoom)
  isMagnifierEnabled.value = false
  hideMagnifier()
}

const zoomIn = () => {
  setModalZoom(modalZoom.value + MODAL_ZOOM_STEP)
}

const zoomOut = () => {
  setModalZoom(modalZoom.value - MODAL_ZOOM_STEP)
}

const handleModalWheel = (event: WheelEvent) => {
  const direction = event.deltaY < 0 ? 1 : -1

  setModalZoom(
    modalZoom.value + direction * MODAL_ZOOM_STEP,
    { x: event.clientX, y: event.clientY },
  )
}

const toggleModalZoom = (event: MouseEvent) => {
  if (modalZoom.value > MIN_MODAL_ZOOM) {
    resetModalView(true)
    return
  }

  setModalZoom(2.5, { x: event.clientX, y: event.clientY })
}

const toggleMagnifier = () => {
  if (!isMagnifierEnabled.value) {
    resetModalView(true)
    return
  }

  isMagnifierEnabled.value = false
  hideMagnifier()
}

const isPointerInsideImage = (event: PointerEvent) => {
  const image = galleryImage.value

  if (!image) return false

  const rect = image.getBoundingClientRect()

  return (
    event.clientX >= rect.left &&
    event.clientX <= rect.right &&
    event.clientY >= rect.top &&
    event.clientY <= rect.bottom
  )
}

const updateMagnifier = (event: PointerEvent) => {
  const eventTarget = event.target as HTMLElement

  if (
    eventTarget.closest('button') ||
    !isMagnifierEnabled.value ||
    modalZoom.value > MIN_MODAL_ZOOM ||
    event.pointerType !== 'mouse' ||
    !selectedImage.value
  ) {
    hideMagnifier()
    return
  }

  const stage = galleryStage.value
  const image = galleryImage.value

  if (!stage || !image) {
    hideMagnifier()
    return
  }

  const stageRect = stage.getBoundingClientRect()
  const imageRect = image.getBoundingClientRect()

  const isInsideImage =
    event.clientX >= imageRect.left &&
    event.clientX <= imageRect.right &&
    event.clientY >= imageRect.top &&
    event.clientY <= imageRect.bottom

  if (!isInsideImage) {
    hideMagnifier()
    return
  }

  const lensSize = Math.min(
    MAGNIFIER_SIZE,
    imageRect.width,
    imageRect.height,
  )

  if (lensSize < 80) {
    hideMagnifier()
    return
  }

  const imageLeftInStage = imageRect.left - stageRect.left
  const imageTopInStage = imageRect.top - stageRect.top
  const imageRightInStage = imageRect.right - stageRect.left
  const imageBottomInStage = imageRect.bottom - stageRect.top

  const left = clamp(
    event.clientX - stageRect.left - lensSize / 2,
    imageLeftInStage,
    imageRightInStage - lensSize,
  )

  const top = clamp(
    event.clientY - stageRect.top - lensSize / 2,
    imageTopInStage,
    imageBottomInStage - lensSize,
  )

  const pointerXInsideLens = event.clientX - (stageRect.left + left)
  const pointerYInsideLens = event.clientY - (stageRect.top + top)
  const sourceX = event.clientX - imageRect.left
  const sourceY = event.clientY - imageRect.top

  const backgroundX =
    pointerXInsideLens - sourceX * MAGNIFIER_ZOOM
  const backgroundY =
    pointerYInsideLens - sourceY * MAGNIFIER_ZOOM

  magnifierStyle.value = {
    left: `${left}px`,
    top: `${top}px`,
    width: `${lensSize}px`,
    height: `${lensSize}px`,
    backgroundImage: `url(${JSON.stringify(selectedImage.value)})`,
    backgroundSize: `${imageRect.width * MAGNIFIER_ZOOM}px ${
      imageRect.height * MAGNIFIER_ZOOM
    }px`,
    backgroundPosition: `${backgroundX}px ${backgroundY}px`,
    backgroundRepeat: 'no-repeat',
  }

  isMagnifierVisible.value = true
}

const handleStagePointerDown = (event: PointerEvent) => {
  const eventTarget = event.target as HTMLElement

  if (
    eventTarget.closest('button') ||
    modalZoom.value <= MIN_MODAL_ZOOM ||
    event.button !== 0 ||
    !isPointerInsideImage(event)
  ) {
    return
  }

  const target = event.currentTarget as HTMLElement
  target.setPointerCapture(event.pointerId)

  isDraggingImage.value = true
  dragStartPointer.value = {
    x: event.clientX,
    y: event.clientY,
  }
  dragStartPosition.value = { ...modalPosition.value }

  event.preventDefault()
}

const handleStagePointerMove = (event: PointerEvent) => {
  if (isDraggingImage.value) {
    modalPosition.value = clampModalPosition({
      x:
        dragStartPosition.value.x +
        event.clientX -
        dragStartPointer.value.x,
      y:
        dragStartPosition.value.y +
        event.clientY -
        dragStartPointer.value.y,
    })

    return
  }

  updateMagnifier(event)
}

const stopImageDrag = (event?: PointerEvent) => {
  if (event) {
    const target = event.currentTarget as HTMLElement

    if (target.hasPointerCapture(event.pointerId)) {
      target.releasePointerCapture(event.pointerId)
    }
  }

  isDraggingImage.value = false
}

const handleModalImageLoad = () => {
  modalPosition.value = clampModalPosition(modalPosition.value)
  hideMagnifier()
}

const handleWindowResize = () => {
  hideMagnifier()

  nextTick(() => {
    modalPosition.value = clampModalPosition(modalPosition.value)
  })
}

const handleGalleryKeydown = (event: KeyboardEvent) => {
  if (!isGalleryOpen.value) return

  if (event.key === 'Escape') {
    event.preventDefault()
    closeGallery()
    return
  }

  if (event.key === 'ArrowRight') {
    event.preventDefault()
    nextImage()
    return
  }

  if (event.key === 'ArrowLeft') {
    event.preventDefault()
    previousImage()
    return
  }

  if (event.key === '+' || event.key === '=') {
    event.preventDefault()
    zoomIn()
    return
  }

  if (event.key === '-') {
    event.preventDefault()
    zoomOut()
    return
  }

  if (event.key === '0') {
    event.preventDefault()
    resetModalView(true)
    return
  }

  if (event.key.toLowerCase() === 'm') {
    event.preventDefault()
    toggleMagnifier()
  }
}

const isNotFoundError = (error: unknown) =>
  typeof error === 'object' &&
  error !== null &&
  (error as { response?: { status?: number } }).response?.status === 404

const getApiErrorMessage = (error: unknown, fallback: string): string => {
  if (typeof error !== 'object' || error === null) {
    return fallback
  }

  const response = (error as { response?: { data?: { message?: unknown } } }).response
  const message = response?.data?.message

  return typeof message === 'string'
    ? message
    : fallback
}

const unsubscribeRealtimeHandlers = () => {
  unsubscribeBidPlaced?.()
  unsubscribeAuctionEnded?.()
  unsubscribeAuctionStarted?.()

  unsubscribeBidPlaced = null
  unsubscribeAuctionEnded = null
  unsubscribeAuctionStarted = null
}

const subscribeRealtimeHandlers = () => {
  if (unsubscribeBidPlaced || unsubscribeAuctionEnded || unsubscribeAuctionStarted) {
    return
  }

  unsubscribeBidPlaced = auctionRealtimeService.on(
    'BidPlaced',
    handleRealtimeAuctionUpdate,
  )
  unsubscribeAuctionEnded = auctionRealtimeService.on(
    'AuctionEnded',
    handleRealtimeAuctionUpdate,
  )
  unsubscribeAuctionStarted = auctionRealtimeService.on(
    'AuctionStarted',
    handleRealtimeAuctionUpdate,
  )
}

const isCurrentAuctionEvent = (
  event: BidPlacedEvent | AuctionEndedEvent | AuctionStartedEvent,
) => {
  if ('lotId' in event) {
    return (
      event.auctionId === auction.value?.auctionId ||
      event.lotId === lot.value?.id
    )
  }

  return event.auctionId === auction.value?.auctionId
}

const refreshAuctionFromRealtime = async () => {
  if (realtimeRefreshPromise) {
    realtimeRefreshQueued = true
    await realtimeRefreshPromise
    return
  }

  do {
    realtimeRefreshQueued = false
    realtimeRefreshPromise = loadAuctionAndBids(false)

    try {
      await realtimeRefreshPromise
    } finally {
      realtimeRefreshPromise = null
    }
  } while (realtimeRefreshQueued)
}

const handleRealtimeAuctionUpdate = async (
  event: BidPlacedEvent | AuctionEndedEvent | AuctionStartedEvent,
) => {
  if (!isCurrentAuctionEvent(event)) {
    return
  }

  try {
    await refreshAuctionFromRealtime()
  } catch (error) {
    console.error('Failed to handle realtime auction update:', error)
  }
}

const leaveAuctionRealtime = async () => {
  const auctionId = subscribedAuctionId

  subscribedAuctionId = null

  if (!auctionId) return

  try {
    await auctionRealtimeService.leaveAuction(auctionId)
  } catch (error) {
    console.error('Failed to leave auction realtime group:', error)
  }
}

const joinAuctionRealtime = async (auctionId: string) => {
  if (subscribedAuctionId === auctionId) {
    return
  }

  await leaveAuctionRealtime()
  subscribeRealtimeHandlers()

  try {
    await auctionRealtimeService.joinAuction(auctionId)
    subscribedAuctionId = auctionId
  } catch (error) {
    console.error('Failed to join auction realtime group:', error)
  }
}

const leaveLotRealtime = async () => {
  const lotId = subscribedLotId

  subscribedLotId = null

  if (!lotId) return

  try {
    await auctionRealtimeService.leaveLot(lotId)
  } catch (error) {
    console.error('Failed to leave lot realtime group:', error)
  }
}

const joinLotRealtime = async (lotId: string) => {
  if (subscribedLotId === lotId) {
    return
  }

  await leaveLotRealtime()
  subscribeRealtimeHandlers()

  try {
    await auctionRealtimeService.joinLot(lotId)
    subscribedLotId = lotId
  } catch (error) {
    console.error('Failed to join lot realtime group:', error)
  }
}

const leaveAllRealtime = async () => {
  await Promise.all([
    leaveAuctionRealtime(),
    leaveLotRealtime(),
  ])
  unsubscribeRealtimeHandlers()
}

const loadAuctionAndBids = async (showLoading = true) => {
  if (!lot.value?.id) return

  if (showLoading) {
    isBidLoading.value = true
  }

  try {
    try {
      const nextAuction = await auctionService.getAuctionByLotId(lot.value.id)
      auction.value = nextAuction
    } catch (error) {
      if (!isNotFoundError(error)) {
        throw error
      }

      auction.value = null
      bids.value = []
      await leaveAuctionRealtime()
      return
    }

    const auctionId = auction.value?.auctionId

    if (!auctionId) {
      bids.value = []
      await leaveAuctionRealtime()
      return
    }

    await joinAuctionRealtime(auctionId)
    bids.value = await bidService.getBidsByAuction(auctionId)
  } finally {
    if (showLoading) {
      isBidLoading.value = false
    }
  }
}

const loadLot = async () => {
  const requestId = ++latestLotRequestId

  try {
    isLoading.value = true
    errorMessage.value = ''
    auction.value = null
    bids.value = []
    await leaveAllRealtime()

    const nextLot = await itemService.getLot(lotIdParam.value)

    if (requestId !== latestLotRequestId) {
      return
    }

    lot.value = nextLot
    selectedImageIndex.value = 0

    if (!canSeeLot.value) return

    await joinLotRealtime(nextLot.id)
    await loadAuctionAndBids()
  } catch (error) {
    if (requestId !== latestLotRequestId) {
      return
    }

    console.error('Error loading lot information:', error)
    errorMessage.value = t('lot.failedLoad')
  } finally {
    if (requestId === latestLotRequestId) {
      isLoading.value = false
    }
  }
}

const submitBid = async () => {
  bidError.value = ''
  bidSuccess.value = ''

  if (!canBid.value || !auction.value?.auctionId || !lot.value) {
    bidError.value = t('lot.biddingUnavailable')
    return
  }

  if (
    bidAmount.value === null ||
    Number.isNaN(bidAmount.value) ||
    bidAmount.value <= 0
  ) {
    bidError.value = t('lot.validBidRequired')
    return
  }

  try {
    isBidSubmitting.value = true

    await bidService.placeBid(auction.value.auctionId, {
      amount: bidAmount.value,
      currency: lot.value.currency,
    })

    bidSuccess.value = t('lot.bidSuccess')
    bidAmount.value = null

    await loadAuctionAndBids()
  } catch (error: unknown) {
    console.error('Place bid failed:', error)

    bidError.value = getApiErrorMessage(error, t('lot.bidFailed'))
  } finally {
    isBidSubmitting.value = false
  }
}

watch(
  lotIdParam,
  () => {
    void loadLot()
  },
  {
    immediate: true,
  },
)

watch(
  [visibleLotTitle, locale],
  ([title]) => {
    if (title) {
      setPageTitle(title)
    }
  },
  {
    flush: 'post',
  },
)

onMounted(() => {
  window.addEventListener('keydown', handleGalleryKeydown)
  window.addEventListener('resize', handleWindowResize)

  timer = window.setInterval(() => {
    currentTime.value = Date.now()
  }, 1000)
})

onBeforeUnmount(() => {
  if (timer) {
    clearInterval(timer)
  }

  window.removeEventListener('keydown', handleGalleryKeydown)
  window.removeEventListener('resize', handleWindowResize)

  void leaveAllRealtime().finally(() => {
    void auctionRealtimeService.stop()
  })

  document.body.style.overflow = previousBodyOverflow
})
</script>

<template>
  <div class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
    <div
      v-if="isLoading"
      class="rounded-3xl border bg-background p-8 text-sm text-muted-foreground"
    >
      {{ $t('lot.loadingListing') }}
    </div>

    <div
      v-else-if="errorMessage"
      class="rounded-3xl border border-error/30 bg-error/10 p-8 text-sm text-error"
    >
      {{ errorMessage }}
    </div>

    <div
      v-else-if="lot && !canSeeLot"
      class="rounded-3xl border bg-background p-8 text-sm text-muted-foreground"
    >
      {{ $t('lot.unavailableListing') }}
    </div>

    <div v-else-if="lot" class="space-y-8">
      <div class="grid gap-8 lg:grid-cols-[1.15fr_0.85fr]">
        <section class="min-w-0 space-y-4">
          <button
            type="button"
            class="group relative block h-[420px] w-full overflow-hidden rounded-3xl border bg-background text-left shadow-sm outline-none transition hover:border-foreground/20 focus-visible:ring-2 focus-visible:ring-ring sm:h-[520px]"
            :disabled="!selectedImage"
            :aria-label="$t('lot.openGallery')"
            @click="openGallery()"
          >
            <img
              v-if="selectedImage"
              :src="selectedImage"
              :alt="lot.title"
              draggable="false"
              class="h-full w-full select-none object-contain transition duration-300 group-hover:scale-[1.01]"
            />

            <div
              v-else
              class="flex h-full items-center justify-center text-sm text-muted-foreground"
            >
              {{ $t('lot.noImageAvailable') }}
            </div>

            <div
              v-if="selectedImage"
              class="pointer-events-none absolute inset-0 bg-gradient-to-t from-black/20 via-transparent to-transparent opacity-0 transition group-hover:opacity-100"
            />

            <div
              v-if="selectedImage"
              class="pointer-events-none absolute bottom-4 right-4 flex items-center gap-2 rounded-full bg-black/70 px-4 py-2 text-xs font-medium text-white opacity-90 shadow-lg backdrop-blur transition group-hover:bg-black/80"
            >
              <svg
                aria-hidden="true"
                class="h-4 w-4"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="1.8"
              >
                <circle cx="11" cy="11" r="7" />
                <path d="m20 20-3.2-3.2M11 8v6M8 11h6" />
              </svg>
              {{ $t('lot.inspectDetails') }}
            </div>
          </button>

          <div
            v-if="lot.media?.length"
            class="flex gap-3 overflow-x-auto pb-2"
          >
            <button
              v-for="(image, index) in lot.media"
              :key="image.key ?? index"
              type="button"
              class="h-24 w-24 shrink-0 overflow-hidden rounded-2xl border bg-background transition"
              :class="
                selectedImageIndex === index
                  ? 'border-primary ring-2 ring-primary/30'
                  : 'border-border opacity-70 hover:opacity-100'
              "
              :aria-label="$t('lot.selectImage', { number: index + 1 })"
              @click="selectImage(index)"
            >
              <img
                :src="image.url"
                :alt="`${lot.title} ${index + 1}`"
                class="h-full w-full object-cover"
              />
            </button>
          </div>
        </section>

        <section class="space-y-5">
          <div class="rounded-3xl border bg-background p-6 shadow-sm">
            <div class="flex flex-wrap items-start justify-between gap-3">
              <div>
                <p
                  class="text-xs uppercase tracking-[0.25em] text-muted-foreground"
                >
                  {{ lot.brand }}
                </p>

                <h1
                  class="mt-2 text-3xl font-semibold leading-tight text-foreground"
                >
                  {{ lot.title }}
                </h1>
              </div>
            </div>

            <div class="mt-6 rounded-2xl bg-muted p-5">
              <p class="text-sm text-muted-foreground">{{ $t('lot.currentPrice') }}</p>

              <p class="mt-1 text-4xl font-semibold text-foreground">
                {{
                  formatMoney(
                    auction?.currentPrice ?? lot.price,
                    auction?.currency ?? lot.currency,
                  )
                }}
              </p>
            </div>

            <div class="mt-5 grid gap-3 sm:grid-cols-2">
              <div class="rounded-2xl border bg-background p-4">
                <p class="text-xs text-muted-foreground">{{ $t('common.shipping') }}</p>
                <p class="mt-1 font-medium">
                  {{ formatMoney(lot.shippingPrice ?? 0, lot.shippingCurrency ?? lot.currency) }}
                </p>
              </div>

              <div class="rounded-2xl border bg-background p-4">
                <p class="text-xs text-muted-foreground">{{ $t('common.dispatch') }}</p>
                <p class="mt-1 font-medium">
                  {{ $t('lot.dispatchDays', { count: lot.shippingHandlingDays ?? 3 }) }}
                </p>
              </div>

              <div class="rounded-2xl border bg-background p-4">
                <p class="text-xs text-muted-foreground">{{ $t('common.shipsFrom') }}</p>
                <p class="mt-1 font-medium">
                  {{ lot.shippingOriginCountry || lot.country }}
                </p>
              </div>

              <div class="rounded-2xl border bg-background p-4">
                <p class="text-xs text-muted-foreground">{{ $t('common.shipsTo') }}</p>
                <p class="mt-1 font-medium">
                  {{ lot.shipsToCountries || lot.country }}
                </p>
              </div>
            </div>

            <div
              v-if="auction?.endsAt"
              class="mt-5 rounded-2xl border bg-background p-5"
            >
              <div class="mb-3 flex items-center justify-between gap-3">
                <h2 class="font-semibold">
                  {{ isAuctionEnded ? $t('lot.auctionEnded') : $t('lot.auctionEndsIn') }}
                </h2>

                <span class="text-xs text-muted-foreground">
                  {{ $t('lot.endsAt', { date: formatDate(auction.endsAt) }) }}
                </span>
              </div>

              <div class="flex flex-wrap gap-3">
                <div class="rounded-2xl bg-muted px-4 py-3 text-center">
                  <span class="countdown text-3xl font-bold">
                    <span :style="countdownStyle(auctionTimeLeft.days)" />
                  </span>
                  <p class="mt-1 text-xs text-muted-foreground">{{ $t('common.days') }}</p>
                </div>

                <div class="rounded-2xl bg-muted px-4 py-3 text-center">
                  <span class="countdown text-3xl font-bold">
                    <span :style="countdownStyle(auctionTimeLeft.hours)" />
                  </span>
                  <p class="mt-1 text-xs text-muted-foreground">{{ $t('common.hours') }}</p>
                </div>

                <div class="rounded-2xl bg-muted px-4 py-3 text-center">
                  <span class="countdown text-3xl font-bold">
                    <span :style="countdownStyle(auctionTimeLeft.minutes)" />
                  </span>
                  <p class="mt-1 text-xs text-muted-foreground">{{ $t('common.minutesShort') }}</p>
                </div>

                <div class="rounded-2xl bg-muted px-4 py-3 text-center">
                  <span class="countdown text-3xl font-bold">
                    <span :style="countdownStyle(auctionTimeLeft.seconds)" />
                  </span>
                  <p class="mt-1 text-xs text-muted-foreground">{{ $t('common.secondsShort') }}</p>
                </div>
              </div>
            </div>
          </div>

          <div class="rounded-3xl border bg-background p-6 shadow-sm">
            <h2 class="text-lg font-semibold">{{ $t('lot.placeBid') }}</h2>

            <div v-if="bidSuccess" class="alert alert-success mt-4 text-sm">
              {{ bidSuccess }}
            </div>

            <div v-if="bidError" class="alert alert-error mt-4 text-sm">
              {{ bidError }}
            </div>

            <div class="mt-4 flex gap-3">
              <input
                v-model.number="bidAmount"
                type="number"
                step="0.01"
                min="0"
                class="h-12 w-full rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
                :placeholder="$t('lot.enterBidAmount')"
                :disabled="!canBid || isBidSubmitting"
              />

              <button
                type="button"
                class="inline-flex h-12 items-center justify-center rounded-md bg-primary px-5 text-sm font-medium text-primary-foreground transition hover:bg-primary/90 disabled:pointer-events-none disabled:opacity-50"
                :disabled="
                  !canBid ||
                  isBidSubmitting ||
                  !bidAmount ||
                  bidAmount <= 0
                "
                @click="submitBid"
              >
                {{ isBidSubmitting ? $t('lot.placingBid') : $t('lot.placeBid') }}
              </button>
            </div>

            <p
              v-if="!isAuthenticated"
              class="mt-3 text-xs text-muted-foreground"
            >
              {{ $t('lot.signInToBid') }}
            </p>

            <p
              v-else-if="auction && !isAuctionActive"
              class="mt-3 text-xs text-muted-foreground"
            >
              {{ $t('lot.bidActiveOnly') }}
            </p>
          </div>
        </section>
      </div>

      <div class="grid gap-6 lg:grid-cols-[1fr_360px]">
        <section class="space-y-6">
          <div class="rounded-3xl border bg-background p-6 shadow-sm">
            <h2 class="text-xl font-semibold">{{ $t('lot.description') }}</h2>

            <p
              class="mt-4 whitespace-pre-line leading-7 text-muted-foreground"
            >
              {{ lot.description }}
            </p>
          </div>

          <div class="rounded-3xl border bg-background p-6 shadow-sm">
            <h2 class="text-xl font-semibold">{{ $t('lot.itemDetails') }}</h2>

            <nav
              v-if="categoryBreadcrumb.length"
              class="mt-4 flex flex-wrap gap-2 text-sm text-muted-foreground"
              :aria-label="$t('catalog.category')"
            >
              <span
                v-for="(part, index) in categoryBreadcrumb"
                :key="`${part}-${index}`"
                class="inline-flex items-center gap-2"
              >
                <span>{{ part }}</span>
                <span
                  v-if="index < categoryBreadcrumb.length - 1"
                  aria-hidden="true"
                >/</span>
              </span>
            </nav>

            <div class="mt-5 grid gap-3 sm:grid-cols-2">
              <div
                v-for="row in detailRows"
                :key="row.label"
                class="rounded-2xl bg-muted p-4"
              >
                <p class="text-xs text-muted-foreground">
                  {{ row.label }}
                </p>
                <p class="font-medium">
                  {{ row.value }}
                </p>
              </div>
            </div>

            <div
              v-if="measurementRows.length"
              class="mt-6 rounded-2xl border bg-background p-4"
            >
              <div class="flex flex-wrap items-baseline justify-between gap-2">
                <h3 class="font-semibold">
                  {{ $t('lot.measurements') }}
                </h3>

                <p class="text-xs text-muted-foreground">
                  {{ $t('lot.measurementsSellerProvided') }}
                </p>
              </div>

              <dl class="mt-4 grid gap-3 sm:grid-cols-2">
                <div
                  v-for="measurement in measurementRows"
                  :key="measurement.key"
                  class="flex items-center justify-between gap-4 rounded-xl bg-muted px-3 py-2 text-sm"
                >
                  <dt class="text-muted-foreground">
                    {{ measurement.label }}
                  </dt>
                  <dd class="font-medium">
                    {{ measurement.value }}
                  </dd>
                </div>
              </dl>
            </div>
          </div>
        </section>

        <aside class="space-y-6">
          <div class="rounded-3xl border bg-background p-6 shadow-sm">
            <h2 class="text-xl font-semibold">{{ $t('common.seller') }}</h2>

            <div class="mt-5 flex items-center gap-4">
              <div class="avatar placeholder">
                <div
                  class="w-14 rounded-full bg-foreground text-background"
                >
                  <span class="text-lg">
                    {{ lot.seller?.name?.[0]?.toUpperCase() ?? 'S' }}
                  </span>
                </div>
              </div>

              <div>
                <p class="font-semibold">
                  {{ lot.seller?.name ?? $t('lot.unknownSeller') }}
                </p>

                <p class="text-sm text-muted-foreground">
                  @{{ lot.seller?.username ?? $t('lot.sellerUsernameFallback') }}
                </p>
              </div>
            </div>
          </div>

          <div class="rounded-3xl border bg-background p-6 shadow-sm">
            <h2 class="text-xl font-semibold">{{ $t('lot.bidHistory') }}</h2>

            <div class="mt-4 flex justify-between rounded-2xl bg-muted p-3">
              <span class="text-muted-foreground">{{ $t('lot.totalBids') }}</span>
              <span class="font-medium">{{ sortedBids.length }}</span>
            </div>

            <p
              v-if="isBidLoading"
              class="mt-3 text-sm text-muted-foreground"
            >
              {{ $t('lot.loadingBids') }}
            </p>

            <p
              v-else-if="sortedBids.length === 0"
              class="mt-3 text-sm text-muted-foreground"
            >
              {{ $t('lot.noBidsYet') }}
            </p>

            <ul v-else class="mt-4 space-y-3">
              <li
                v-for="bid in sortedBids"
                :key="bid.bidId"
                class="flex items-center justify-between rounded-2xl border bg-background p-4 text-sm"
              >
                <span class="font-semibold">
                  {{ formatMoney(bid.amount, bid.currency) }}
                </span>

                <span class="text-xs text-muted-foreground">
                  {{ formatDate(bid.placedAt) }}
                </span>
              </li>
            </ul>
          </div>
        </aside>
      </div>
    </div>

    <Teleport to="body">
      <Transition
        enter-active-class="transition duration-200 ease-out"
        enter-from-class="opacity-0"
        enter-to-class="opacity-100"
        leave-active-class="transition duration-150 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
      >
        <div
          v-if="isGalleryOpen && lot"
          ref="galleryRoot"
          tabindex="-1"
          role="dialog"
          aria-modal="true"
          :aria-label="$t('lot.lotGallery')"
          class="fixed inset-0 z-[200] flex h-dvh w-screen flex-col overflow-hidden bg-black text-white outline-none"
        >
          <header
            class="relative z-40 flex h-16 shrink-0 items-center justify-between border-b border-white/10 bg-black/90 px-4 backdrop-blur-xl sm:px-6"
          >
            <div class="min-w-0 pr-4">
              <p class="truncate text-sm font-medium sm:text-base">
                {{ lot.title }}
              </p>

              <p class="mt-0.5 text-xs text-white/55">
                {{ selectedImageIndex + 1 }} / {{ imageCount }}
              </p>
            </div>

            <div class="flex shrink-0 items-center gap-1.5 sm:gap-2">
              <button
                type="button"
                class="hidden h-10 items-center gap-2 rounded-full border px-3 text-sm transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white/70 sm:flex"
                :class="
                  isMagnifierEnabled && modalZoom === MIN_MODAL_ZOOM
                    ? 'border-white bg-white text-black'
                    : 'border-white/15 bg-white/5 text-white hover:bg-white/10'
                "
                :aria-pressed="isMagnifierEnabled"
                :title="$t('lot.toggleMagnifier')"
                @click="toggleMagnifier"
              >
                <svg
                  aria-hidden="true"
                  class="h-4 w-4"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="1.8"
                >
                  <circle cx="11" cy="11" r="7" />
                  <path d="m20 20-3.2-3.2M11 8v6M8 11h6" />
                </svg>
                {{ $t('lot.magnifier') }}
              </button>

              <button
                type="button"
                class="grid h-10 w-10 place-items-center rounded-full border border-white/15 bg-white/5 text-white transition hover:bg-white/10 disabled:cursor-not-allowed disabled:opacity-35"
                :disabled="modalZoom <= MIN_MODAL_ZOOM"
                :aria-label="$t('lot.zoomOut')"
                :title="$t('lot.zoomOut')"
                @click="zoomOut"
              >
                <svg
                  aria-hidden="true"
                  class="h-4 w-4"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                >
                  <path d="M5 12h14" />
                </svg>
              </button>

              <button
                type="button"
                class="h-10 min-w-16 rounded-full border border-white/15 bg-white/5 px-3 text-sm tabular-nums text-white transition hover:bg-white/10"
                :aria-label="$t('lot.resetZoom')"
                :title="$t('lot.resetView')"
                @click="resetModalView(true)"
              >
                {{ Math.round(modalZoom * 100) }}%
              </button>

              <button
                type="button"
                class="grid h-10 w-10 place-items-center rounded-full border border-white/15 bg-white/5 text-white transition hover:bg-white/10 disabled:cursor-not-allowed disabled:opacity-35"
                :disabled="modalZoom >= MAX_MODAL_ZOOM"
                :aria-label="$t('lot.zoomIn')"
                :title="$t('lot.zoomIn')"
                @click="zoomIn"
              >
                <svg
                  aria-hidden="true"
                  class="h-4 w-4"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                >
                  <path d="M12 5v14M5 12h14" />
                </svg>
              </button>

              <div class="mx-1 hidden h-6 w-px bg-white/10 sm:block" />

              <button
                type="button"
                class="grid h-10 w-10 place-items-center rounded-full border border-white/15 bg-white/5 text-white transition hover:bg-white/10"
                :aria-label="$t('lot.closeGallery')"
                :title="$t('lot.closeEsc')"
                @click="closeGallery"
              >
                <svg
                  aria-hidden="true"
                  class="h-5 w-5"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="1.8"
                >
                  <path d="m6 6 12 12M18 6 6 18" />
                </svg>
              </button>
            </div>
          </header>

          <main
            ref="galleryStage"
            class="relative flex min-h-0 flex-1 touch-none select-none items-center justify-center overflow-hidden bg-black"
            @wheel.prevent="handleModalWheel"
            @dblclick="toggleModalZoom"
            @pointerdown="handleStagePointerDown"
            @pointermove="handleStagePointerMove"
            @pointerup="stopImageDrag"
            @pointercancel="stopImageDrag"
            @pointerleave="hideMagnifier"
          >
            <button
              v-if="hasMultipleImages"
              type="button"
              class="absolute left-3 z-30 grid h-11 w-11 place-items-center rounded-full border border-white/15 bg-black/55 text-white shadow-xl backdrop-blur transition hover:bg-black/80 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white/70 sm:left-6 sm:h-12 sm:w-12"
              :aria-label="$t('lot.previousImage')"
              @click.stop="previousImage"
            >
              <svg
                aria-hidden="true"
                class="h-5 w-5"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="1.8"
              >
                <path d="m15 18-6-6 6-6" />
              </svg>
            </button>

            <img
              v-if="selectedImage"
              ref="galleryImage"
              :src="selectedImage"
              :alt="lot.title"
              draggable="false"
              class="max-h-full max-w-full object-contain will-change-transform"
              :class="[
                isDraggingImage
                  ? 'cursor-grabbing transition-none'
                  : 'transition-transform duration-150 ease-out',
                modalZoom > MIN_MODAL_ZOOM && !isDraggingImage
                  ? 'cursor-grab'
                  : '',
                modalZoom === MIN_MODAL_ZOOM && isMagnifierEnabled
                  ? 'cursor-none'
                  : 'cursor-zoom-in',
              ]"
              :style="modalImageStyle"
              @load="handleModalImageLoad"
              @click.stop
            />

            <div
              v-if="isMagnifierVisible"
              class="pointer-events-none absolute z-20 overflow-hidden rounded-[2px] border border-white/95 bg-black/20 shadow-[0_18px_60px_rgba(0,0,0,0.55)] ring-1 ring-black/35"
              :style="magnifierStyle"
            >
              <span
                class="absolute right-2 top-2 rounded-full bg-black/65 px-2 py-1 text-[10px] font-semibold tracking-wide text-white shadow backdrop-blur"
              >
                {{ MAGNIFIER_ZOOM }}×
              </span>
            </div>

            <button
              v-if="hasMultipleImages"
              type="button"
              class="absolute right-3 z-30 grid h-11 w-11 place-items-center rounded-full border border-white/15 bg-black/55 text-white shadow-xl backdrop-blur transition hover:bg-black/80 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white/70 sm:right-6 sm:h-12 sm:w-12"
              :aria-label="$t('lot.nextImage')"
              @click.stop="nextImage"
            >
              <svg
                aria-hidden="true"
                class="h-5 w-5"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                stroke-width="1.8"
              >
                <path d="m9 18 6-6-6-6" />
              </svg>
            </button>

            <div
              class="pointer-events-none absolute bottom-4 left-1/2 hidden -translate-x-1/2 rounded-full border border-white/10 bg-black/60 px-4 py-2 text-xs text-white/70 shadow-lg backdrop-blur sm:block"
            >
              {{ galleryHint }}
            </div>
          </main>

          <footer
            v-if="hasMultipleImages"
            class="relative z-40 shrink-0 border-t border-white/10 bg-black/90 px-3 py-3 backdrop-blur-xl"
          >
            <div
              class="mx-auto flex max-w-full justify-start gap-2 overflow-x-auto px-1 sm:justify-center sm:gap-3"
            >
              <button
                v-for="(image, index) in lot.media"
                :key="image.key ?? index"
                type="button"
                class="relative h-16 w-16 shrink-0 overflow-hidden rounded-lg border-2 bg-white/5 transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-white/70 sm:h-20 sm:w-20"
                :class="
                  selectedImageIndex === index
                    ? 'border-white opacity-100'
                    : 'border-transparent opacity-45 hover:opacity-90'
                "
                :aria-label="$t('lot.openImage', { number: index + 1 })"
                :aria-current="selectedImageIndex === index ? 'true' : undefined"
                @click="selectImage(index)"
              >
                <img
                  :src="image.url"
                  :alt="`${lot.title} ${index + 1}`"
                  class="h-full w-full object-cover"
                />

                <span
                  v-if="selectedImageIndex === index"
                  class="pointer-events-none absolute inset-x-2 bottom-1 h-0.5 rounded-full bg-white"
                />
              </button>
            </div>
          </footer>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>
