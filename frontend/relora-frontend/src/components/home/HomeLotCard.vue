<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink } from 'vue-router'
import { useI18n } from 'vue-i18n'

import { useLocalePath } from '@/composables/useLocalePath'
import { formatPrice } from '@/app/helpers/homeHelpers'
import { buildMediaUrl } from '@/shared/mediaUrl'
import type { LotPreview } from '@/types/lot'

// lot card for homepage lists, second photo shows on hover
const props = defineProps<{
  lot: LotPreview
}>()

const localePath = useLocalePath()
const { locale } = useI18n()

const images = computed(() => {
  return (props.lot.media ?? [])
    .slice(0, 2)
    .map((media) => buildMediaUrl(media))
})

const primaryImage = computed(() => images.value[0] ?? '')
const secondaryImage = computed(() => images.value[1] ?? '')

const price = computed(() => formatPrice(props.lot.price, props.lot.currency ?? 'EUR', locale.value))
</script>

<template>
  <RouterLink
    :to="localePath(`/lots/${lot.id}`)"
    class="group block min-w-0 focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-[var(--auction-accent)]"
  >
    <article>
      <div class="relative aspect-[4/5] overflow-hidden rounded-[6px] bg-[#efefec] dark:bg-neutral-900">
        <img
          v-if="primaryImage"
          :src="primaryImage"
          :alt="lot.title"
          loading="lazy"
          decoding="async"
          class="h-full w-full object-cover transition duration-700 ease-[cubic-bezier(0.22,1,0.36,1)] group-hover:scale-[1.04]"
          :class="{ 'group-hover:opacity-0': secondaryImage }"
        />

        <img
          v-if="secondaryImage"
          :src="secondaryImage"
          :alt="lot.title"
          loading="lazy"
          decoding="async"
          class="absolute inset-0 h-full w-full object-cover opacity-0 transition duration-700 ease-[cubic-bezier(0.22,1,0.36,1)] group-hover:scale-[1.04] group-hover:opacity-100"
        />
      </div>

      <div class="pt-4">
        <div class="flex items-center justify-between gap-3 font-mono text-[10px] uppercase tracking-[0.22em] text-foreground/55">
          <span class="truncate">{{ lot.brand }}</span>
          <span class="shrink-0 transition-transform duration-500 group-hover:rotate-45" aria-hidden="true">↗</span>
        </div>

        <h3 class="mt-2 line-clamp-2 text-base font-medium leading-tight tracking-[-0.02em] text-foreground">
          {{ lot.title }}
        </h3>

        <div class="mt-3 flex items-end justify-between gap-3 border-t border-foreground/10 pt-3">
          <p class="font-semibold tabular-nums tracking-[-0.02em] text-foreground">
            {{ price }}
          </p>
          <p v-if="lot.sizeName" class="truncate font-mono text-[11px] uppercase text-foreground/50">
            {{ lot.sizeName }}
          </p>
        </div>
      </div>
    </article>
  </RouterLink>
</template>
