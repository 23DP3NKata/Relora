<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { RouterLink } from 'vue-router'
import { useI18n } from 'vue-i18n'
import gsap from 'gsap'
import { ScrollTrigger } from 'gsap/ScrollTrigger'
import { animate, press } from 'motion'

import MagneticButton from '@/components/home/MagneticButton.vue'
import { useLocalePath } from '@/composables/useLocalePath'
import { useNow } from '@/composables/useNow'
import { heroConfig } from '@/config/homeHero'
import { formatLotNumber, formatPrice, formatTimeLeft, secondsLeft } from '@/app/helpers/homeHelpers'
import type { HomeLiveLot } from '@/types/home'

gsap.registerPlugin(ScrollTrigger)

// homepage hero
// GSAP: intro, cursor parallax, scroll
// Motion: small spring interactions
const props = defineProps<{
  lot: HomeLiveLot
}>()

const { locale } = useI18n()
const localePath = useLocalePath()
const now = useNow()

const hero = ref<HTMLElement | null>(null)
const priceEl = ref<HTMLElement | null>(null)
const mediaFrame = ref<HTMLElement | null>(null)
const video = ref<HTMLVideoElement | null>(null)
const cursorLabel = ref<HTMLElement | null>(null)
const showCursorLabel = ref(false)
const priceJustChanged = ref(false)

const lotNumber = formatLotNumber(0)

const lotLink = computed(() => {
  return props.lot.isDemo ? localePath('/catalog') : localePath(`/lots/${props.lot.lotId}`)
})

const price = computed(() => formatPrice(props.lot.currentPrice, props.lot.currency, locale.value))
const timeLeft = computed(() => formatTimeLeft(props.lot.endsAt, now.value))
const isEnded = computed(() => !props.lot.isDemo && secondsLeft(props.lot.endsAt, now.value) === 0)
const isClosingSoon = computed(() => !isEnded.value && secondsLeft(props.lot.endsAt, now.value) < 3600)

// video only for pinned (or demo) lot
const isMobile = window.matchMedia('(max-width: 1023px)').matches
const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches

const hasVideo = computed(() => {
  const isPinned = props.lot.lotId === heroConfig.pinnedLotId || props.lot.isDemo
  return Boolean(heroConfig.videoMp4) && isPinned
})

const videoMp4 = isMobile && heroConfig.videoMp4Mobile ? heroConfig.videoMp4Mobile : heroConfig.videoMp4

// demo image has no background, don't crop it
const isCutout = computed(() => props.lot.imageUrl === heroConfig.poster)

let mm: gsap.MatchMedia | undefined
let floatTween: gsap.core.Tween | undefined
let observer: IntersectionObserver | undefined
let stopPress: (() => void) | undefined

function playHeavyStuff() {
  floatTween?.play()

  if (video.value && !reduceMotion) {
    video.value.muted = true
    video.value.play().catch(() => {})
  }
}

function pauseHeavyStuff() {
  floatTween?.pause()
  video.value?.pause()
}

function setupAnimations() {
  if (!hero.value) return

  mm = gsap.matchMedia(hero.value)

  // intro, mobile and desktop
  mm.add('(prefers-reduced-motion: no-preference)', () => {
    const intro = gsap.timeline({ defaults: { ease: 'expo.out', duration: 1.4 } })

    intro
      .from('.hero-line-inner', { yPercent: 110, stagger: 0.12 })
      .from('.hero-media-frame', { clipPath: 'inset(100% 0% 0% 0%)', duration: 1.6 }, 0.1)
      .from('.hero-media-content', { scale: 1.25, duration: 2 }, 0.1)
      .from('.hero-rule', { scaleX: 0, transformOrigin: 'left center', stagger: 0.08 }, 0.6)
      .from('.hero-reveal', { y: 24, autoAlpha: 0, stagger: 0.06, duration: 1 }, 0.7)
  })

  // desktop only: parallax, float, scroll out
  mm.add('(min-width: 1024px) and (prefers-reduced-motion: no-preference)', () => {
    const layers = gsap.utils.toArray<HTMLElement>('[data-depth]')

    const movers = layers.map((layer) => ({
      depth: Number(layer.dataset.depth),
      x: gsap.quickTo(layer, 'x', { duration: 1.2, ease: 'power3.out' }),
      y: gsap.quickTo(layer, 'y', { duration: 1.2, ease: 'power3.out' }),
    }))

    const tiltX = gsap.quickTo('.hero-media-float', 'rotationY', { duration: 1.4, ease: 'power3.out' })
    const tiltY = gsap.quickTo('.hero-media-float', 'rotationX', { duration: 1.4, ease: 'power3.out' })
    const labelX = gsap.quickTo(cursorLabel.value, 'x', { duration: 0.5, ease: 'power3.out' })
    const labelY = gsap.quickTo(cursorLabel.value, 'y', { duration: 0.5, ease: 'power3.out' })

    function onPointerMove(event: PointerEvent) {
      const rect = hero.value!.getBoundingClientRect()
      // -1..1 from hero center
      const nx = ((event.clientX - rect.left) / rect.width - 0.5) * 2
      const ny = ((event.clientY - rect.top) / rect.height - 0.5) * 2

      for (const mover of movers) {
        mover.x(nx * mover.depth * 40)
        mover.y(ny * mover.depth * 40)
      }

      tiltX(nx * 6)
      tiltY(-ny * 4)
      labelX(event.clientX - rect.left)
      labelY(event.clientY - rect.top)
    }

    function onPointerLeave() {
      for (const mover of movers) {
        mover.x(0)
        mover.y(0)
      }

      tiltX(0)
      tiltY(0)
    }

    hero.value!.addEventListener('pointermove', onPointerMove)
    hero.value!.addEventListener('pointerleave', onPointerLeave)

    gsap.set('.hero-media-float', { transformPerspective: 1200 })

    floatTween = gsap.to('.hero-media-float', {
      y: -14,
      duration: 3.2,
      ease: 'sine.inOut',
      repeat: -1,
      yoyo: true,
    })

    // arch radius = half of lot width
    const archRadius = () => (hero.value!.querySelector('.hero-media-frame') as HTMLElement).offsetWidth / 2

    // layers leave at different speeds, arch turns into a card
    gsap.timeline({
      defaults: { ease: 'none' },
      scrollTrigger: {
        trigger: hero.value,
        start: 'top top',
        end: 'bottom top',
        scrub: 0.6,
        invalidateOnRefresh: true,
      },
    })
      .to('.hero-line-1', { xPercent: -18, autoAlpha: 0.1 }, 0)
      .to('.hero-line-2', { xPercent: 18, autoAlpha: 0.1 }, 0)
      .to('.hero-meta-left', { yPercent: -80, autoAlpha: 0 }, 0)
      .to('.hero-meta-right', { yPercent: -160, autoAlpha: 0 }, 0)
      .to('.hero-cta', { yPercent: -60, autoAlpha: 0 }, 0)
      .to('.hero-media', { yPercent: 14, scale: 0.9 }, 0)
      .fromTo(
        '.hero-media-frame',
        { borderRadius: () => `${archRadius()}px ${archRadius()}px 8px 8px` },
        { borderRadius: '8px 8px 8px 8px' },
        0,
      )

    return () => {
      hero.value?.removeEventListener('pointermove', onPointerMove)
      hero.value?.removeEventListener('pointerleave', onPointerLeave)
      floatTween = undefined
    }
  })
}

// highlight new price from server
watch(
  () => props.lot.currentPrice,
  async () => {
    await nextTick()
    priceJustChanged.value = true

    if (priceEl.value && !reduceMotion) {
      gsap.fromTo(priceEl.value, { yPercent: 60, autoAlpha: 0 }, { yPercent: 0, autoAlpha: 1, duration: 0.6, ease: 'expo.out' })
    }

    setTimeout(() => {
      priceJustChanged.value = false
    }, 1600)
  },
)

onMounted(() => {
  setupAnimations()

  // pause video and float when hero is off screen
  observer = new IntersectionObserver((entries) => {
    if (entries[0]?.isIntersecting) playHeavyStuff()
    else pauseHeavyStuff()
  })

  if (hero.value) observer.observe(hero.value)

  if (mediaFrame.value && !reduceMotion) {
    stopPress = press(mediaFrame.value, (element) => {
      animate(element, { scale: 0.97 }, { duration: 0.15 })
      return () => animate(element, { scale: 1 }, { type: 'spring', stiffness: 220, damping: 16 })
    })
  }
})

onBeforeUnmount(() => {
  mm?.revert()
  observer?.disconnect()
  stopPress?.()
})
</script>

<template>
  <section
    ref="hero"
    class="home-hero relative isolate bg-background"
    aria-labelledby="home-hero-title"
  >
    <!-- layer 1: background -->
    <div class="pointer-events-none absolute inset-0 -z-10 overflow-hidden" aria-hidden="true">
      <div class="hero-columns absolute inset-0 hidden lg:block" />
      <div class="absolute left-1/2 top-[18%] h-[70%] w-[70%] -translate-x-1/2 rounded-full bg-[radial-gradient(closest-side,rgba(0,0,0,0.06),transparent)] dark:bg-[radial-gradient(closest-side,rgba(255,255,255,0.07),transparent)]" />
    </div>

    <!-- layer 6: cursor label -->
    <!-- outer span moved by GSAP, inner one fades in with CSS -->
    <span
      ref="cursorLabel"
      class="pointer-events-none absolute left-0 top-0 z-40 hidden lg:block"
      aria-hidden="true"
    >
      <span
        class="block -translate-x-1/2 -translate-y-1/2 rounded-full bg-[#111111] px-4 py-2 font-mono text-[10px] uppercase tracking-[0.2em] whitespace-nowrap text-white transition-[opacity,scale] duration-300 dark:bg-white dark:text-[#111111]"
        :class="showCursorLabel ? 'scale-100 opacity-100' : 'scale-50 opacity-0'"
      >
        {{ $t('home.liveHero.viewLot') }}
      </span>
    </span>

    <!-- top meta row -->
    <div class="hero-reveal flex items-center justify-between gap-4 border-b border-foreground/10 pb-3 font-mono text-[10px] uppercase tracking-[0.24em] text-foreground/55 sm:text-[11px]">
      <span>{{ $t('home.liveHero.eyebrow') }}</span>

      <span class="inline-flex items-center gap-2">
        <span
          class="h-1.5 w-1.5 rounded-full"
          :class="lot.isDemo || isEnded ? 'bg-foreground/35' : 'hero-live-dot bg-[var(--auction-accent)]'"
        />
        <template v-if="lot.isDemo">{{ $t('home.liveHero.preview') }}</template>
        <template v-else-if="isEnded">{{ $t('home.liveHero.ended') }}</template>
        <template v-else>{{ $t('home.liveHero.live') }}</template>
      </span>
    </div>

    <!-- stacked on mobile, layered on desktop -->
    <div class="hero-stage relative flex flex-col gap-6 pt-6 lg:block lg:h-[clamp(600px,calc(100svh-250px),820px)] lg:pt-0">

      <!-- layer 3: headline -->
      <h1
        id="home-hero-title"
        class="hero-title text-[clamp(3.1rem,15vw,5.5rem)] font-semibold uppercase leading-[0.86] tracking-[-0.065em] text-foreground lg:static lg:text-[clamp(5.5rem,10.5vw,10.25rem)]"
      >
        <span class="hero-line hero-line-1 block overflow-hidden pb-[0.04em] lg:absolute lg:left-0 lg:top-[9%] lg:z-20" data-depth="0.35">
          <span class="hero-line-inner block">{{ $t('home.liveHero.titleLine1') }}</span>
        </span>
        <span class="hero-line hero-line-2 block overflow-hidden pb-[0.04em] lg:absolute lg:bottom-[7%] lg:right-0 lg:z-20 lg:text-right" data-depth="0.6">
          <span class="hero-line-inner block">{{ $t('home.liveHero.titleLine2') }}</span>
        </span>
      </h1>

      <!-- layer 2: lot video or photo -->
      <div
        class="hero-media relative mx-auto w-full max-w-[420px] lg:absolute lg:inset-x-0 lg:bottom-[5%] lg:top-[7%] lg:z-10 lg:w-[clamp(300px,29vw,430px)] lg:max-w-none"
        data-depth="0.2"
      >
        <div class="hero-media-float h-full">
          <RouterLink v-slot="{ href, navigate }" :to="lotLink" custom>
            <a
              ref="mediaFrame"
              :href="href"
              class="hero-media-frame group relative block h-[42svh] min-h-[300px] overflow-hidden bg-[#efefec] lg:h-full lg:min-h-0 dark:bg-neutral-900"
              :aria-label="`${lot.brand} ${lot.title}`"
              @click="navigate"
              @mouseenter="showCursorLabel = true"
              @mouseleave="showCursorLabel = false"
            >
              <video
                v-if="hasVideo"
                ref="video"
                class="hero-media-content h-full w-full object-cover"
                :poster="heroConfig.poster"
                muted
                loop
                playsinline
                preload="metadata"
                aria-hidden="true"
              >
                <source v-if="heroConfig.videoWebm && !isMobile" :src="heroConfig.videoWebm" type="video/webm" />
                <source :src="videoMp4" type="video/mp4" />
              </video>

              <img
                v-else
                :src="lot.imageUrl"
                :alt="`${lot.brand} ${lot.title}`"
                fetchpriority="high"
                decoding="async"
                class="hero-media-content h-full w-full"
                :class="isCutout ? 'object-contain object-bottom pt-10' : 'object-cover'"
              />

              <!-- lot details on hover (always visible on mobile) -->
              <div class="absolute inset-x-3 bottom-3 flex items-center justify-between gap-3 bg-white/90 px-4 py-3 font-mono text-[10px] uppercase tracking-[0.18em] text-[#111111] backdrop-blur-sm transition duration-500 ease-out lg:translate-y-[calc(100%+12px)] lg:opacity-0 lg:group-hover:translate-y-0 lg:group-hover:opacity-100 lg:group-focus-visible:translate-y-0 lg:group-focus-visible:opacity-100">
                <span class="truncate">
                  {{ lot.sizeName }} · {{ lot.conditionName }}
                </span>
                <span class="shrink-0">{{ $t('home.liveHero.viewLot') }} ↗</span>
              </div>
            </a>
          </RouterLink>
        </div>
      </div>

      <!-- layer 4: auction info -->
      <div class="hero-info grid grid-cols-2 gap-x-6 gap-y-5 lg:contents">
        <div class="hero-meta-left col-span-2 lg:absolute lg:left-0 lg:top-[44%] lg:z-30 lg:w-[min(24vw,300px)]" data-depth="0.9">
          <p class="hero-reveal font-mono text-[10px] uppercase tracking-[0.24em] text-foreground/55 sm:text-[11px]">
            {{ $t('home.liveHero.lot') }} {{ lotNumber }} — {{ lot.brand }}
          </p>
          <p class="hero-reveal mt-2 text-lg font-medium leading-tight tracking-[-0.02em] text-foreground lg:text-xl">
            {{ lot.title }}
          </p>
          <div class="hero-rule mt-5 hidden h-px bg-foreground/15 lg:block" />
          <p class="hero-reveal mt-5 hidden font-mono text-[10px] uppercase tracking-[0.24em] text-foreground/55 lg:block">
            {{ $t('home.liveHero.currentBid') }}
          </p>
          <p class="mt-2 hidden overflow-hidden lg:block">
            <span
              ref="priceEl"
              class="hero-reveal block text-[clamp(2.5rem,3.6vw,3.5rem)] font-semibold leading-none tracking-[-0.05em] tabular-nums transition-colors duration-700"
              :class="priceJustChanged ? 'text-[var(--auction-accent)]' : 'text-foreground'"
            >
              {{ price }}
            </span>
          </p>
        </div>

        <!-- price on mobile -->
        <div class="lg:hidden">
          <p class="font-mono text-[10px] uppercase tracking-[0.24em] text-foreground/55">
            {{ $t('home.liveHero.currentBid') }}
          </p>
          <p
            class="mt-1.5 text-[2rem] font-semibold leading-none tracking-[-0.05em] tabular-nums transition-colors duration-700"
            :class="priceJustChanged ? 'text-[var(--auction-accent)]' : 'text-foreground'"
          >
            {{ price }}
          </p>
        </div>

        <div class="hero-meta-right text-right lg:absolute lg:right-0 lg:top-[30%] lg:z-30" data-depth="1.2">
          <p class="hero-reveal font-mono text-[10px] uppercase tracking-[0.24em] text-foreground/55 sm:text-[11px]">
            {{ $t('home.liveHero.timeLeft') }}
          </p>
          <p
            class="hero-reveal mt-1.5 font-mono text-[1.6rem] leading-none tracking-[-0.02em] tabular-nums lg:mt-2 lg:text-[clamp(1.6rem,2.2vw,2.1rem)]"
            :class="isClosingSoon ? 'text-[var(--auction-accent)]' : 'text-foreground'"
          >
            {{ isEnded ? $t('home.liveHero.ended') : timeLeft }}
          </p>
          <div class="hero-rule mt-5 hidden h-px origin-right bg-foreground/15 lg:block" />
          <p class="hero-reveal mt-4 hidden font-mono text-[11px] uppercase tracking-[0.24em] text-foreground/55 lg:block">
            {{ $t('home.liveHero.bids') }}
            <span class="ml-2 text-foreground">{{ lot.bidCount ?? '—' }}</span>
          </p>
        </div>
      </div>

      <!-- CTA -->
      <div class="hero-cta lg:absolute lg:bottom-[9%] lg:left-0 lg:z-30" data-depth="0.5">
        <div class="hero-reveal flex flex-col gap-4 sm:flex-row sm:items-center sm:gap-6">
          <MagneticButton :to="lotLink" wide>
            {{ $t('home.liveHero.bidNow') }}
          </MagneticButton>

          <RouterLink
            :to="localePath('/catalog?endingSoon=true')"
            class="text-center font-mono text-[11px] uppercase tracking-[0.2em] text-foreground/60 underline-offset-[6px] transition hover:text-foreground hover:underline"
          >
            {{ $t('home.liveHero.allLots') }}
          </RouterLink>
        </div>
      </div>

      <!-- scroll hint -->
      <div class="pointer-events-none absolute bottom-0 left-1/2 hidden -translate-x-1/2 translate-y-1/2 items-center gap-3 font-mono text-[10px] uppercase tracking-[0.24em] text-foreground/45 lg:flex" aria-hidden="true">
        <span class="hero-scroll-line block h-6 w-px bg-foreground/30" />
        {{ $t('home.liveHero.scroll') }}
      </div>
    </div>
  </section>
</template>

<style scoped>
.hero-columns {
  background-image: linear-gradient(to right, rgb(0 0 0 / 0.045) 1px, transparent 1px);
  background-size: calc(100% / 6) 100%;
}

:global(.dark) .hero-columns {
  background-image: linear-gradient(to right, rgb(255 255 255 / 0.06) 1px, transparent 1px);
}

/* headline over the lot, difference keeps it readable */
@media (min-width: 1024px) {
  .hero-line {
    color: #ffffff;
    mix-blend-mode: difference;
  }
}

/* arch shape */
.hero-media-frame {
  border-radius: 999px 999px 8px 8px;
  will-change: transform;
}

@media (max-width: 1023px) {
  .hero-media-frame {
    border-radius: 220px 220px 8px 8px;
  }
}

.hero-live-dot {
  animation: hero-live-dot 1.6s ease-in-out infinite;
}

@keyframes hero-live-dot {
  0%,
  100% {
    opacity: 1;
  }

  50% {
    opacity: 0.25;
  }
}

.hero-scroll-line {
  transform-origin: top;
  animation: hero-scroll-line 2.4s cubic-bezier(0.65, 0, 0.35, 1) infinite;
}

@keyframes hero-scroll-line {
  0% {
    transform: scaleY(0);
  }

  50% {
    transform: scaleY(1);
    transform-origin: top;
  }

  51% {
    transform-origin: bottom;
  }

  100% {
    transform: scaleY(0);
    transform-origin: bottom;
  }
}

@media (prefers-reduced-motion: reduce) {
  .hero-live-dot,
  .hero-scroll-line {
    animation: none;
  }
}
</style>
