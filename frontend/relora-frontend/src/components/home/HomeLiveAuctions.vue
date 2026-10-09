<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'
import gsap from 'gsap'
import { ScrollTrigger } from 'gsap/ScrollTrigger'

import HomeLiveLotCard from '@/components/home/HomeLiveLotCard.vue'
import MagneticButton from '@/components/home/MagneticButton.vue'
import { useLocalePath } from '@/composables/useLocalePath'
import { useNow } from '@/composables/useNow'
import type { HomeLiveLot } from '@/types/home'

gsap.registerPlugin(ScrollTrigger)

// live auctions section: 1 big card, 2 row cards, 3 small cards
const props = defineProps<{
  lots: HomeLiveLot[]
}>()

const localePath = useLocalePath()
const now = useNow()
const section = ref<HTMLElement | null>(null)

// first lot is in hero, so numbers start from 2
const featured = computed(() => props.lots[0])
const sideLots = computed(() => props.lots.slice(1, 3))
const bottomLots = computed(() => props.lots.slice(3, 6))

// small vertical offsets for small cards
const bottomOffsets = ['lg:mt-0', 'lg:mt-20', 'lg:mt-10']

let mm: gsap.MatchMedia | undefined

onMounted(() => {
  if (!section.value) return

  mm = gsap.matchMedia(section.value)

  mm.add('(prefers-reduced-motion: no-preference)', () => {
    gsap.from('.live-title-inner', {
      yPercent: 110,
      duration: 1.3,
      ease: 'expo.out',
      scrollTrigger: { trigger: '.live-title', start: 'top 85%' },
    })

    // cards reveal with mask and slight skew
    gsap.set('.live-card', { y: 60, skewY: 3, autoAlpha: 0 })
    gsap.set('.live-card-media', { clipPath: 'inset(100% 0% 0% 0%)' })
    gsap.set('.live-card-image', { scale: 1.3 })

    ScrollTrigger.batch('.live-card', {
      start: 'top 88%',
      once: true,
      onEnter: (cards) => {
        const media = cards.map((card) => card.querySelector('.live-card-media'))
        const images = cards.map((card) => card.querySelector('.live-card-image')).filter(Boolean)

        gsap.to(cards, { y: 0, skewY: 0, autoAlpha: 1, duration: 1.2, ease: 'expo.out', stagger: 0.1 })
        gsap.to(media, { clipPath: 'inset(0% 0% 0% 0%)', duration: 1.4, ease: 'expo.out', stagger: 0.1 })
        // clear scale so css hover works again
        gsap.to(images, { scale: 1, duration: 1.8, ease: 'expo.out', stagger: 0.1, clearProps: 'scale' })
      },
    })
  })
})

onBeforeUnmount(() => {
  mm?.revert()
})
</script>

<template>
  <section
    ref="section"
    class="live-auctions"
    aria-labelledby="live-auctions-title"
  >
    <header class="grid gap-6 border-t border-foreground/10 pt-6 lg:grid-cols-12 lg:items-end lg:gap-10">
      <div class="lg:col-span-8">
        <p class="font-mono text-[10px] uppercase tracking-[0.24em] text-foreground/55 sm:text-[11px]">
          02 — {{ $t('home.liveAuctions.eyebrow') }}
        </p>
        <h2
          id="live-auctions-title"
          class="live-title mt-4 overflow-hidden pb-[0.04em] text-[clamp(2.9rem,9vw,7.5rem)] font-semibold uppercase leading-[0.86] tracking-[-0.065em] text-foreground"
        >
          <span class="live-title-inner block">{{ $t('home.liveAuctions.title') }}</span>
        </h2>
      </div>

      <div class="flex flex-col items-start gap-5 lg:col-span-4 lg:pb-2">
        <p class="max-w-sm text-sm leading-6 text-foreground/65">
          {{ $t('home.liveAuctions.description') }}
        </p>
        <MagneticButton :to="localePath('/catalog')" variant="light">
          {{ $t('home.liveAuctions.viewAll') }}
        </MagneticButton>
      </div>
    </header>

    <!-- no live auctions yet -->
    <div
      v-if="!featured"
      class="mt-10 grid gap-6 border-y border-foreground/10 py-12 lg:mt-14 lg:grid-cols-12 lg:gap-10 lg:py-16"
    >
      <p class="text-[clamp(1.6rem,3vw,2.5rem)] font-medium leading-[1.1] tracking-[-0.03em] text-foreground lg:col-span-7">
        {{ $t('home.liveAuctions.emptyTitle') }}
      </p>
      <p class="max-w-sm text-sm leading-6 text-foreground/65 lg:col-span-5 lg:self-end">
        {{ $t('home.liveAuctions.emptyText') }}
      </p>
    </div>

    <div v-else class="mt-10 grid gap-x-8 gap-y-10 lg:mt-14 lg:grid-cols-12">
      <div class="lg:col-span-6">
        <HomeLiveLotCard :lot="featured" :index="1" variant="featured" :now="now" />
      </div>

      <div class="flex flex-col gap-8 lg:col-span-6 lg:justify-between lg:pt-24">
        <HomeLiveLotCard
          v-for="(lot, i) in sideLots"
          :key="lot.lotId"
          :lot="lot"
          :index="i + 2"
          variant="row"
          :now="now"
        />
      </div>
    </div>

    <!-- horizontal scroll on mobile -->
    <div
      v-if="featured && bottomLots.length"
      class="-mx-4 mt-12 flex snap-x snap-mandatory gap-4 overflow-x-auto px-4 pb-2 lg:mx-0 lg:mt-20 lg:grid lg:grid-cols-12 lg:gap-8 lg:overflow-visible lg:px-0"
    >
      <div
        v-for="(lot, i) in bottomLots"
        :key="lot.lotId"
        class="w-[68%] shrink-0 snap-start sm:w-[42%] lg:col-span-4 lg:w-auto"
        :class="bottomOffsets[i]"
      >
        <HomeLiveLotCard :lot="lot" :index="i + 4" variant="small" :now="now" />
      </div>
    </div>
  </section>
</template>
