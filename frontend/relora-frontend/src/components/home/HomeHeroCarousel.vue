<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'

import { useLocalePath } from '@/composables/useLocalePath'
import {
  Carousel,
  CarouselContent,
  CarouselItem,
} from '@/components/ui/carousel'
import type { UnwrapRefCarouselApi } from '@/components/ui/carousel/interface'

defineProps<{
  username?: string
}>()

const localePath = useLocalePath()
const carouselApi = ref<UnwrapRefCarouselApi>()
const selectedIndex = ref(0)
const isPaused = ref(false)

const slides = [
  {
    titleKey: 'hero.slides.discover.title',
    descriptionKey: 'hero.slides.discover.description',
    primaryKey: 'hero.slides.discover.primary',
    primaryTo: '/catalog',
    secondaryKey: 'hero.slides.discover.secondary',
    secondaryTo: '/designers',
  },
  {
    titleKey: 'hero.slides.find.title',
    descriptionKey: 'hero.slides.find.description',
    primaryKey: 'hero.slides.find.primary',
    primaryTo: '/catalog?newlyListed=true&sort=NewlyListed',
  },
  {
    titleKey: 'hero.slides.sell.title',
    descriptionKey: 'hero.slides.sell.description',
    primaryKey: 'hero.slides.sell.primary',
    primaryTo: '/sell',
  },
]

let autoplayTimer: ReturnType<typeof setTimeout> | undefined
let reducedMotionQuery: MediaQueryList | undefined

function clearAutoplay() {
  if (autoplayTimer) {
    clearTimeout(autoplayTimer)
    autoplayTimer = undefined
  }
}

function scheduleAutoplay(delay = 5000) {
  clearAutoplay()

  if (isPaused.value || reducedMotionQuery?.matches || document.hidden) return

  autoplayTimer = setTimeout(() => {
    carouselApi.value?.scrollNext()
    scheduleAutoplay()
  }, delay)
}

function syncSelectedIndex() {
  selectedIndex.value = carouselApi.value?.selectedScrollSnap() ?? 0
}

function handleInit(api: UnwrapRefCarouselApi) {
  carouselApi.value = api
  syncSelectedIndex()
  api?.on('select', syncSelectedIndex)
  api?.on('pointerDown', pauseAutoplay)
  api?.on('pointerUp', resumeAfterInteraction)
  scheduleAutoplay()
}

function selectSlide(index: number) {
  carouselApi.value?.scrollTo(index)
  scheduleAutoplay(12000)
}

function pauseAutoplay() {
  isPaused.value = true
  clearAutoplay()
}

function resumeAutoplay() {
  isPaused.value = false
  scheduleAutoplay()
}

function resumeAfterInteraction() {
  isPaused.value = false
  scheduleAutoplay(12000)
}

function handleVisibilityChange() {
  if (document.hidden) clearAutoplay()
  else scheduleAutoplay()
}

onMounted(() => {
  reducedMotionQuery = window.matchMedia('(prefers-reduced-motion: reduce)')
  document.addEventListener('visibilitychange', handleVisibilityChange)
})

onBeforeUnmount(() => {
  clearAutoplay()
  document.removeEventListener('visibilitychange', handleVisibilityChange)
  carouselApi.value?.off('select', syncSelectedIndex)
  carouselApi.value?.off('pointerDown', pauseAutoplay)
  carouselApi.value?.off('pointerUp', resumeAfterInteraction)
})
</script>

<template>
  <section
    class="relative overflow-hidden rounded-[30px] border bg-foreground/[0.035]"
    aria-label="Relora"
    @mouseenter="pauseAutoplay"
    @mouseleave="resumeAutoplay"
    @focusin="pauseAutoplay"
    @focusout="resumeAutoplay"
  >
    <Carousel :opts="{ loop: true, align: 'start' }" @init-api="handleInit">
      <CarouselContent class="ml-0">
        <CarouselItem
          v-for="(slide, index) in slides"
          :key="slide.titleKey"
          class="pl-0"
          :aria-label="$t('hero.slideLabel', { current: index + 1, total: slides.length })"
        >
          <div class="flex min-h-[330px] flex-col justify-between px-6 py-8 sm:min-h-[360px] sm:px-10 sm:py-10 lg:px-14">
            <div>
              <p class="text-[11px] font-medium uppercase tracking-[0.24em] text-foreground/50">Relora</p>
              <p v-if="username" class="mt-3 text-xs text-foreground/55">
                {{ $t('hero.welcome', { name: username }) }}
              </p>
            </div>

            <div class="max-w-3xl py-8">
              <component
                :is="index === 0 ? 'h1' : 'h2'"
                class="text-4xl font-semibold leading-[1.04] tracking-[-0.04em] sm:text-5xl lg:text-6xl"
              >
                {{ $t(slide.titleKey) }}
              </component>
              <p class="mt-5 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base">
                {{ $t(slide.descriptionKey) }}
              </p>
              <div class="mt-7 flex flex-wrap gap-3">
                <RouterLink
                  :to="localePath(slide.primaryTo)"
                  class="inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-5 text-sm font-medium text-background transition hover:opacity-85 focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
                >
                  {{ $t(slide.primaryKey) }}
                </RouterLink>
                <RouterLink
                  v-if="slide.secondaryKey && slide.secondaryTo"
                  :to="localePath(slide.secondaryTo)"
                  class="inline-flex min-h-11 items-center justify-center rounded-full border bg-background px-5 text-sm font-medium transition hover:bg-foreground hover:text-background focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
                >
                  {{ $t(slide.secondaryKey) }}
                </RouterLink>
              </div>
            </div>
          </div>
        </CarouselItem>
      </CarouselContent>
    </Carousel>

    <div class="absolute bottom-5 right-6 z-10 flex items-center gap-2 sm:bottom-7 sm:right-10" role="tablist" :aria-label="$t('home.hero.navigationLabel')">
      <button
        v-for="(_, index) in slides"
        :key="index"
        type="button"
        role="tab"
        class="h-2.5 rounded-full transition-all focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 motion-reduce:transition-none"
        :class="selectedIndex === index ? 'w-7 bg-foreground' : 'w-2.5 bg-foreground/25 hover:bg-foreground/45'"
        :aria-selected="selectedIndex === index"
        :aria-label="$t('home.hero.goToSlide', { number: index + 1 })"
        @click="selectSlide(index)"
      />
    </div>
  </section>
</template>
