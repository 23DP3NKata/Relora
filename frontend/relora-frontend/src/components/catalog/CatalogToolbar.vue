<script setup lang="ts">
import { computed } from 'vue'
import {
  FunnelIcon,
  XMarkIcon,
} from '@heroicons/vue/24/outline'

import CatalogSortSelect from '@/components/catalog/CatalogSortSelect.vue'

import type {
  CatalogActiveFilter,
  CatalogSort,
} from '@/types/catalog'

const props = withDefaults(
  defineProps<{
    activeFilters: CatalogActiveFilter[]
    sort: CatalogSort
    disabled?: boolean
  }>(),
  {
    disabled: false,
  },
)

const emit = defineEmits<{
  'open-filters': []
  'update:sort': [value: CatalogSort]
  'remove-filter': [filter: CatalogActiveFilter]
  'clear-all': []
}>()

const removableFilters = computed(() => {
  return props.activeFilters.filter(
    filter => filter.removable,
  )
})
</script>

<template>
  <div
    class="mb-7 flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between"
  >
    <div class="flex min-w-0 flex-1 flex-wrap items-center gap-2">
      <button
        type="button"
        class="inline-flex h-10 items-center gap-2 rounded-full border border-border bg-background px-4 text-sm font-medium transition hover:border-foreground/40 hover:bg-muted/40 disabled:cursor-not-allowed disabled:opacity-50"
        :disabled="disabled"
        @click="emit('open-filters')"
      >
        <FunnelIcon class="h-4 w-4" />

        {{ $t('common.filters') }}

        <span
          v-if="activeFilters.length > 0"
          class="inline-flex h-5 min-w-5 items-center justify-center rounded-full bg-foreground px-1.5 text-[11px] text-background"
        >
          {{ activeFilters.length }}
        </span>
      </button>

      <div
        v-for="filter in activeFilters"
        :key="filter.id"
        class="inline-flex h-10 items-center gap-1.5 rounded-full border border-border bg-muted/30 px-3 text-sm"
      >
        <span>
          {{ filter.label }}
        </span>

        <button
          v-if="filter.removable"
          type="button"
          class="rounded-full p-0.5 text-foreground/60 transition hover:bg-foreground/10 hover:text-foreground"
          :aria-label="$t('catalog.removeFilter', { label: filter.label })"
          @click="emit('remove-filter', filter)"
        >
          <XMarkIcon class="h-3.5 w-3.5" />
        </button>
      </div>

      <button
        v-if="removableFilters.length > 0"
        type="button"
        class="px-2 text-sm text-foreground/60 underline-offset-4 transition hover:text-foreground hover:underline"
        @click="emit('clear-all')"
      >
        {{ $t('common.clearAll') }}
      </button>
    </div>

    <CatalogSortSelect
      :model-value="sort"
      :disabled="disabled"
      class="w-full lg:w-auto"
      @update:model-value="
        emit('update:sort', $event)
      "
    />
  </div>
</template>
