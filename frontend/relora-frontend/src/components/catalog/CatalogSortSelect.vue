<script setup lang="ts">
import { sortOptions } from '@/config/catalogOptions'
import type { CatalogSort } from '@/types/catalog'

withDefaults(
  defineProps<{
    modelValue: CatalogSort
    disabled?: boolean
  }>(),
  {
    disabled: false,
  },
)

const emit = defineEmits<{
  'update:modelValue': [value: CatalogSort]
}>()

function handleChange(event: Event): void {
  const target = event.target as HTMLSelectElement

  emit(
    'update:modelValue',
    target.value as CatalogSort,
  )
}
</script>

<template>
  <label class="relative block">
    <span class="sr-only">
      {{ $t('common.sortCatalog') }}
    </span>

    <select
      :value="modelValue"
      :disabled="disabled"
      class="h-11 min-w-[190px] cursor-pointer appearance-none rounded-full border border-border bg-background py-2 pl-4 pr-10 text-sm font-medium outline-none transition hover:border-foreground/40 focus:border-foreground disabled:cursor-not-allowed disabled:opacity-50"
      @change="handleChange"
    >
      <option
        v-for="option in sortOptions"
        :key="option.value"
        :value="option.value"
      >
        {{
          $t('common.sortBy', {
            label: $t(`catalog.sort.${option.value}`),
          })
        }}
      </option>
    </select>

    <svg
      aria-hidden="true"
      viewBox="0 0 20 20"
      fill="currentColor"
      class="pointer-events-none absolute right-4 top-1/2 h-4 w-4 -translate-y-1/2 text-foreground/60"
    >
      <path
        fill-rule="evenodd"
        d="M5.22 7.22a.75.75 0 011.06 0L10 10.94l3.72-3.72a.75.75 0 111.06 1.06l-4.25 4.25a.75.75 0 01-1.06 0L5.22 8.28a.75.75 0 010-1.06z"
        clip-rule="evenodd"
      />
    </svg>
  </label>
</template>
