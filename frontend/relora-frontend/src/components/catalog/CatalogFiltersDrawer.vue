<script setup lang="ts">
import {
  computed,
  onBeforeUnmount,
  ref,
  watch,
} from 'vue'
import { useI18n } from 'vue-i18n'

import {
  CheckIcon,
  MagnifyingGlassIcon,
  XMarkIcon,
} from '@heroicons/vue/24/outline'

import {
  brandOptions,
  conditionOptions,
  countryOptions,
  sizeOptions,
  type CatalogOption,
} from '@/config/catalogOptions'

import {
  createEmptyCatalogFilters,
  type CatalogArrayFilterKey,
  type CatalogFilters,
  type CatalogLockedFilters,
} from '@/types/catalog'

import {
  mergeLockedFilters,
} from '@/app/helpers/catalogHelpers'

import type {
  CategoryNode,
  LotFormLookups,
  LookupOption,
} from '@/types/lotCatalog'

const props = withDefaults(
  defineProps<{
    open: boolean
    modelValue: CatalogFilters
    locked?: CatalogLockedFilters
    lookups?: LotFormLookups | null
  }>(),
  {
    locked: () => ({}),
    lookups: null,
  },
)

const emit = defineEmits<{
  'update:open': [value: boolean]
  apply: [filters: CatalogFilters]
}>()

const { t, te } = useI18n()

const draft = ref<CatalogFilters>(
  createEmptyCatalogFilters(),
)

const brandSearch = ref('')
const countrySearch = ref('')

let previousBodyOverflow = ''

const rootCategories = computed(
  () => props.lookups?.categories ?? [],
)

const departments = computed(
  () => props.lookups?.departments ?? ['Women', 'Men', 'Unisex'],
)

const materialOptions = computed(
  () => props.lookups?.materials ?? [],
)

const colorOptions = computed(
  () => props.lookups?.colors ?? [],
)

const normalizedSizeOptions = computed<CatalogOption[]>(() => {
  const lookupSizes = props.lookups?.sizeOptions ?? []

  if (lookupSizes.length > 0) {
    return lookupSizes.map(option => ({
      value: option.code,
      labelKey: option.nameKey,
    }))
  }

  return sizeOptions
})

const availableSubcategories = computed(() => {
  const selectedRoots = new Set(draft.value.rootCategorySlugs)
  const sourceRoots = selectedRoots.size > 0
    ? rootCategories.value.filter(category =>
        selectedRoots.has(category.slug),
      )
    : rootCategories.value

  return sourceRoots.flatMap(
    category => category.children ?? [],
  )
})

const activeCount = computed(() => {
  return (
    draft.value.departments.length +
    draft.value.genders.length +
    draft.value.categories.length +
    draft.value.rootCategorySlugs.length +
    draft.value.categorySlugs.length +
    draft.value.brands.length +
    draft.value.sizes.length +
    draft.value.conditions.length +
    draft.value.countries.length +
    draft.value.materialIds.length +
    draft.value.primaryColorIds.length +
    Number(draft.value.minPrice !== null) +
    Number(draft.value.maxPrice !== null) +
    Number(draft.value.productionYearFrom !== null) +
    Number(draft.value.productionYearTo !== null) +
    Number(draft.value.vintageOnly) +
    Number(draft.value.hasMeasurements) +
    Number(draft.value.hasProofOfOrigin) +
    Number(draft.value.endingSoon) +
    Number(draft.value.newlyListed)
  )
})

function resetDraft(): void {
  draft.value = mergeLockedFilters(
    props.modelValue,
    props.locked,
  )

  brandSearch.value = ''
  countrySearch.value = ''
}

watch(
  [
    () => props.open,
    () => props.modelValue,
    () => props.locked,
  ],
  ([isOpen]) => {
    if (isOpen) {
      resetDraft()
    }
  },
  {
    deep: true,
    immediate: true,
  },
)

watch(
  () => draft.value.rootCategorySlugs,
  () => {
    const allowedSlugs = new Set(
      availableSubcategories.value.map(
        category => category.slug,
      ),
    )

    draft.value.categorySlugs =
      draft.value.categorySlugs.filter(
        slug => allowedSlugs.has(slug),
      )
  },
  { deep: true },
)

watch(
  () => props.open,
  isOpen => {
    if (typeof document === 'undefined') {
      return
    }

    if (isOpen) {
      previousBodyOverflow =
        document.body.style.overflow

      document.body.style.overflow = 'hidden'
    } else {
      document.body.style.overflow =
        previousBodyOverflow
    }
  },
)

onBeforeUnmount(() => {
  if (typeof document !== 'undefined') {
    document.body.style.overflow =
      previousBodyOverflow
  }
})

const allBrandOptions = computed(() => {
  const values = new Set([
    ...brandOptions.map(option => option.value),
    ...draft.value.brands,
  ])

  return [...values]
    .sort((a, b) => a.localeCompare(b))
    .map(value => ({
      value,
      label: value,
    }))
})

const filteredBrandOptions = computed(() => {
  const search = brandSearch.value
    .trim()
    .toLowerCase()

  if (!search) {
    return allBrandOptions.value
  }

  return allBrandOptions.value.filter(
    option =>
      option.label
        .toLowerCase()
        .includes(search),
  )
})

const allCountryOptions = computed(() => {
  const values = new Set([
    ...countryOptions.map(option => option.value),
    ...draft.value.countries,
  ])

  return [...values]
    .sort((a, b) => a.localeCompare(b))
    .map(value => ({
      value,
      label: catalogOptionLabel(value),
    }))
})

const filteredCountryOptions = computed(() => {
  const search = countrySearch.value
    .trim()
    .toLowerCase()

  if (!search) {
    return allCountryOptions.value
  }

  return allCountryOptions.value.filter(
    option =>
      option.label
        .toLowerCase()
        .includes(search),
  )
})

function catalogOptionLabel(value: string): string {
  const key = `catalog.options.${value}`

  return te(key) ? t(key) : value
}

function lookupOptionLabel(option: LookupOption): string {
  return te(option.nameKey)
    ? t(option.nameKey)
    : option.code
}

function categoryLabel(category: CategoryNode): string {
  return te(category.nameKey)
    ? t(category.nameKey)
    : category.slug
}

function sizeLabel(option: CatalogOption): string {
  if (option.labelKey && te(option.labelKey)) {
    return t(option.labelKey)
  }

  return option.value
}

function isSelected(
  key: CatalogArrayFilterKey,
  value: string,
): boolean {
  return draft.value[key].includes(value)
}

function isLocked(
  key: CatalogArrayFilterKey,
  value: string,
): boolean {
  return (
    props.locked[key]?.includes(value) ??
    false
  )
}

function toggleValue(
  key: CatalogArrayFilterKey,
  value: string,
): void {
  if (isLocked(key, value)) {
    return
  }

  const currentValues = draft.value[key]

  if (currentValues.includes(value)) {
    draft.value[key] = currentValues.filter(
      currentValue => currentValue !== value,
    )

    return
  }

  draft.value[key] = [
    ...currentValues,
    value,
  ]
}

function updateNumber(
  key:
    | 'minPrice'
    | 'maxPrice'
    | 'productionYearFrom'
    | 'productionYearTo',
  event: Event,
): void {
  const target = event.target as HTMLInputElement

  if (target.value === '') {
    draft.value[key] = null
    return
  }

  const value = Number(target.value)

  draft.value[key] =
    Number.isFinite(value) && value >= 0
      ? value
      : null
}

function clearDraft(): void {
  draft.value = mergeLockedFilters(
    createEmptyCatalogFilters(),
    props.locked,
  )
}

function closeDrawer(): void {
  emit('update:open', false)
}

function applyFilters(): void {
  emit(
    'apply',
    mergeLockedFilters(
      draft.value,
      props.locked,
    ),
  )

  closeDrawer()
}

function handleKeydown(
  event: KeyboardEvent,
): void {
  if (
    event.key === 'Escape' &&
    props.open
  ) {
    closeDrawer()
  }
}
</script>

<template>
  <Teleport to="body">
    <Transition name="catalog-drawer">
      <div
        v-if="open"
        class="fixed inset-0 z-[100] bg-black/35 backdrop-blur-[1px]"
        @click.self="closeDrawer"
        @keydown="handleKeydown"
      >
        <aside
          class="flex h-full w-full max-w-[390px] flex-col bg-background shadow-2xl"
          role="dialog"
          aria-modal="true"
          :aria-label="$t('catalog.drawerLabel')"
        >
          <header
            class="flex items-center justify-between border-b border-border px-5 py-5"
          >
            <div class="flex items-center gap-2">
              <h2 class="text-xl font-semibold">
                {{ $t('common.filters') }}
              </h2>

              <span
                class="inline-flex h-6 min-w-6 items-center justify-center rounded-full bg-foreground px-1.5 text-xs text-background"
              >
                {{ activeCount }}
              </span>
            </div>

            <button
              type="button"
              class="rounded-full border border-border p-2 transition hover:bg-muted"
              :aria-label="$t('catalog.closeFilters')"
              @click="closeDrawer"
            >
              <XMarkIcon class="h-5 w-5" />
            </button>
          </header>

          <div
            class="flex-1 space-y-7 overflow-y-auto px-5 py-6"
          >
            <section>
              <h3 class="mb-3 text-sm font-semibold">
                {{ $t('catalog.department') }}
              </h3>

              <div class="flex flex-wrap gap-2">
                <button
                  v-for="department in departments"
                  :key="department"
                  type="button"
                  class="rounded-full border px-4 py-2 text-sm transition"
                  :class="
                    isSelected('departments', department)
                      ? 'border-foreground bg-foreground text-background'
                      : 'border-border bg-background hover:border-foreground/40'
                  "
                  :disabled="
                    isLocked('departments', department)
                  "
                  @click="
                    toggleValue(
                      'departments',
                      department,
                    )
                  "
                >
                  {{ $t(`departments.${department}`) }}
                </button>
              </div>
            </section>

            <section>
              <h3 class="mb-3 text-sm font-semibold">
                {{ $t('catalog.rootCategory') }}
              </h3>

              <div class="flex flex-wrap gap-2">
                <button
                  v-for="category in rootCategories"
                  :key="category.id"
                  type="button"
                  class="rounded-full border px-4 py-2 text-sm transition"
                  :class="
                    isSelected(
                      'rootCategorySlugs',
                      category.slug,
                    )
                      ? 'border-foreground bg-foreground text-background'
                      : 'border-border bg-background hover:border-foreground/40'
                  "
                  :disabled="
                    isLocked(
                      'rootCategorySlugs',
                      category.slug,
                    )
                  "
                  @click="
                    toggleValue(
                      'rootCategorySlugs',
                      category.slug,
                    )
                  "
                >
                  {{ categoryLabel(category) }}
                </button>
              </div>
            </section>

            <section v-if="availableSubcategories.length > 0">
              <h3 class="mb-3 text-sm font-semibold">
                {{ $t('catalog.subcategory') }}
              </h3>

              <div class="flex flex-wrap gap-2">
                <button
                  v-for="category in availableSubcategories"
                  :key="category.id"
                  type="button"
                  class="rounded-full border px-4 py-2 text-sm transition"
                  :class="
                    isSelected(
                      'categorySlugs',
                      category.slug,
                    )
                      ? 'border-foreground bg-foreground text-background'
                      : 'border-border bg-background hover:border-foreground/40'
                  "
                  :disabled="
                    isLocked(
                      'categorySlugs',
                      category.slug,
                    )
                  "
                  @click="
                    toggleValue(
                      'categorySlugs',
                      category.slug,
                    )
                  "
                >
                  {{ categoryLabel(category) }}
                </button>
              </div>
            </section>

            <section>
              <h3 class="mb-3 text-sm font-semibold">
                {{ $t('catalog.brand') }}
              </h3>

              <div class="relative mb-3">
                <MagnifyingGlassIcon
                  class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-foreground/50"
                />

                <input
                  v-model="brandSearch"
                  type="search"
                  :placeholder="$t('catalog.searchBrands')"
                  class="h-11 w-full rounded-xl border border-border bg-background pl-9 pr-3 text-sm outline-none transition focus:border-foreground"
                />
              </div>

              <div
                class="max-h-52 space-y-1 overflow-y-auto pr-1"
              >
                <button
                  v-for="option in filteredBrandOptions"
                  :key="option.value"
                  type="button"
                  class="flex w-full items-center justify-between rounded-xl px-3 py-2.5 text-left text-sm transition hover:bg-muted"
                  :disabled="
                    isLocked(
                      'brands',
                      option.value,
                    )
                  "
                  @click="
                    toggleValue(
                      'brands',
                      option.value,
                    )
                  "
                >
                  <span>
                    {{ option.label }}
                  </span>

                  <span
                    class="flex h-5 w-5 items-center justify-center rounded border"
                    :class="
                      isSelected(
                        'brands',
                        option.value,
                      )
                        ? 'border-foreground bg-foreground text-background'
                        : 'border-border'
                    "
                  >
                    <CheckIcon
                      v-if="
                        isSelected(
                          'brands',
                          option.value,
                        )
                      "
                      class="h-3.5 w-3.5"
                    />
                  </span>
                </button>
              </div>
            </section>

            <section>
              <h3 class="mb-3 text-sm font-semibold">
                {{ $t('catalog.size') }}
              </h3>

              <div class="flex flex-wrap gap-2">
                <button
                  v-for="option in normalizedSizeOptions"
                  :key="option.value"
                  type="button"
                  class="flex h-10 min-w-11 items-center justify-center rounded-xl border px-3 text-sm transition"
                  :class="
                    isSelected(
                      'sizes',
                      option.value,
                    )
                      ? 'border-foreground bg-foreground text-background'
                      : 'border-border bg-background hover:border-foreground/40'
                  "
                  :disabled="
                    isLocked(
                      'sizes',
                      option.value,
                    )
                  "
                  @click="
                    toggleValue(
                      'sizes',
                      option.value,
                    )
                  "
                >
                  {{ sizeLabel(option) }}
                </button>
              </div>
            </section>

            <section>
              <h3 class="mb-3 text-sm font-semibold">
                {{ $t('catalog.condition') }}
              </h3>

              <div class="flex flex-wrap gap-2">
                <button
                  v-for="option in conditionOptions"
                  :key="option.value"
                  type="button"
                  class="rounded-full border px-4 py-2 text-sm transition"
                  :class="
                    isSelected(
                      'conditions',
                      option.value,
                    )
                      ? 'border-foreground bg-foreground text-background'
                      : 'border-border bg-background hover:border-foreground/40'
                  "
                  :disabled="
                    isLocked(
                      'conditions',
                      option.value,
                    )
                  "
                  @click="
                    toggleValue(
                      'conditions',
                      option.value,
                    )
                  "
                >
                  {{ catalogOptionLabel(option.value) }}
                </button>
              </div>
            </section>

            <section v-if="materialOptions.length > 0">
              <h3 class="mb-3 text-sm font-semibold">
                {{ $t('catalog.material') }}
              </h3>

              <div class="flex flex-wrap gap-2">
                <button
                  v-for="option in materialOptions"
                  :key="option.id"
                  type="button"
                  class="rounded-full border px-4 py-2 text-sm transition"
                  :class="
                    isSelected('materialIds', option.id)
                      ? 'border-foreground bg-foreground text-background'
                      : 'border-border bg-background hover:border-foreground/40'
                  "
                  :disabled="
                    isLocked('materialIds', option.id)
                  "
                  @click="
                    toggleValue(
                      'materialIds',
                      option.id,
                    )
                  "
                >
                  {{ lookupOptionLabel(option) }}
                </button>
              </div>
            </section>

            <section v-if="colorOptions.length > 0">
              <h3 class="mb-3 text-sm font-semibold">
                {{ $t('catalog.primaryColor') }}
              </h3>

              <div class="flex flex-wrap gap-2">
                <button
                  v-for="option in colorOptions"
                  :key="option.id"
                  type="button"
                  class="rounded-full border px-4 py-2 text-sm transition"
                  :class="
                    isSelected('primaryColorIds', option.id)
                      ? 'border-foreground bg-foreground text-background'
                      : 'border-border bg-background hover:border-foreground/40'
                  "
                  :disabled="
                    isLocked('primaryColorIds', option.id)
                  "
                  @click="
                    toggleValue(
                      'primaryColorIds',
                      option.id,
                    )
                  "
                >
                  {{ lookupOptionLabel(option) }}
                </button>
              </div>
            </section>

            <section>
              <h3 class="mb-3 text-sm font-semibold">
                {{ $t('catalog.price') }}
              </h3>

              <div class="grid grid-cols-2 gap-3">
                <label>
                  <span
                    class="mb-1.5 block text-xs text-foreground/60"
                  >
                    {{ $t('catalog.from') }}
                  </span>

                  <div class="relative">
                    <input
                      type="number"
                      min="0"
                      step="1"
                      :value="draft.minPrice ?? ''"
                      placeholder="0"
                      class="h-11 w-full rounded-xl border border-border bg-background px-3 pr-12 text-sm outline-none transition focus:border-foreground"
                      @input="
                        updateNumber(
                          'minPrice',
                          $event,
                        )
                      "
                    />

                    <span
                      class="pointer-events-none absolute right-3 top-1/2 -translate-y-1/2 text-xs text-foreground/50"
                    >
                      EUR
                    </span>
                  </div>
                </label>

                <label>
                  <span
                    class="mb-1.5 block text-xs text-foreground/60"
                  >
                    {{ $t('catalog.to') }}
                  </span>

                  <div class="relative">
                    <input
                      type="number"
                      min="0"
                      step="1"
                      :value="draft.maxPrice ?? ''"
                      placeholder="1000"
                      class="h-11 w-full rounded-xl border border-border bg-background px-3 pr-12 text-sm outline-none transition focus:border-foreground"
                      @input="
                        updateNumber(
                          'maxPrice',
                          $event,
                        )
                      "
                    />

                    <span
                      class="pointer-events-none absolute right-3 top-1/2 -translate-y-1/2 text-xs text-foreground/50"
                    >
                      EUR
                    </span>
                  </div>
                </label>
              </div>
            </section>

            <section>
              <h3 class="mb-3 text-sm font-semibold">
                {{ $t('catalog.productionYear') }}
              </h3>

              <div class="grid grid-cols-2 gap-3">
                <label>
                  <span
                    class="mb-1.5 block text-xs text-foreground/60"
                  >
                    {{ $t('catalog.from') }}
                  </span>

                  <input
                    type="number"
                    min="1800"
                    step="1"
                    :value="draft.productionYearFrom ?? ''"
                    placeholder="1990"
                    class="h-11 w-full rounded-xl border border-border bg-background px-3 text-sm outline-none transition focus:border-foreground"
                    @input="
                      updateNumber(
                        'productionYearFrom',
                        $event,
                      )
                    "
                  />
                </label>

                <label>
                  <span
                    class="mb-1.5 block text-xs text-foreground/60"
                  >
                    {{ $t('catalog.to') }}
                  </span>

                  <input
                    type="number"
                    min="1800"
                    step="1"
                    :value="draft.productionYearTo ?? ''"
                    placeholder="2020"
                    class="h-11 w-full rounded-xl border border-border bg-background px-3 text-sm outline-none transition focus:border-foreground"
                    @input="
                      updateNumber(
                        'productionYearTo',
                        $event,
                      )
                    "
                  />
                </label>
              </div>
            </section>

            <section>
              <h3 class="mb-3 text-sm font-semibold">
                {{ $t('catalog.country') }}
              </h3>

              <div class="relative mb-3">
                <MagnifyingGlassIcon
                  class="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-foreground/50"
                />

                <input
                  v-model="countrySearch"
                  type="search"
                  :placeholder="$t('catalog.searchCountry')"
                  class="h-11 w-full rounded-xl border border-border bg-background pl-9 pr-3 text-sm outline-none transition focus:border-foreground"
                />
              </div>

              <div
                class="max-h-44 space-y-1 overflow-y-auto pr-1"
              >
                <button
                  v-for="option in filteredCountryOptions"
                  :key="option.value"
                  type="button"
                  class="flex w-full items-center justify-between rounded-xl px-3 py-2.5 text-left text-sm transition hover:bg-muted"
                  :disabled="
                    isLocked(
                      'countries',
                      option.value,
                    )
                  "
                  @click="
                    toggleValue(
                      'countries',
                      option.value,
                    )
                  "
                >
                  <span>
                    {{ option.label }}
                  </span>

                  <span
                    class="flex h-5 w-5 items-center justify-center rounded border"
                    :class="
                      isSelected(
                        'countries',
                        option.value,
                      )
                        ? 'border-foreground bg-foreground text-background'
                        : 'border-border'
                    "
                  >
                    <CheckIcon
                      v-if="
                        isSelected(
                          'countries',
                          option.value,
                        )
                      "
                      class="h-3.5 w-3.5"
                    />
                  </span>
                </button>
              </div>
            </section>

            <section>
              <h3 class="mb-3 text-sm font-semibold">
                {{ $t('catalog.quickFilters') }}
              </h3>

              <div class="space-y-2">
                <label
                  class="flex cursor-pointer items-center justify-between rounded-xl border border-border px-4 py-3"
                >
                  <span class="text-sm">
                    {{ $t('catalog.vintageOnly') }}
                  </span>

                  <input
                    v-model="draft.vintageOnly"
                    type="checkbox"
                    class="h-4 w-4 accent-foreground"
                    :disabled="locked.vintageOnly === true"
                  />
                </label>

                <label
                  class="flex cursor-pointer items-center justify-between rounded-xl border border-border px-4 py-3"
                >
                  <span class="text-sm">
                    {{ $t('catalog.hasMeasurements') }}
                  </span>

                  <input
                    v-model="draft.hasMeasurements"
                    type="checkbox"
                    class="h-4 w-4 accent-foreground"
                  />
                </label>

                <label
                  class="flex cursor-pointer items-center justify-between rounded-xl border border-border px-4 py-3"
                >
                  <span class="text-sm">
                    {{ $t('catalog.hasProofOfOrigin') }}
                  </span>

                  <input
                    v-model="draft.hasProofOfOrigin"
                    type="checkbox"
                    class="h-4 w-4 accent-foreground"
                  />
                </label>

                <label
                  class="flex cursor-pointer items-center justify-between rounded-xl border border-border px-4 py-3"
                >
                  <span class="text-sm">
                    {{ $t('catalog.endingSoon') }}
                  </span>

                  <input
                    v-model="draft.endingSoon"
                    type="checkbox"
                    class="h-4 w-4 accent-foreground"
                  />
                </label>

                <label
                  class="flex cursor-pointer items-center justify-between rounded-xl border border-border px-4 py-3"
                >
                  <span class="text-sm">
                    {{ $t('catalog.newlyListed') }}
                  </span>

                  <input
                    v-model="draft.newlyListed"
                    type="checkbox"
                    class="h-4 w-4 accent-foreground"
                  />
                </label>
              </div>
            </section>
          </div>

          <footer
            class="grid grid-cols-[auto_1fr] gap-3 border-t border-border bg-background px-5 py-4"
          >
            <button
              type="button"
              class="rounded-full px-4 py-3 text-sm font-medium underline-offset-4 transition hover:underline"
              @click="clearDraft"
            >
              {{ $t('common.clearAll') }}
            </button>

            <button
              type="button"
              class="rounded-full bg-foreground px-5 py-3 text-sm font-semibold text-background transition hover:opacity-90"
              @click="applyFilters"
            >
              {{ $t('common.showResults') }}
            </button>
          </footer>
        </aside>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.catalog-drawer-enter-active,
.catalog-drawer-leave-active {
  transition: opacity 180ms ease;
}

.catalog-drawer-enter-active aside,
.catalog-drawer-leave-active aside {
  transition: transform 220ms ease;
}

.catalog-drawer-enter-from,
.catalog-drawer-leave-to {
  opacity: 0;
}

.catalog-drawer-enter-from aside,
.catalog-drawer-leave-to aside {
  transform: translateX(-100%);
}
</style>
