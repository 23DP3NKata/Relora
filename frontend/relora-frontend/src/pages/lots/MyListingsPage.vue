<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import { storeToRefs } from 'pinia'
import { toast } from 'vue-sonner'
import { useI18n } from 'vue-i18n'

import { useAuthStore } from '@/stores/authStore'
import { itemService } from '@/app/services/lotService'
import { auctionService } from '@/app/services/auctionService'
import { buildMediaUrl } from '@/shared/mediaUrl'
import { useLocalePath } from '@/composables/useLocalePath'
import type { LotPreview } from '@/types/lot'
import type { AuctionListItem, StartAuctionDuration } from '@/types/auction'

import ConfirmDeleteModal from '@/components/modals/ConfirmDeleteModal.vue'

type ListingState =
  | 'draft'
  | 'review'
  | 'rejected'
  | 'ready'
  | 'live'
  | 'sold'
  | 'unsold'
  | 'expired'
  | 'unknown'

type ListingFilter = 'all' | 'attention' | 'live' | 'sold'
type ListingAction = 'delete' | 'publish' | 'auction'

type ListingRow = {
  lot: LotPreview
  auction?: AuctionListItem
  normalizedStatus: string
  auctionStatus: string
  state: ListingState
}

const authStore = useAuthStore()
const { isAuthenticated } = storeToRefs(authStore)
const { t, locale } = useI18n()
const localePath = useLocalePath()

const listings = ref<LotPreview[]>([])
const auctions = ref<AuctionListItem[]>([])
const selectedDurationByLot = ref<Record<string, StartAuctionDuration>>({})

const isAuthChecking = ref(true)
const isLoading = ref(true)
const errorMessage = ref('')
const searchQuery = ref('')
const activeFilter = ref<ListingFilter>('all')
const failedImageIds = ref<Set<string>>(new Set())

const isDeleteModalOpen = ref(false)
const lotToDeleteId = ref<string | null>(null)
const pendingAction = ref<{
  lotId: string
  action: ListingAction
} | null>(null)

const normalizeStatus = (lot: LotPreview) => {
  const raw = String(lot.statusName ?? lot.status ?? '').toLowerCase()

  if (raw.includes('draft')) return 'draft'
  if (raw.includes('pend')) return 'pending'
  if (raw.includes('reject')) return 'rejected'
  if (raw.includes('active')) return 'active'
  if (raw.includes('approve') || raw.includes('publish')) return 'approved'
  if (raw.includes('unsold')) return 'unsold'
  if (raw.includes('sold')) return 'sold'
  if (raw.includes('expir')) return 'expired'

  return 'unknown'
}

const normalizeAuctionStatus = (auction?: AuctionListItem) =>
  String(auction?.status ?? '').toLowerCase()

const getAuctionByLotId = (lotId: string) =>
  auctions.value.find((auction) => String(auction.lotId) === String(lotId))

const resolveListingState = (
  normalizedStatus: string,
  auctionStatus: string,
): ListingState => {
  if (auctionStatus.includes('unsold')) {
    return 'unsold'
  }
  if (auctionStatus.includes('sold') || normalizedStatus === 'sold') {
    return 'sold'
  }
  if (auctionStatus.includes('active') || auctionStatus.includes('live')) {
    return 'live'
  }
  if (auctionStatus.includes('expir') || normalizedStatus === 'expired') {
    return 'expired'
  }
  if (normalizedStatus === 'pending') return 'review'
  if (normalizedStatus === 'rejected') return 'rejected'
  if (normalizedStatus === 'draft') return 'draft'
  if (normalizedStatus === 'unsold') return 'unsold'
  if (normalizedStatus === 'approved' || normalizedStatus === 'active') {
    return 'ready'
  }
  return 'unknown'
}

const listingRows = computed<ListingRow[]>(() =>
  listings.value
    .map((lot) => {
      const auction = getAuctionByLotId(lot.id)
      const normalizedStatus = normalizeStatus(lot)
      const auctionStatus = normalizeAuctionStatus(auction)

      return {
        lot,
        auction,
        normalizedStatus,
        auctionStatus,
        state: resolveListingState(normalizedStatus, auctionStatus),
      }
    })
    .sort((left, right) => {
      const leftDate = left.lot.createdAt
        ? new Date(left.lot.createdAt).getTime()
        : 0
      const rightDate = right.lot.createdAt
        ? new Date(right.lot.createdAt).getTime()
        : 0
      return rightDate - leftDate
    }),
)

const needsAttention = (state: ListingState) =>
  ['draft', 'rejected', 'ready', 'unsold', 'expired'].includes(state)

const filters = computed(() => [
  {
    id: 'all' as const,
    label: t('listings.filters.all'),
    count: listingRows.value.length,
  },
  {
    id: 'attention' as const,
    label: t('listings.filters.attention'),
    count: listingRows.value.filter((row) => needsAttention(row.state)).length,
  },
  {
    id: 'live' as const,
    label: t('listings.filters.live'),
    count: listingRows.value.filter((row) => row.state === 'live').length,
  },
  {
    id: 'sold' as const,
    label: t('listings.filters.sold'),
    count: listingRows.value.filter((row) => row.state === 'sold').length,
  },
])

const visibleRows = computed(() => {
  const query = searchQuery.value.trim().toLowerCase()

  return listingRows.value.filter((row) => {
    const matchesFilter =
      activeFilter.value === 'all' ||
      (activeFilter.value === 'attention' && needsAttention(row.state)) ||
      (activeFilter.value === 'live' && row.state === 'live') ||
      (activeFilter.value === 'sold' && row.state === 'sold')

    if (!matchesFilter) return false
    if (!query) return true

    return [row.lot.title, row.lot.brand, row.lot.id]
      .filter(Boolean)
      .some((value) => String(value).toLowerCase().includes(query))
  })
})

const statusLabel = (state: ListingState) => {
  return t(`listings.status.${state}`)
}

const statusBadgeClass = (state: ListingState) => {
  if (state === 'live') {
    return 'border-emerald-500/30 bg-emerald-500/10 text-emerald-700 dark:text-emerald-300'
  }
  if (state === 'ready') {
    return 'border-blue-500/30 bg-blue-500/10 text-blue-700 dark:text-blue-300'
  }
  if (state === 'review') {
    return 'border-amber-500/30 bg-amber-500/10 text-amber-700 dark:text-amber-300'
  }
  if (state === 'rejected') {
    return 'border-red-500/30 bg-red-500/10 text-red-700 dark:text-red-300'
  }
  if (state === 'sold') {
    return 'border-foreground/25 bg-foreground text-background'
  }
  if (state === 'unsold') {
    return 'border-orange-500/30 bg-orange-500/10 text-orange-700 dark:text-orange-300'
  }
  return 'border-foreground/15 bg-foreground/[0.04] text-foreground/60'
}

const actionTitle = (row: ListingRow) => {
  return t(`listings.actionTitle.${row.state}`)
}

const actionDescription = (row: ListingRow) => {
  return t(`listings.actionDescription.${row.state}`)
}

const canEdit = (status: string) => status === 'draft' || status === 'rejected'

const canDelete = (status: string) =>
  ['draft', 'rejected', 'pending'].includes(status)

const canPublish = (status: string) =>
  status === 'draft' || status === 'rejected'

const canStartAuction = (row: ListingRow) => {
  if (row.normalizedStatus === 'unsold') {
    return true
  }

  if (row.auction?.auctionId) {
    if (
      row.auctionStatus.includes('active') ||
      row.auctionStatus.includes('live') ||
      row.auctionStatus.includes('finished') ||
      row.auctionStatus === 'sold' ||
      row.auctionStatus.includes('expired')
    ) {
      return false
    }
  }

  return ['approved', 'active', 'unsold'].includes(row.normalizedStatus)
}

const isActionPending = (lotId: string, action?: ListingAction) =>
  pendingAction.value?.lotId === lotId &&
  (!action || pendingAction.value.action === action)

const isLotBusy = (lotId: string) => pendingAction.value?.lotId === lotId

const isNotFoundError = (error: unknown) =>
  typeof error === 'object' &&
  error !== null &&
  (error as { response?: { status?: number } }).response?.status === 404

const resolveAuctionIdForLot = async (row: ListingRow) => {
  if (row.auction?.auctionId) {
    return row.auction.auctionId
  }

  try {
    const existingAuction = await auctionService.getAuctionByLotId(row.lot.id)
    if (existingAuction.auctionId) {
      return existingAuction.auctionId
    }
  } catch (error) {
    if (!isNotFoundError(error)) {
      throw error
    }
  }

  return auctionService.createAuction({
    lotId: row.lot.id,
  })
}

const lotToDelete = computed(() =>
  listings.value.find((lot) => lot.id === lotToDeleteId.value),
)

const deleteModalTitle = computed(() =>
  lotToDelete.value
    ? t('listings.deleteTitle', { title: lotToDelete.value.title })
    : t('listings.deleteFallbackTitle'),
)

const emptyState = computed(() => {
  if (listingRows.value.length === 0) {
    return {
      title: t('listings.emptyFirst.title'),
      description: t('listings.emptyFirst.description'),
      action: t('listings.emptyFirst.action'),
      showAction: true,
    }
  }

  return {
    title: t('listings.emptyFiltered.title'),
    description: t('listings.emptyFiltered.description'),
    action: t('listings.emptyFiltered.action'),
    showAction: false,
  }
})

const formatPrice = (lot: LotPreview) => {
  const amount = Number(lot.price ?? 0)
  const rawCurrency = String(lot.currency ?? 'EUR').toUpperCase()
  const currency = /^[A-Z]{3}$/.test(rawCurrency) ? rawCurrency : 'EUR'

  if (!Number.isFinite(amount)) return `— ${currency}`

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

const formatDate = (value?: string) => {
  if (!value) return t('common.unavailable')

  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return t('common.unavailable')

  return new Intl.DateTimeFormat(locale.value, {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  }).format(date)
}

const markImageAsFailed = (lotId: string) => {
  failedImageIds.value = new Set([...failedImageIds.value, lotId])
}

const clearFilters = () => {
  activeFilter.value = 'all'
  searchQuery.value = ''
}

const loadData = async () => {
  try {
    isLoading.value = true
    errorMessage.value = ''
    failedImageIds.value = new Set()

    const [lotsResponse, auctionsResponse] = await Promise.all([
      itemService.getMyLots(),
      auctionService.getAuctions({ pageSize: 100 }).catch(() => []),
    ])

    listings.value = Array.isArray(lotsResponse) ? lotsResponse : []
    auctions.value = Array.isArray(auctionsResponse) ? auctionsResponse : []

    for (const lot of listings.value) {
      selectedDurationByLot.value[lot.id] =
        selectedDurationByLot.value[lot.id] ?? '7'
    }
  } catch (error) {
    console.error('Failed to load listings', error)
    errorMessage.value = t('listings.loadFailed')
  } finally {
    isLoading.value = false
  }
}

const openDeleteModal = (lotId: string) => {
  lotToDeleteId.value = lotId
  isDeleteModalOpen.value = true
}

const closeDeleteModal = () => {
  if (pendingAction.value?.action === 'delete') return
  lotToDeleteId.value = null
  isDeleteModalOpen.value = false
}

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

const confirmDeleteLot = async () => {
  const lotId = lotToDeleteId.value
  if (!lotId) return

  try {
    pendingAction.value = { lotId, action: 'delete' }
    await itemService.deleteLot(lotId)
    listings.value = listings.value.filter((item) => item.id !== lotId)
    toast.success(t('listings.deleted'), {
      position: 'bottom-right',
    })
    lotToDeleteId.value = null
    isDeleteModalOpen.value = false
  } catch (error: unknown) {
    const message =
      getApiErrorMessage(error, t('listings.deleteFailed'))
    toast.error(message, { position: 'bottom-right' })
  } finally {
    pendingAction.value = null
  }
}

const publishLot = async (lotId: string) => {
  try {
    pendingAction.value = { lotId, action: 'publish' }
    await itemService.publishLot(lotId)
    toast.success(t('listings.submittedReview'), {
      position: 'bottom-right',
    })
    await loadData()
  } catch (error: unknown) {
    const message =
      getApiErrorMessage(error, t('listings.submitFailed'))
    toast.error(message, { position: 'bottom-right' })
  } finally {
    pendingAction.value = null
  }
}

const startAuctionForLot = async (row: ListingRow) => {
  try {
    pendingAction.value = { lotId: row.lot.id, action: 'auction' }
    const targetAuctionId = await resolveAuctionIdForLot(row)

    const duration = selectedDurationByLot.value[row.lot.id] ?? '7'
    await auctionService.startAuction(targetAuctionId, duration)
    toast.success(t('listings.auctionStarted'), {
      position: 'bottom-right',
    })
    await loadData()
  } catch (error: unknown) {
    const message = getApiErrorMessage(error, t('listings.startAuctionFailed'))
    toast.error(message, { position: 'bottom-right' })
  } finally {
    pendingAction.value = null
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

  await loadData()
})
</script>

<template>
  <main class="min-h-[70vh] bg-background text-foreground">
    <div class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <header
        class="mb-8 flex flex-col gap-5 sm:flex-row sm:items-end sm:justify-between"
      >
        <div class="max-w-2xl">
          <h1 class="text-3xl font-semibold">{{ $t('listings.title') }}</h1>
          <p class="mt-2 text-sm text-foreground/70">
            {{ $t('listings.description') }}
          </p>
        </div>

        <RouterLink
          v-if="isAuthenticated"
          :to="localePath('/sell')"
          class="inline-flex min-h-10 items-center justify-center rounded-lg bg-foreground px-4 py-2 text-sm font-medium text-background transition-opacity hover:opacity-80 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground focus-visible:ring-offset-2 focus-visible:ring-offset-background"
        >
          {{ $t('listings.createListing') }}
        </RouterLink>
      </header>

      <div
        v-if="isAuthChecking"
        class="animate-pulse rounded-2xl border p-6"
        :aria-label="$t('listings.checkingSession')"
      >
        <div class="h-4 w-40 rounded bg-foreground/10" />
        <div class="mt-3 h-3 w-64 max-w-full rounded bg-foreground/[0.07]" />
      </div>

      <section
        v-else-if="!isAuthenticated"
        class="rounded-2xl border bg-background p-6"
      >
        <h2 class="text-lg font-semibold">{{ $t('listings.signInTitle') }}</h2>
        <p class="mt-2 text-sm text-foreground/70">
          {{ $t('listings.signInDescription') }}
        </p>
        <RouterLink
          :to="localePath('/login')"
          class="mt-5 inline-flex rounded-lg border px-4 py-2 text-sm transition hover:bg-foreground hover:text-background"
        >
          {{ $t('navigation.signIn') }}
        </RouterLink>
      </section>

      <template v-else>
        <div
          class="mb-6 flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between"
        >
          <nav
            class="grid grid-cols-2 gap-1 rounded-2xl border bg-background p-1.5 sm:flex"
            :aria-label="$t('listings.filterAria')"
          >
            <button
              v-for="filter in filters"
              :key="filter.id"
              type="button"
              :aria-pressed="activeFilter === filter.id"
              :class="[
                'inline-flex min-h-10 items-center justify-center gap-2 rounded-xl px-3 text-xs font-medium transition sm:px-4 sm:text-sm',
                'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-foreground focus-visible:ring-offset-2 focus-visible:ring-offset-background',
                activeFilter === filter.id
                  ? 'bg-foreground text-background'
                  : 'text-foreground/60 hover:bg-foreground/5 hover:text-foreground',
              ]"
              @click="activeFilter = filter.id"
            >
              <span>{{ filter.label }}</span>
              <span
                :class="
                  activeFilter === filter.id
                    ? 'text-background/70'
                    : 'text-foreground/40'
                "
                class="tabular-nums"
              >
                {{ filter.count }}
              </span>
            </button>
          </nav>

          <label class="relative block w-full lg:max-w-xs">
            <span class="sr-only">{{ $t('listings.searchLabel') }}</span>
            <input
              v-model="searchQuery"
              type="search"
              :placeholder="$t('listings.searchPlaceholder')"
              class="h-11 w-full rounded-xl border bg-background px-4 text-sm outline-none transition placeholder:text-foreground/40 focus:border-foreground/40 focus:ring-2 focus:ring-foreground/10"
            />
          </label>
        </div>

        <div
          v-if="isLoading"
          class="space-y-4"
          aria-live="polite"
          :aria-label="$t('listings.loadingListings')"
        >
          <div
            v-for="index in 3"
            :key="index"
            class="grid animate-pulse grid-cols-[6.5rem_minmax(0,1fr)] gap-4 rounded-2xl border p-4 sm:grid-cols-[8rem_minmax(0,1fr)] sm:gap-5 lg:grid-cols-[9rem_minmax(0,1fr)_16rem]"
          >
            <div class="aspect-[4/5] rounded-xl bg-foreground/[0.07]" />
            <div class="py-1">
              <div class="h-3 w-24 rounded bg-foreground/[0.07]" />
              <div class="mt-4 h-6 w-3/5 rounded bg-foreground/[0.07]" />
              <div class="mt-3 h-3 w-2/5 rounded bg-foreground/[0.05]" />
              <div class="mt-5 h-14 rounded-xl bg-foreground/[0.04]" />
            </div>
            <div
              class="col-span-2 h-16 rounded-xl bg-foreground/[0.04] lg:col-span-1 lg:h-full"
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
              {{ $t('listings.unableLoad') }}
            </p>
            <p class="mt-1 text-sm text-foreground/70">{{ errorMessage }}</p>
          </div>
          <button
            type="button"
            class="rounded-lg border px-4 py-2 text-sm transition hover:bg-foreground hover:text-background"
            @click="loadData"
          >
            {{ $t('common.tryAgain') }}
          </button>
        </div>

        <section
          v-else-if="visibleRows.length === 0"
          class="rounded-2xl border bg-background px-6 py-14 text-center sm:px-10"
        >
          <div class="mx-auto max-w-lg">
            <h2 class="text-xl font-semibold">{{ emptyState.title }}</h2>
            <p class="mt-3 text-sm leading-6 text-foreground/70">
              {{ emptyState.description }}
            </p>
            <RouterLink
              v-if="emptyState.showAction"
              :to="localePath('/sell')"
              class="mt-6 inline-flex rounded-lg border px-4 py-2 text-sm transition hover:bg-foreground hover:text-background"
            >
              {{ emptyState.action }}
            </RouterLink>
            <button
              v-else
              type="button"
              class="mt-6 rounded-lg border px-4 py-2 text-sm transition hover:bg-foreground hover:text-background"
              @click="clearFilters"
            >
              {{ emptyState.action }}
            </button>
          </div>
        </section>

        <div v-else class="space-y-4">
          <article
            v-for="row in visibleRows"
            :key="row.lot.id"
            class="group grid grid-cols-[6.5rem_minmax(0,1fr)] gap-4 rounded-2xl border bg-background p-4 transition-colors hover:border-foreground/30 sm:grid-cols-[8rem_minmax(0,1fr)] sm:gap-5 lg:grid-cols-[9rem_minmax(0,1fr)_16rem]"
          >
            <div
              class="aspect-[4/5] self-start overflow-hidden rounded-xl border bg-foreground/[0.04]"
            >
              <img
                v-if="
                  buildMediaUrl(row.lot.media?.[0]) &&
                  !failedImageIds.has(row.lot.id)
                "
                :src="buildMediaUrl(row.lot.media?.[0])"
                :alt="row.lot.title"
                class="h-full w-full object-cover transition duration-300 group-hover:scale-[1.02]"
                loading="lazy"
                @error="markImageAsFailed(row.lot.id)"
              />
              <div
                v-else
                class="flex h-full items-center justify-center p-3 text-center text-xs text-foreground/40"
              >
                {{ $t('listings.imageUnavailable') }}
              </div>
            </div>

            <div class="min-w-0 py-0.5">
              <span
                :class="[
                  'inline-flex rounded-full border px-2.5 py-1 text-xs font-medium',
                  statusBadgeClass(row.state),
                ]"
              >
                {{ statusLabel(row.state) }}
              </span>

              <h2
                class="mt-3 line-clamp-2 text-lg font-semibold leading-snug sm:text-xl"
              >
                {{ row.lot.title }}
              </h2>

              <div
                class="mt-2 flex flex-wrap items-center gap-x-3 gap-y-1 text-sm text-foreground/60"
              >
                <span v-if="row.lot.brand">{{ row.lot.brand }}</span>
                <span v-if="row.lot.brand" aria-hidden="true">·</span>
                <span class="font-medium text-foreground">
                  {{ formatPrice(row.lot) }}
                </span>
              </div>

              <p class="mt-1 text-xs text-foreground/45">
                {{ $t('listings.listedOn', { date: formatDate(row.lot.createdAt) }) }}
              </p>

              <div class="mt-4 rounded-xl bg-foreground/[0.04] p-3">
                <p class="text-xs font-medium text-foreground/60">
                  {{ actionTitle(row) }}
                </p>
                <p class="mt-1 text-xs leading-5 text-foreground/70 sm:text-sm">
                  {{ actionDescription(row) }}
                </p>
              </div>
            </div>

            <div
              class="col-span-2 flex flex-col justify-between gap-3 border-t pt-4 lg:col-span-1 lg:border-l lg:border-t-0 lg:pl-5 lg:pt-0"
            >
              <div v-if="canStartAuction(row)" class="space-y-2">
                <label
                  class="block text-xs text-foreground/50"
                  :for="`duration-${row.lot.id}`"
                >
                  {{ $t('listings.auctionDuration') }}
                </label>
                <select
                  :id="`duration-${row.lot.id}`"
                  v-model="selectedDurationByLot[row.lot.id]"
                  class="h-10 w-full rounded-lg border bg-background px-3 text-sm outline-none focus:border-foreground/40 focus:ring-2 focus:ring-foreground/10"
                  :disabled="isLotBusy(row.lot.id)"
                >
                  <option value="3">{{ $t('listings.durationDays', { count: 3 }) }}</option>
                  <option value="5">{{ $t('listings.durationDays', { count: 5 }) }}</option>
                  <option value="7">{{ $t('listings.durationDays', { count: 7 }) }}</option>
                </select>
                <button
                  type="button"
                  class="h-10 w-full rounded-lg bg-foreground px-3 text-sm font-medium text-background transition-opacity hover:opacity-80 disabled:cursor-wait disabled:opacity-50"
                  :disabled="isLotBusy(row.lot.id)"
                  @click="startAuctionForLot(row)"
                >
                  {{
                    isActionPending(row.lot.id, 'auction')
                      ? row.state === 'unsold'
                        ? $t('listings.restarting')
                        : $t('listings.starting')
                      : row.state === 'unsold'
                        ? $t('listings.restartAuction')
                        : $t('listings.startAuction')
                  }}
                </button>
              </div>

              <div class="flex flex-wrap gap-2 lg:flex-col">
                <RouterLink
                  :to="localePath(`/lot/${row.lot.id}`)"
                  class="inline-flex min-h-10 flex-1 items-center justify-center rounded-lg border px-3 text-sm transition hover:bg-foreground hover:text-background lg:w-full"
                >
                  {{ $t('listings.viewListing') }}
                </RouterLink>

                <RouterLink
                  v-if="canEdit(row.normalizedStatus)"
                  :to="localePath(`/lots/${row.lot.id}/edit`)"
                  class="inline-flex min-h-10 flex-1 items-center justify-center rounded-lg border px-3 text-sm transition hover:bg-foreground hover:text-background lg:w-full"
                >
                  {{ $t('listings.edit') }}
                </RouterLink>

                <button
                  v-if="canPublish(row.normalizedStatus)"
                  type="button"
                  class="min-h-10 flex-1 rounded-lg bg-foreground px-3 text-sm font-medium text-background transition-opacity hover:opacity-80 disabled:cursor-wait disabled:opacity-50 lg:w-full"
                  :disabled="isLotBusy(row.lot.id)"
                  @click="publishLot(row.lot.id)"
                >
                  {{
                    isActionPending(row.lot.id, 'publish')
                      ? $t('listings.submitting')
                      : $t('listings.submitForReview')
                  }}
                </button>

                <button
                  v-if="canDelete(row.normalizedStatus)"
                  type="button"
                  class="min-h-10 flex-1 rounded-lg px-3 text-sm text-red-600 transition hover:bg-red-500/10 disabled:cursor-wait disabled:opacity-50 dark:text-red-300 lg:w-full"
                  :disabled="isLotBusy(row.lot.id)"
                  @click="openDeleteModal(row.lot.id)"
                >
                  {{ $t('listings.deleteListing') }}
                </button>
              </div>
            </div>
          </article>
        </div>
      </template>

      <ConfirmDeleteModal
        v-if="isDeleteModalOpen && lotToDelete"
        :title="deleteModalTitle"
        :message="$t('listings.deleteConfirmMessage')"
        @cancel="closeDeleteModal"
        @confirm="confirmDeleteLot"
      />
    </div>
  </main>
</template>
