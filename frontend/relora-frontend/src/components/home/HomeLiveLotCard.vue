<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink } from 'vue-router'
import { useI18n } from 'vue-i18n'

import { useLocalePath } from '@/composables/useLocalePath'
import { formatLotNumber, formatPrice, formatTimeLeft, secondsLeft } from '@/app/helpers/homeHelpers'
import type { HomeLiveLot } from '@/types/home'

// live auction card: featured, row or small
const props = defineProps<{
  lot: HomeLiveLot
  index: number
  variant: 'featured' | 'row' | 'small'
  now: number
}>()

const { locale } = useI18n()
const localePath = useLocalePath()

const link = computed(() => {
  return props.lot.isDemo ? localePath('/catalog') : localePath(`/lots/${props.lot.lotId}`)
})

const price = computed(() => formatPrice(props.lot.currentPrice, props.lot.currency, locale.value))
const timeLeft = computed(() => formatTimeLeft(props.lot.endsAt, props.now))
const seconds = computed(() => secondsLeft(props.lot.endsAt, props.now))
const isEnded = computed(() => !props.lot.isDemo && seconds.value === 0)
const isClosingSoon = computed(() => !isEnded.value && seconds.value < 3600)
</script>

<template>
  <RouterLink
    :to="link"
    class="live-card group block focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-[var(--auction-accent)]"
    :class="variant === 'featured' ? 'live-card-featured' : ''"
  >
    <article :class="variant === 'row' ? 'grid grid-cols-[40%_minmax(0,1fr)] items-end gap-4 sm:gap-5' : ''">
      <div class="live-card-media relative aspect-[4/5] overflow-hidden rounded-[6px] bg-[#efefec] dark:bg-neutral-900">
        <img
          v-if="lot.imageUrl"
          :src="lot.imageUrl"
          :alt="`${lot.brand} ${lot.title}`"
          loading="lazy"
          decoding="async"
          class="live-card-image h-full w-full object-cover transition-transform duration-[1.2s] ease-[cubic-bezier(0.22,1,0.36,1)] group-hover:scale-[1.06]"
        />

        <span
          v-if="isClosingSoon"
          class="absolute left-3 top-3 inline-flex items-center gap-2 bg-white px-2.5 py-1.5 font-mono text-[10px] uppercase tracking-[0.18em] text-[#111111]"
        >
          <span class="h-1.5 w-1.5 rounded-full bg-[var(--auction-accent)]" />
          {{ $t('home.liveAuctions.closingSoon') }}
        </span>
      </div>

      <div :class="variant === 'row' ? 'pb-1' : 'pt-4'">
        <div class="flex items-center justify-between gap-3 font-mono text-[10px] uppercase tracking-[0.22em] text-foreground/55">
          <span class="truncate">{{ $t('home.liveHero.lot') }} {{ formatLotNumber(index) }} — {{ lot.brand }}</span>
          <span class="shrink-0 transition-transform duration-500 group-hover:rotate-45" aria-hidden="true">↗</span>
        </div>

        <h3
          class="mt-2 font-medium leading-[1.05] tracking-[-0.03em] text-foreground"
          :class="variant === 'featured' ? 'text-2xl sm:text-3xl lg:text-[2.6rem]' : 'line-clamp-2 text-lg'"
        >
          {{ lot.title }}
        </h3>

        <div class="mt-4 flex items-end justify-between gap-4 border-t border-foreground/10 pt-3">
          <div>
            <p class="font-mono text-[10px] uppercase tracking-[0.22em] text-foreground/45">
              {{ $t('home.liveHero.currentBid') }}
            </p>
            <p
              class="mt-1 font-semibold tracking-[-0.03em] tabular-nums text-foreground"
              :class="variant === 'featured' ? 'text-2xl lg:text-3xl' : 'text-lg'"
            >
              {{ price }}
            </p>
          </div>

          <div class="text-right">
            <p class="font-mono text-[10px] uppercase tracking-[0.22em] text-foreground/45">
              {{ $t('home.liveHero.timeLeft') }}
            </p>
            <p
              class="mt-1 font-mono text-sm tabular-nums"
              :class="isClosingSoon ? 'text-[var(--auction-accent)]' : 'text-foreground'"
            >
              {{ isEnded ? $t('home.liveHero.ended') : timeLeft }}
            </p>
          </div>
        </div>
      </div>
    </article>
  </RouterLink>
</template>
