<script setup lang="ts">
import {
  computed,
  ref,
  watch,
} from 'vue'
import { useI18n } from 'vue-i18n'

import {
  RouterLink,
  useRoute,
  useRouter,
  type LocationQueryRaw,
} from 'vue-router'

import AuctionCard from '@/components/auctions/AuctionCard.vue'
import CatalogFiltersDrawer from '@/components/catalog/CatalogFiltersDrawer.vue'
import CatalogToolbar from '@/components/catalog/CatalogToolbar.vue'

import { itemService } from '@/app/services/lotService'

import {
  getStringQueryParam,
  mergeLockedFilters,
  parseCatalogFilters,
  parseCatalogSort,
  removeCatalogFilterQuery,
  serializeCatalogFilters,
} from '@/app/helpers/catalogHelpers'

import {
  cloneCatalogFilters,
  createEmptyCatalogFilters,
  isCatalogArrayFilterKey,
  type CatalogActiveFilter,
  type CatalogArrayFilterKey,
  type CatalogFilters,
  type CatalogLockedFilters,
  type CatalogSort,
} from '@/types/catalog'

import type { LotPreview } from '@/types/lot'
import { useLocalePath } from '@/composables/useLocalePath'

const route = useRoute()
const router = useRouter()
const localePath = useLocalePath()
const { t, te } = useI18n()

const lots = ref<LotPreview[]>([])
const isLoading = ref(false)
const errorMessage = ref('')

const totalCount = ref(0)
const totalPages = ref(0)
const currentPage = ref(1)
const pageSize = ref(20)

const filtersDrawerOpen = ref(false)

let latestRequestId = 0

const searchQuery = computed(() => {
  const value = route.query.query

  if (typeof value !== 'string') {
    return ''
  }

  return value.trim()
})

const pageFromRoute = computed(() => {
  const value = route.query.page

  if (typeof value !== 'string') {
    return 1
  }

  const parsedPage = Number(value)

  if (
    !Number.isInteger(parsedPage) ||
    parsedPage < 1
  ) {
    return 1
  }

  return parsedPage
})

/*
 * На странице поиска нет обязательного gender,
 * category или brand из route params.
 *
 * Поэтому заблокированных фильтров нет.
 */
const lockedFilters = computed<CatalogLockedFilters>(
  () => ({}),
)

const routeFilters = computed<CatalogFilters>(
  () => parseCatalogFilters(route.query),
)

const effectiveFilters = computed<CatalogFilters>(
  () =>
    mergeLockedFilters(
      routeFilters.value,
      lockedFilters.value,
    ),
)

const selectedSort = computed<CatalogSort>(
  () =>
    parseCatalogSort(
      getStringQueryParam(
        route.query,
        'sort',
      ),
    ),
)

function isLockedValue(
  key: CatalogArrayFilterKey,
  value: string,
): boolean {
  return (
    lockedFilters.value[key]?.includes(value) ??
    false
  )
}

function catalogOptionLabel(value: string): string {
  const key = `catalog.options.${value}`

  return te(key) ? t(key) : value
}

const activeFilters = computed<
  CatalogActiveFilter[]
>(() => {
  const result: CatalogActiveFilter[] = []

  const addArrayFilters = (
    key: CatalogArrayFilterKey,
    values: string[],
    labelFactory: (
      value: string,
    ) => string = value => value,
  ): void => {
    values.forEach(value => {
      result.push({
        id: `${key}:${value}`,
        key,
        value,
        label: labelFactory(value),
        removable: !isLockedValue(
          key,
          value,
        ),
      })
    })
  }

  addArrayFilters(
    'genders',
    effectiveFilters.value.genders,
    catalogOptionLabel,
  )

  addArrayFilters(
    'categories',
    effectiveFilters.value.categories,
    catalogOptionLabel,
  )

  addArrayFilters(
    'brands',
    effectiveFilters.value.brands,
  )

  addArrayFilters(
    'sizes',
    effectiveFilters.value.sizes,
    value => t('common.size', { value }),
  )

  addArrayFilters(
    'conditions',
    effectiveFilters.value.conditions,
    catalogOptionLabel,
  )

  addArrayFilters(
    'countries',
    effectiveFilters.value.countries,
    catalogOptionLabel,
  )

  if (
    effectiveFilters.value.minPrice !== null ||
    effectiveFilters.value.maxPrice !== null
  ) {
    const from =
      effectiveFilters.value.minPrice !== null
        ? `€${effectiveFilters.value.minPrice}`
        : '€0'

    const to =
      effectiveFilters.value.maxPrice !== null
        ? `€${effectiveFilters.value.maxPrice}`
        : t('common.any')

    result.push({
      id: 'price',
      key: 'price',
      label: `${from} – ${to}`,
      removable: true,
    })
  }

  if (effectiveFilters.value.endingSoon) {
    result.push({
      id: 'endingSoon',
      key: 'endingSoon',
      label: t('catalog.endingSoon'),
      removable: true,
    })
  }

  if (effectiveFilters.value.newlyListed) {
    result.push({
      id: 'newlyListed',
      key: 'newlyListed',
      label: t('catalog.newlyListed'),
      removable: true,
    })
  }

  return result
})

const loadSearchResults =
  async (): Promise<void> => {
    const query = searchQuery.value

    if (!query) {
      lots.value = []
      totalCount.value = 0
      totalPages.value = 0
      currentPage.value = 1
      errorMessage.value = ''
      isLoading.value = false

      return
    }

    const requestId = ++latestRequestId

    try {
      isLoading.value = true
      errorMessage.value = ''

      const filters = effectiveFilters.value

      const result = await itemService.getLots({
        search: query,

        genders: filters.genders,
        categories: filters.categories,
        brands: filters.brands,
        sizes: filters.sizes,
        conditions: filters.conditions,
        countries: filters.countries,

        minPrice:
          filters.minPrice ?? undefined,

        maxPrice:
          filters.maxPrice ?? undefined,

        endingSoon: filters.endingSoon,
        newlyListed: filters.newlyListed,

        sort: selectedSort.value,

        page: pageFromRoute.value,
        pageSize: pageSize.value,
      })

      /*
       * Если пользователь быстро поменял фильтры,
       * старый запрос не должен перезаписать новый.
       */
      if (requestId !== latestRequestId) {
        return
      }

      lots.value = result.items
      totalCount.value = result.totalCount
      totalPages.value = result.totalPages
      currentPage.value = result.page
    } catch (error) {
      if (requestId !== latestRequestId) {
        return
      }

      console.error(
        'Failed to load search results:',
        error,
      )

      lots.value = []
      totalCount.value = 0
      totalPages.value = 0
      currentPage.value = 1
      errorMessage.value =
        t('search.failed')
    } finally {
      if (requestId === latestRequestId) {
        isLoading.value = false
      }
    }
  }

const applyFilters = async (
  filters: CatalogFilters,
): Promise<void> => {
  /*
   * Удаляем старые фильтры из URL,
   * но сохраняем query поиска, sort и остальные
   * параметры страницы.
   */
  const nextQuery =
    removeCatalogFilterQuery(
      route.query,
    )

  Object.assign(
    nextQuery,
    serializeCatalogFilters(filters),
  )

  /*
   * После изменения фильтра всегда
   * возвращаемся на первую страницу.
   */
  delete nextQuery.page

  await router.push({
    path: route.path,
    query: nextQuery,
  })

  filtersDrawerOpen.value = false
}

const changeSort = async (
  sort: CatalogSort,
): Promise<void> => {
  const nextQuery: LocationQueryRaw = {
    ...route.query,
  }

  if (sort === 'NewlyListed') {
    delete nextQuery.sort
  } else {
    nextQuery.sort = sort
  }

  delete nextQuery.page

  await router.push({
    path: route.path,
    query: nextQuery,
  })
}

const removeActiveFilter = async (
  filter: CatalogActiveFilter,
): Promise<void> => {
  if (!filter.removable) {
    return
  }

  const nextFilters =
    cloneCatalogFilters(
      routeFilters.value,
    )

  if (
    isCatalogArrayFilterKey(filter.key) &&
    filter.value
  ) {
    nextFilters[filter.key] =
      nextFilters[filter.key].filter(
        value => value !== filter.value,
      )
  }

  if (filter.key === 'price') {
    nextFilters.minPrice = null
    nextFilters.maxPrice = null
  }

  if (filter.key === 'endingSoon') {
    nextFilters.endingSoon = false
  }

  if (filter.key === 'newlyListed') {
    nextFilters.newlyListed = false
  }

  await applyFilters(nextFilters)
}

const clearAllFilters =
  async (): Promise<void> => {
    await applyFilters(
      createEmptyCatalogFilters(),
    )
  }

const changePage = async (
  page: number,
): Promise<void> => {
  if (
    page === currentPage.value ||
    page < 1 ||
    page > totalPages.value
  ) {
    return
  }

  const nextQuery: LocationQueryRaw = {
    ...route.query,
  }

  if (page === 1) {
    delete nextQuery.page
  } else {
    nextQuery.page = String(page)
  }

  await router.push({
    path: route.path,
    query: nextQuery,
  })

  window.scrollTo({
    top: 0,
    behavior: 'smooth',
  })
}

/*
 * Следим за полным URL, а не только за searchQuery.
 *
 * Теперь изменение:
 * - фильтра;
 * - сортировки;
 * - страницы;
 * - текста поиска
 *
 * вызовет новый запрос.
 */
watch(
  () => route.fullPath,
  () => {
    void loadSearchResults()
  },
  {
    immediate: true,
  },
)
</script>

<template>
    <section class="rounded-[28px] px-4 py-6 md:px-6">

        <div class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
            <div class="mb-6">
                <h1 class="text-3xl font-semibold">
                    {{ $t('search.resultsFor', { query: searchQuery }) }}
                </h1>
                
                <p
                v-if="!isLoading && searchQuery"
                class="mt-2 text-sm text-foreground/70"
                >
                  {{ $t('common.listingCount', { count: totalCount }) }}
                </p>  
            </div>
            
            <CatalogToolbar
              :active-filters="activeFilters"
              :sort="selectedSort"
              :disabled="isLoading"
              @open-filters="
                filtersDrawerOpen = true
              "
              @update:sort="changeSort"
              @remove-filter="removeActiveFilter"
              @clear-all="clearAllFilters"
            />

            <CatalogFiltersDrawer
              v-model:open="filtersDrawerOpen"
              :model-value="effectiveFilters"
              :locked="lockedFilters"
              @apply="applyFilters"
            />


            <div
            v-if="!searchQuery"
            class="rounded-2xl border p-6 text-sm text-foreground/70"
            >
            {{ $t('search.enterQuery') }}
        </div>
        
        <div
        v-else-if="isLoading"
        class="rounded-2xl border p-6 text-sm text-foreground/70"
        >
        {{ $t('common.loading') }}
    </div>

    <div
    v-else-if="errorMessage"
    class="rounded-2xl border border-red-500/30 bg-red-500/10 p-6 text-sm text-red-500"
    >
    {{ errorMessage }}
    </div>

    <div
    v-else-if="lots.length === 0"
    class="rounded-2xl border p-6 text-sm text-foreground/70"
    >
    {{ $t('search.noResultsFor', { query: searchQuery }) }}
    </div>

            <div
            v-else
            class="grid grid-cols-1 gap-x-3 gap-y-6 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5"
            >
                <RouterLink
                v-for="lot in lots"
                :key="lot.id"
                :to="localePath(`/lots/${lot.id}`)"
                class="group overflow-hidden"
                >
                    <AuctionCard
                    :brand="lot.brand"
                    :title="lot.title"
                    :price="lot.price"
                    :currency="lot.currency"
                    :media="lot.media"
                    :time-left="lot.statusName ?? $t('catalog.status.Live')"
                    />
                </RouterLink>
            </div>
        </div>
    </section>
</template>
