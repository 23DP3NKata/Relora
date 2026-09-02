<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink } from 'vue-router'
import { useI18n } from 'vue-i18n'

import { useLocalePath } from '@/composables/useLocalePath'
import type { LotPreview } from '@/types/lot'

const props = withDefaults(
  defineProps<{
    lot: LotPreview
    badge?: string
    price?: number | null
    currency?: string | null
  }>(),
  {
    badge: '',
    price: null,
    currency: null,
  },
)

const localePath = useLocalePath()
const { locale } = useI18n()

const images = computed(() => {
  return (props.lot.media ?? [])
    .map((media) => media.url || media.key)
    .filter((url): url is string => Boolean(url))
    .slice(0, 2)
})

const primaryImage = computed(() => images.value[0] ?? '')
const secondaryImage = computed(() => images.value[1] ?? '')

const displayPrice = computed(() => {
  return props.price ?? props.lot.price
})

const displayCurrency = computed(() => {
  return props.currency ?? props.lot.currency ?? 'EUR'
})

const formattedPrice = computed(() => {
  try {
    return new Intl.NumberFormat(locale.value, {
      style: 'currency',
      currency: displayCurrency.value,
      minimumFractionDigits: 0,
      maximumFractionDigits: 2,
    }).format(displayPrice.value)
  } catch {
    return `${displayPrice.value.toLocaleString(locale.value)} ${displayCurrency.value}`
  }
})
</script>

<template>
  <RouterLink
    :to="localePath(`/lots/${lot.id}`)"
    class="group block min-w-0"
  >
    <article class="h-full overflow-hidden rounded-[22px] border bg-background">
      <div class="relative aspect-[4/5] overflow-hidden bg-neutral-100 dark:bg-neutral-900">
        <img
          v-if="primaryImage"
          :src="primaryImage"
          :alt="lot.title"
          loading="lazy"
          class="h-full w-full object-cover transition duration-500 group-hover:scale-[1.025]"
          :class="{ 'group-hover:opacity-0': secondaryImage }"
        />

        <img
          v-if="secondaryImage"
          :src="secondaryImage"
          :alt="lot.title"
          loading="lazy"
          class="absolute inset-0 h-full w-full object-cover opacity-0 transition duration-500 group-hover:scale-[1.025] group-hover:opacity-100"
        />

        <div
          v-if="!primaryImage"
          class="flex h-full items-center justify-center px-4 text-center text-xs text-foreground/45"
        >
          No image
        </div>

        <span
          v-if="badge"
          class="absolute left-3 top-3 rounded-full bg-black/80 px-3 py-1.5 text-[11px] font-medium text-white backdrop-blur"
        >
          {{ badge }}
        </span>
      </div>

      <div class="p-4">
        <p class="truncate text-[11px] font-medium uppercase tracking-[0.18em] text-foreground/55">
          {{ lot.brand }}
        </p>

        <h3 class="mt-2 line-clamp-2 min-h-10 text-sm font-medium leading-5 text-foreground sm:text-base">
          {{ lot.title }}
        </h3>

        <div class="mt-4 flex items-end justify-between gap-3">
          <p class="text-sm font-semibold sm:text-base">
            {{ formattedPrice }}
          </p>

          <p
            v-if="lot.sizeName"
            class="truncate text-xs text-foreground/50"
          >
            {{ lot.sizeName }}
          </p>
        </div>
      </div>
    </article>
  </RouterLink>
</template>
