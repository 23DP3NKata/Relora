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

import { lookupService } from '@/app/services/lookupService'
import { itemService } from '@/app/services/lotService'
import { brandSlugToName } from '@/app/helpers/slugHelpers'

import {
  getStringQueryParam,
  mergeLockedFilters,
  parseCatalogFilters,
  parseCatalogSort,
  removeCatalogFilterQuery,
  serializeCatalogFilters,
  stripLockedFilters,
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
import type { CategoryNode, LotFormLookups, LookupOption } from '@/types/lotCatalog'
import { useLocalePath } from '@/composables/useLocalePath'

import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationNext,
  PaginationPrevious,
} from '@/components/ui/pagination'

const route = useRoute()
const router = useRouter()
const localePath = useLocalePath()
const { t, te } = useI18n()

const lots = ref<LotPreview[]>([])
const lookups = ref<LotFormLookups | null>(null)
const isLoading = ref(true)
const isLookupLoading = ref(false)
const errorMessage = ref('')

const totalCount = ref(0)
const totalPages = ref(0)
const currentPage = ref(1)
const pageSize = ref(20)

const filtersDrawerOpen = ref(false)

let latestRequestId = 0

const rootRouteMap: Record<string, string[]> = {
  bags: ['bags'],
  shoes: ['shoes'],
  accessories: ['accessories'],
  jewellery: ['jewellery', 'watches'],
}

const pageFromRoute = computed(() => {
  const value = route.query.page

  if (typeof value !== 'string') {
    return 1
  }

  const page = Number(value)

  if (
    !Number.isInteger(page) ||
    page < 1
  ) {
    return 1
  }

  return page
})

const brandFromRoute = computed(() => {
  if (route.name !== 'designer-catalog') {
    return undefined
  }

  const brandSlug = route.params.brandSlug

  if (typeof brandSlug !== 'string') {
    return undefined
  }

  return brandSlugToName(brandSlug)
})

const departmentFromRoute = computed(() => {
  const gender = route.params.gender

  if (gender === 'men') {
    return 'Men'
  }

  if (gender === 'women') {
    return 'Women'
  }

  if (gender === 'unisex') {
    return 'Unisex'
  }

  return undefined
})

const routeCategoryParam = computed(() => {
  const category = route.params.category

  return typeof category === 'string'
    ? category.toLowerCase()
    : undefined
})

const routeSubcategoryParam = computed(() => {
  const subcategory = route.params.subcategory

  return typeof subcategory === 'string'
    ? subcategory.toLowerCase()
    : undefined
})

const rootCategorySlugsFromRoute = computed(() => {
  const category = routeCategoryParam.value

  if (!category || category === 'vintage') {
    return []
  }

  return rootRouteMap[category] ?? []
})

const categorySlugsFromRoute = computed(() => {
  if (routeSubcategoryParam.value) {
    return [routeSubcategoryParam.value]
  }

  const category = routeCategoryParam.value

  if (
    category &&
    category !== 'vintage' &&
    !(category in rootRouteMap)
  ) {
    return [category]
  }

  return []
})

const isVintageRoute = computed(
  () => routeCategoryParam.value === 'vintage',
)

function localizedKeyLabel(key: string, fallback: string): string {
  return te(key) ? t(key) : fallback
}

function catalogOptionLabel(value: string): string {
  const key = `catalog.options.${value}`

  return localizedKeyLabel(key, value)
}

function visitCategoryTree(
  categories: CategoryNode[],
  predicate: (category: CategoryNode) => boolean,
): CategoryNode | undefined {
  for (const category of categories) {
    if (predicate(category)) {
      return category
    }

    const match = visitCategoryTree(
      category.children ?? [],
      predicate,
    )

    if (match) {
      return match
    }
  }

  return undefined
}

function categoryBySlug(slug: string): CategoryNode | undefined {
  return visitCategoryTree(
    lookups.value?.categories ?? [],
    category => category.slug === slug,
  )
}

function categoryLabel(slug: string): string {
  const category = categoryBySlug(slug)

  return category
    ? localizedKeyLabel(category.nameKey, slug)
    : catalogOptionLabel(slug)
}

function lookupOptionById(
  options: LookupOption[] | undefined,
  id: string,
): LookupOption | undefined {
  return options?.find(option => option.id === id)
}

function lookupOptionLabel(
  option: LookupOption | undefined,
  fallback: string,
): string {
  if (!option) {
    return fallback
  }

  return localizedKeyLabel(option.nameKey, option.code)
}

const pageTitle = computed(() => {
  if (brandFromRoute.value) {
    return brandFromRoute.value
  }

  if (isVintageRoute.value) {
    return t('navigation.vintage')
  }

  if (routeSubcategoryParam.value) {
    const categoryTitle = categoryLabel(routeSubcategoryParam.value)

    return departmentFromRoute.value
      ? `${localizedKeyLabel(`departments.${departmentFromRoute.value}`, departmentFromRoute.value)} ${categoryTitle}`
      : categoryTitle
  }

  if (rootCategorySlugsFromRoute.value.length === 1) {
    const slug = rootCategorySlugsFromRoute.value[0]

    return slug
      ? categoryLabel(slug)
      : t('catalog.title')
  }

  if (rootCategorySlugsFromRoute.value.includes('jewellery')) {
    return t('navigation.jewelryWatches')
  }

  if (departmentFromRoute.value) {
    return localizedKeyLabel(
      `departments.${departmentFromRoute.value}`,
      departmentFromRoute.value,
    )
  }

  return t('catalog.title')
})

const lockedFilters = computed<CatalogLockedFilters>(
  () => ({
    departments: departmentFromRoute.value
      ? [departmentFromRoute.value]
      : [],

    rootCategorySlugs: rootCategorySlugsFromRoute.value,
    categorySlugs: categorySlugsFromRoute.value,

    brands: brandFromRoute.value
      ? [brandFromRoute.value]
      : [],

    vintageOnly: isVintageRoute.value,
  }),
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

const activeFilters = computed<CatalogActiveFilter[]>(() => {
  const result: CatalogActiveFilter[] = []

  const addArrayFilters = (
    key: CatalogArrayFilterKey,
    values: string[],
    labelFactory: (value: string) => string = value => value,
  ): void => {
    values.forEach(value => {
      result.push({
        id: `${key}:${value}`,
        key,
        value,
        label: labelFactory(value),
        removable: !isLockedValue(key, value),
      })
    })
  }

  addArrayFilters(
    'departments',
    effectiveFilters.value.departments,
    value => localizedKeyLabel(`departments.${value}`, value),
  )

  addArrayFilters(
    'rootCategorySlugs',
    effectiveFilters.value.rootCategorySlugs,
    categoryLabel,
  )

  addArrayFilters(
    'categorySlugs',
    effectiveFilters.value.categorySlugs,
    categoryLabel,
  )

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

  addArrayFilters(
    'materialIds',
    effectiveFilters.value.materialIds,
    value => lookupOptionLabel(
      lookupOptionById(lookups.value?.materials, value),
      value,
    ),
  )

  addArrayFilters(
    'primaryColorIds',
    effectiveFilters.value.primaryColorIds,
    value => lookupOptionLabel(
      lookupOptionById(lookups.value?.colors, value),
      value,
    ),
  )

  if (
    effectiveFilters.value.minPrice !== null ||
    effectiveFilters.value.maxPrice !== null
  ) {
    const from =
      effectiveFilters.value.minPrice !== null
        ? `EUR ${effectiveFilters.value.minPrice}`
        : 'EUR 0'

    const to =
      effectiveFilters.value.maxPrice !== null
        ? `EUR ${effectiveFilters.value.maxPrice}`
        : t('common.any')

    result.push({
      id: 'price',
      key: 'price',
      label: `${from} - ${to}`,
      removable: true,
    })
  }

  if (
    effectiveFilters.value.productionYearFrom !== null ||
    effectiveFilters.value.productionYearTo !== null
  ) {
    const from =
      effectiveFilters.value.productionYearFrom ?? t('common.any')
    const to =
      effectiveFilters.value.productionYearTo ?? t('common.any')

    result.push({
      id: 'productionYear',
      key: 'productionYear',
      label: `${t('catalog.productionYear')}: ${from} - ${to}`,
      removable: true,
    })
  }

  if (effectiveFilters.value.vintageOnly) {
    result.push({
      id: 'vintageOnly',
      key: 'vintageOnly',
      label: t('catalog.vintageOnly'),
      removable: lockedFilters.value.vintageOnly !== true,
    })
  }

  if (effectiveFilters.value.hasMeasurements) {
    result.push({
      id: 'hasMeasurements',
      key: 'hasMeasurements',
      label: t('catalog.hasMeasurements'),
      removable: true,
    })
  }

  if (effectiveFilters.value.hasProofOfOrigin) {
    result.push({
      id: 'hasProofOfOrigin',
      key: 'hasProofOfOrigin',
      label: t('catalog.hasProofOfOrigin'),
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

const loadCatalogLookups = async (): Promise<void> => {
  if (lookups.value || isLookupLoading.value) {
    return
  }

  try {
    isLookupLoading.value = true
    lookups.value = await lookupService.getLotFormLookups()
  } catch (error) {
    console.error(
      'Failed to load catalog lookups:',
      error,
    )
  } finally {
    isLookupLoading.value = false
  }
}

const loadLots = async (): Promise<void> => {
  const requestId = ++latestRequestId

  try {
    isLoading.value = true
    errorMessage.value = ''

    const filters = effectiveFilters.value

    const result = await itemService.getLots({
      search: getStringQueryParam(
        route.query,
        'search',
      ),

      departments: filters.departments,
      genders: filters.genders,
      categories: filters.categories,
      rootCategorySlugs: filters.rootCategorySlugs,
      categorySlugs: filters.categorySlugs,
      brands: filters.brands,
      sizes: filters.sizes,
      conditions: filters.conditions,
      countries: filters.countries,
      materialIds: filters.materialIds,
      primaryColorIds: filters.primaryColorIds,

      minPrice:
        filters.minPrice ?? undefined,

      maxPrice:
        filters.maxPrice ?? undefined,

      productionYearFrom:
        filters.productionYearFrom ?? undefined,

      productionYearTo:
        filters.productionYearTo ?? undefined,

      vintageOnly: filters.vintageOnly,
      hasMeasurements: filters.hasMeasurements,
      hasProofOfOrigin: filters.hasProofOfOrigin,
      endingSoon: filters.endingSoon,
      newlyListed: filters.newlyListed,

      sort: selectedSort.value,

      page: pageFromRoute.value,
      pageSize: pageSize.value,
    })

    if (requestId !== latestRequestId) {
      return
    }

    lots.value = result.items
    currentPage.value = result.page
    totalCount.value = result.totalCount
    totalPages.value = result.totalPages
  } catch (error) {
    if (requestId !== latestRequestId) {
      return
    }

    console.error(
      'Failed to load catalog:',
      error,
    )

    lots.value = []
    totalCount.value = 0
    totalPages.value = 0
    currentPage.value = 1

    errorMessage.value =
      t('catalog.failed')
  } finally {
    if (requestId === latestRequestId) {
      isLoading.value = false
    }
  }
}

const applyFilters = async (
  filters: CatalogFilters,
): Promise<void> => {
  const unlockedFilters =
    stripLockedFilters(
      filters,
      lockedFilters.value,
    )

  const nextQuery =
    removeCatalogFilterQuery(
      route.query,
    )

  Object.assign(
    nextQuery,
    serializeCatalogFilters(
      unlockedFilters,
    ),
  )

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

  if (filter.key === 'productionYear') {
    nextFilters.productionYearFrom = null
    nextFilters.productionYearTo = null
  }

  if (filter.key === 'vintageOnly') {
    nextFilters.vintageOnly = false
  }

  if (filter.key === 'hasMeasurements') {
    nextFilters.hasMeasurements = false
  }

  if (filter.key === 'hasProofOfOrigin') {
    nextFilters.hasProofOfOrigin = false
  }

  if (filter.key === 'endingSoon') {
    nextFilters.endingSoon = false
  }

  if (filter.key === 'newlyListed') {
    nextFilters.newlyListed = false
  }

  await applyFilters(
    mergeLockedFilters(
      nextFilters,
      lockedFilters.value,
    ),
  )
}

const clearAllFilters =
  async (): Promise<void> => {
    await applyFilters(
      mergeLockedFilters(
        createEmptyCatalogFilters(),
        lockedFilters.value,
      ),
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

watch(
  () => route.fullPath,
  () => {
    void loadCatalogLookups()
    void loadLots()
  },
  {
    immediate: true,
  },
)
</script>

<template>
  <div
    class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8"
  >
    <section class="rounded-[28px] px-4 md:px-6">
      <div
        class="mb-6 flex items-end justify-between gap-3"
      >
        <div>
          <h1
            class="text-2xl font-semibold tracking-tight sm:text-3xl"
          >
            {{ pageTitle }}
          </h1>

          <p
            class="mt-2 text-sm text-foreground/70 sm:text-base"
          >
            {{ $t('common.listingCount', { count: totalCount }) }}
          </p>
        </div>
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
        :lookups="lookups"
        @apply="applyFilters"
      />

      <div
        v-if="isLoading"
        class="rounded-2xl border p-6 text-sm text-foreground/70"
      >
        {{ $t('common.loading') }}
      </div>

      <div
        v-else-if="errorMessage"
        class="rounded-2xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-600 dark:border-red-900/40 dark:bg-red-950/30 dark:text-red-300"
      >
        {{ errorMessage }}
      </div>

      <div
        v-else-if="lots.length === 0"
        class="rounded-2xl border p-6 text-sm text-foreground/70"
      >
        {{ $t('catalog.noListings') }}
      </div>

      <template v-else>
        <div
          class="grid grid-cols-1 gap-x-3 gap-y-6 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5"
        >
          <RouterLink
            v-for="lot in lots"
            :key="lot.id"
            :to="localePath(`/lots/${lot.id}`)"
            class="block min-w-0"
          >
            <AuctionCard
              :brand="lot.brand"
              :title="lot.title"
              :price="lot.price"
              :currency="lot.currency"
              :media="lot.media"
              :time-left="
                lot.statusName ?? $t('catalog.status.Live')
              "
            />
          </RouterLink>
        </div>

        <Pagination
          v-if="totalPages > 1"
          v-slot="{ page }"
          :page="currentPage"
          :items-per-page="pageSize"
          :total="totalCount"
          :sibling-count="1"
          :show-edges="true"
          class="mt-10"
          @update:page="changePage"
        >
          <PaginationContent
            v-slot="{ items }"
          >
            <PaginationPrevious />

            <template
              v-for="(item, index) in items"
              :key="index"
            >
              <PaginationItem
                v-if="item.type === 'page'"
                :value="item.value"
                :is-active="
                  item.value === page
                "
              >
                {{ item.value }}
              </PaginationItem>

              <PaginationEllipsis
                v-else
                :index="index"
              />
            </template>

            <PaginationNext />
          </PaginationContent>
        </Pagination>
      </template>
    </section>
  </div>
</template>
