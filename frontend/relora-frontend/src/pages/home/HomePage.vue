<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/authStore'
import { storeToRefs } from 'pinia'
import { getAuctions } from '@/app/services/auctionService'
import { RouterLink } from "vue-router"
import { getCookie, setCookie } from '@/app/services/cookieService'
import api from '@/api'
import { useLocalePath } from '@/composables/useLocalePath'
import { Search, Compass, Trophy, ArrowRight } from 'lucide-vue-next'
import { getBidCount, getHomeLiveLots, getShowcaseLot } from '@/app/services/homeService'
import { auctionRealtimeService } from '@/app/services/auctionRealtimeService'
import type { HomeLiveLot } from '@/types/home'

import SelectPreferenceModal from '@/components/modals/SelectPreferenceModal.vue'
import HomeHero from '@/components/home/HomeHero.vue'
import HomeLiveAuctions from '@/components/home/HomeLiveAuctions.vue'
import HomeNewLots from '@/components/home/HomeNewLots.vue'
import RefreshCwIcon from '@/components/ui/icons/RefreshCwIcon.vue'
import TimerIcon from '@/components/ui/icons/TimerIcon.vue'

const authStore = useAuthStore()
const localePath = useLocalePath()
const { t } = useI18n()
const { user, isAuthenticated } = storeToRefs(authStore)

type UserPreference = 'men' | 'women'

const userPreference = ref<UserPreference>('women')
const showPreferenceModal = ref(false)

// real live lots, the first one goes to the hero
const liveLots = ref<HomeLiveLot[]>([])
const lotsLoaded = ref(false)
const showcaseLot = getShowcaseLot()

// no real auctions yet -> hero shows the showcase lot
const heroLot = computed(() => liveLots.value[0] ?? showcaseLot)
const feedLots = computed(() => liveLots.value.slice(1))

let stopBidPlaced: (() => void) | undefined
let stopAuctionEnded: (() => void) | undefined

onMounted(() => {
  const preference = getCookie('user_preference')

  if (preference === 'men' || preference === 'women') {
    userPreference.value = preference
    return
  }

  userPreference.value = 'women'
  showPreferenceModal.value = true
})

async function changeUserPreference(preference: UserPreference) {
  if (userPreference.value === preference) {
    return
  }

  await selectUserPreference(preference)
}

async function selectUserPreference(preference: UserPreference) {
  // close the modal right away, saving can take a moment
  userPreference.value = preference
  showPreferenceModal.value = false

  try {
    await api.post('/api/auth/set-preference', {
      preference,
    })
  } catch {
    // server is not available, save the choice in the browser
    setCookie('user_preference', preference)
  }
}

onMounted(async () => {
  liveLots.value = await getHomeLiveLots()
  lotsLoaded.value = true

  const hero = liveLots.value[0]
  if (!hero?.auctionId) return

  hero.bidCount = await getBidCount(hero.auctionId)

  // new bids from server (SignalR), display only
  stopBidPlaced = auctionRealtimeService.on('BidPlaced', (event) => {
    const lot = liveLots.value.find((item) => item.auctionId === event.auctionId)
    if (!lot) return

    // ignore older prices
    if (event.amount > lot.currentPrice) {
      lot.currentPrice = event.amount
    }

    if (lot.bidCount !== null) {
      lot.bidCount++
    }
  })

  stopAuctionEnded = auctionRealtimeService.on('AuctionEnded', (event) => {
    const lot = liveLots.value.find((item) => item.auctionId === event.auctionId)
    if (lot) lot.endsAt = new Date().toISOString()
  })

  auctionRealtimeService.joinAuction(hero.auctionId).catch(() => {})
})

onBeforeUnmount(() => {
  stopBidPlaced?.()
  stopAuctionEnded?.()

  if (heroLot.value?.auctionId) {
    auctionRealtimeService.leaveAuction(heroLot.value.auctionId).catch(() => {})
  }
})

const exploreCollections = ref([
  {
    id: 1,
    imageUrl: 'https://pub-d44c1b06b612479f8e654eedc923ffa1.r2.dev/display/collections/Avant_Garde_Collection_Photo.png',
    query: '?style=avant-garde',
  },
  {
    id: 2,
    imageUrl: 'https://pub-d44c1b06b612479f8e654eedc923ffa1.r2.dev/display/collections/Minimal_Designer_Collection_Photo.png',
    query: '?style=minimal',
  },
  {
    id: 3,
    imageUrl: 'https://pub-d44c1b06b612479f8e654eedc923ffa1.r2.dev/display/collections/Elevated_Streetwear_Collection_Photo.png',
    query: '?style=streetwear',
  },
])

const categories = ref([
  {
    name: 'Vintage',
    imageUrl: 'https://pub-d44c1b06b612479f8e654eedc923ffa1.r2.dev/display/Vintage_Category_Icon.png',
  },
  {
    name: 'Streetwear',
    imageUrl: 'https://pub-d44c1b06b612479f8e654eedc923ffa1.r2.dev/display/Streetwear_Category_Icon.png',
  },
  {
    name: 'Designer',
    imageUrl: 'https://pub-d44c1b06b612479f8e654eedc923ffa1.r2.dev/display/Designer_Category_Icon.png',
  },
  {
    name: 'Sneakers',
    imageUrl: 'https://pub-d44c1b06b612479f8e654eedc923ffa1.r2.dev/display/Shoes_Category_Icon.png',
  },
  {
    name: 'Accessories',
    imageUrl: 'https://pub-d44c1b06b612479f8e654eedc923ffa1.r2.dev/display/Acessories_Categorie_Icon.png',
  },
])

const brands = ref([
  { name: 'Prada', logo: '/brands/prada-logo.svg' },
  { name: 'Gucci', logo: '/brands/gucci-logo.svg' },
  { name: 'Dolce Gabbana', logo: '/brands/dolce-gabbana-logo.svg' },
  { name: 'Dior', logo: '/brands/dior-logo.svg' },
  { name: 'Chanel', logo: '/brands/chanel-2-logo.svg' },
])

const steps = [
  { key: 'discover', icon: Search },
  { key: 'bid', icon: Compass },
  { key: 'win', icon: Trophy },
]

const loading = ref(false)

</script>

<template>

  <SelectPreferenceModal
    v-if="showPreferenceModal"
    @select="selectUserPreference"
  />

  <div class="space-y-16">
    <!-- keep hero space while loading -->
    <div v-if="!lotsLoaded" class="h-[min(100svh,860px)]" aria-hidden="true" />

    <template v-else>
      <HomeHero :lot="heroLot" />
      <HomeLiveAuctions :lots="feedLots" />
      <HomeNewLots :preference="userPreference" @change="changeUserPreference" />
    </template>

    <!-- <section>
      <div class="flex items-end justify-between gap-4">
        <div class="max-w-2xl">
          <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/50">
            {{ $t('home.explore') }}
          </p>
          <h2 class="mt-2 text-3xl font-semibold tracking-tight sm:text-4xl">
            {{ $t('home.discoverWorld') }}
          </h2>
          <p class="mt-3 max-w-xl text-sm leading-6 text-foreground/70 sm:text-base">
            {{ $t('home.discoverWorldDescription') }}
          </p>
        </div>

        <button
          class="hidden rounded-full border bg-background px-5 py-2.5 text-sm font-medium text-foreground transition hover:bg-black hover:text-white dark:hover:bg-white dark:hover:text-black md:inline-flex"
        >
          {{ $t('home.viewAllCategories') }}
        </button>
      </div>

      <div class="mt-5 flex gap-4 overflow-x-auto pb-3 sm:grid sm:grid-cols-2 sm:overflow-visible sm:pb-0 lg:grid-cols-3">
        <article
          v-for="collection in exploreCollections"
          :key="collection.id"
          class="group relative h-[220px] min-w-[230px] cursor-pointer overflow-hidden rounded-[28px] border bg-neutral-100 dark:bg-neutral-900 sm:min-w-0"
        >
          <img
            :src="collection.imageUrl"
            :alt="$t(`home.collections.${collection.id}.title`)"
            class="h-full w-full object-cover transition duration-700 group-hover:scale-105"
          />

          <div class="absolute bottom-0 left-0 right-0 h-[65%] bg-gradient-to-t from-black/70 via-black/25 to-transparent transition duration-300 group-hover:from-black/55" />

          <div class="absolute bottom-0 left-0 p-5">
            <p class="text-[11px] uppercase tracking-[0.22em] text-white/65 drop-shadow">
              {{ $t(`home.collections.${collection.id}.label`) }}
            </p>

            <h3 class="mt-2 text-xl font-semibold text-white drop-shadow-[0_2px_8px_rgba(0,0,0,0.65)]">
              {{ $t(`home.collections.${collection.id}.title`) }}
            </h3>

            <p class="mt-2 max-w-sm text-sm text-white/70">
              {{ $t(`home.collections.${collection.id}.subtitle`) }}
            </p>
          </div>
        </article>
      </div>
    </section> -->

        <section>
      <div>
        <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/50">
          {{ $t('home.categories') }}
        </p>
        <h2 class="mt-2 text-3xl font-semibold tracking-tight sm:text-4xl">
          {{ $t('home.shopByCategory') }}
        </h2>
        <p class="mt-3 max-w-xl text-sm leading-6 text-foreground/70 sm:text-base">
          {{ $t('home.shopByCategoryDescription') }}
        </p>
      </div>

      <div class="mt-5 flex gap-4 overflow-x-auto pb-3 sm:grid sm:grid-cols-2 sm:overflow-visible sm:pb-0 lg:grid-cols-5">
        <article
          v-for="category in categories"
          :key="category.name"
          class="group relative h-[220px] min-w-[230px] cursor-pointer overflow-hidden rounded-[28px] border bg-neutral-100 dark:bg-neutral-900 sm:min-w-0"
        >
          <img
            :src="category.imageUrl"
            :alt="$t(`home.categoryNames.${category.name}`)"
            class="h-full w-full object-cover transition duration-700 group-hover:scale-105"
          />

          <div class="absolute bottom-0 left-0 right-0 h-[65%] bg-gradient-to-t from-black/70 via-black/25 to-transparent transition duration-300 group-hover:from-black/55" />

          <div class="absolute bottom-0 left-0 p-5">
            <p class="text-[11px] uppercase tracking-[0.22em] text-white/65 drop-shadow">
              {{ $t('home.category') }}
            </p>

            <h3 class="mt-2 text-xl font-semibold text-white drop-shadow-[0_2px_8px_rgba(0,0,0,0.65)]">
              {{ $t(`home.categoryNames.${category.name}`) }}
            </h3>
          </div>
        </article>
      </div>
    </section>
    
    <section>
      <div class="max-w-3xl">
        <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/50">
          {{ $t('home.whyRelora') }}
        </p>
        <h2 class="mt-2 text-3xl font-semibold tracking-tight sm:text-4xl">
          {{ $t('home.whyTitle') }}
        </h2>
        <p class="mt-3 text-sm leading-6 text-foreground/70 sm:text-base">
          {{ $t('home.whyDescription') }}
        </p>
      </div>

      <div class="mt-5 grid grid-cols-1 gap-5 md:grid-cols-2 xl:grid-cols-4">
        <article class="rounded-[24px] border bg-background px-7 py-8">
          <div class="mb-6 flex h-14 w-14 items-center justify-center rounded-2xl bg-neutral-100 text-foreground/70 dark:bg-neutral-800">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-7 w-7" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M17 20h5V4H2v16h5m10 0v-4a3 3 0 10-6 0v4m6 0H9" />
            </svg>
          </div>
          <h3 class="text-xl font-semibold">
            {{ $t('home.curatedTitle') }}
          </h3>
          <p class="mt-3 text-sm leading-6 text-foreground/70">
            {{ $t('home.curatedDescription') }}
          </p>
        </article>

        <article class="rounded-[24px] border bg-background px-7 py-8">
          <div class="mb-6 flex h-14 w-14 items-center justify-center rounded-2xl bg-neutral-100 text-foreground/70 dark:bg-neutral-800">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-7 w-7" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 3h12l1 6-7 12L5 9l1-6z" />
            </svg>
          </div>
          <h3 class="text-xl font-semibold">
            {{ $t('home.communityTitle') }}
          </h3>
          <p class="mt-3 text-sm leading-6 text-foreground/70">
            {{ $t('home.communityDescription') }}
          </p>
        </article>

        <article class="rounded-[24px] border bg-background px-7 py-8">
          <div class="mb-6 flex h-14 w-14 items-center justify-center rounded-2xl bg-neutral-100 text-foreground/70 dark:bg-neutral-800">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-7 w-7" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M12 8c-1.657 0-3 1.343-3 3v1H8a2 2 0 00-2 2v4h12v-4a2 2 0 00-2-2h-1v-1c0-1.657-1.343-3-3-3z" />
            </svg>
          </div>
          <h3 class="text-xl font-semibold">
            {{ $t('home.premiumTitle') }}
          </h3>
          <p class="mt-3 text-sm leading-6 text-foreground/70">
            {{ $t('home.premiumDescription') }}
          </p>
        </article>

        <article class="rounded-[24px] border bg-background px-7 py-8">
          <div class="mb-6 flex h-14 w-14 items-center justify-center rounded-2xl bg-neutral-100 text-foreground/70 dark:bg-neutral-800">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-7 w-7" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="1.5">
              <path stroke-linecap="round" stroke-linejoin="round" d="M17 20h5V4H2v16h5m10 0a3 3 0 01-6 0m6 0a3 3 0 00-6 0" />
            </svg>
          </div>
          <h3 class="text-xl font-semibold">
            {{ $t('home.trustTitle') }}
          </h3>
          <p class="mt-3 text-sm leading-6 text-foreground/70">
            {{ $t('home.trustDescription') }}
          </p>
        </article>
      </div>
    </section>

      <section class="w-full">
    <div class="rounded-3xl bg-neutral-950 px-6 py-10 text-white sm:px-10 sm:py-12">
      <div class="flex flex-col gap-10 lg:flex-row lg:items-center">
        <div class="lg:w-1/4 lg:shrink-0 lg:border-r lg:border-white/10 lg:pr-8">
          <span class="text-xs font-semibold uppercase tracking-[0.2em] text-white/50">
            {{ $t('home.howItWorks.label') }}
          </span>
          <h2 class="mt-3 text-2xl font-semibold sm:text-3xl">
            {{ $t('home.howItWorks.title') }}
          </h2>
        </div>

        <div class="flex flex-1 flex-col gap-8 lg:flex-row lg:items-center lg:gap-6 lg:pl-10">
          <template v-for="(step, index) in steps" :key="step.key">
            <div class="flex items-start gap-4">
              <div class="flex h-12 w-12 shrink-0 items-center justify-center rounded-full border border-white/20">
                <component :is="step.icon" class="h-5 w-5" :stroke-width="1.5" />
              </div>
              <div>
                <p class="text-sm font-semibold text-white">
                  {{ $t(`home.howItWorks.steps.${step.key}.number`) }}. {{ $t(`home.howItWorks.steps.${step.key}.title`) }}
                </p>
                <p class="mt-1 text-sm leading-relaxed text-white/50">
                  {{ $t(`home.howItWorks.steps.${step.key}.description`) }}
                </p>
              </div>
            </div>

            <ArrowRight
              v-if="index < steps.length - 1"
              class="hidden h-5 w-5 shrink-0 text-white/30 lg:block"
            />
          </template>
        </div>
      </div>
    </div>
  </section>

    <!-- <section class="rounded-[28px] border px-4 py-6 md:px-6">
      <div class="mb-6 flex items-end justify-between gap-3">
        <div>
          <div class="flex items-center gap-2">
            <h2 class="text-2xl font-semibold tracking-tight">{{ $t('home.newListings') }}</h2>
            <RefreshCwIcon class="h-5 w-5" />
          </div>
          <p class="mt-2 text-sm text-foreground/70 sm:text-base">
            {{ $t('home.newListingsDescription') }}
          </p>
        </div>

        <button class="text-sm text-foreground/70 transition hover:text-foreground">
          {{ $t('home.browseMore') }}
        </button>
      </div>

      <Carousel
        :opts="{ loop: true, align: 'start', slidesToScroll: 1 }"
        class="w-full"
      >
        <CarouselContent class="-ml-3">
          <CarouselItem
            v-for="auction in newListings"
            :key="auction.id"
            class="basis-[78%] pl-3 sm:basis-1/2 md:basis-1/3 lg:basis-1/5"
          >
            <AuctionCard
              :brand="auction.brand"
              :title="auction.title"
              :price="auction.price"
              :image-url="auction.imageUrl"
              :time-left="auction.timeLeft"
            />
          </CarouselItem>
        </CarouselContent>

        <div class="mt-6 flex items-center justify-between">
          <p class="text-sm text-foreground/70">
            {{ $t('home.scrollNew') }}
          </p>

          <div class="flex items-center gap-2">
            <CarouselPrevious
              class="static h-10 w-10 translate-x-0 translate-y-0 rounded-full border bg-background text-foreground shadow-sm hover:bg-black hover:text-white"
            />
            <CarouselNext
              class="static h-10 w-10 translate-x-0 translate-y-0 rounded-full border bg-background text-foreground shadow-sm hover:bg-black hover:text-white"
            />
          </div>
        </div>
      </Carousel>
    </section> -->

    <section>
      <div>
        <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/50">
          {{ $t('home.brands') }}
        </p>
        <h2 class="mt-2 text-3xl font-semibold tracking-tight sm:text-4xl">
          {{ $t('home.shopByBrand') }}
        </h2>
        <p class="mt-3 max-w-xl text-sm leading-6 text-foreground/70 sm:text-base">
          {{ $t('home.shopByBrandDescription') }}
        </p>
      </div>

        <div class="mt-5 flex gap-4 overflow-x-auto pb-3 sm:grid sm:grid-cols-3 sm:overflow-visible sm:pb-0 lg:grid-cols-5">        <article
          v-for="brand in brands"
          :key="brand.name"
          class="group flex aspect-square cursor-pointer flex-col items-center justify-center rounded-[28px] border bg-background p-6 text-center transition duration-300 hover:bg-black dark:hover:bg-white"
        >
          <img
            :src="brand.logo"
            :alt="brand.name"
            class="max-h-30 max-w-[150px] object-contain transition duration-300  group-hover:invert dark:invert group-hover:dark:invert-0"
          />

          <p class="mt-5 text-[11px] uppercase tracking-[0.22em] text-foreground/45 group-hover:text-white/60 dark:group-hover:text-black/60">
            {{ $t('home.brand') }}
          </p>
        </article>
      </div>
    </section>

    <!-- <section class="rounded-[28px] border px-4 py-6 md:px-6">
      <div class="mb-6 flex items-end justify-between gap-3">
        <div>
          <div class="flex items-center gap-2">
            <h2 class="text-2xl font-semibold tracking-tight">{{ $t('home.endingSoon') }}</h2>
            <TimerIcon class="h-5 w-5" />
          </div>
          <p class="mt-2 text-sm text-foreground/70 sm:text-base">
            {{ $t('home.endingSoonDescription') }}
          </p>
        </div>

        <button class="text-sm text-foreground/70 transition hover:text-foreground">
          {{ $t('home.browseMore') }}
        </button>
      </div>

      <Carousel
        :opts="{ loop: true, align: 'start', slidesToScroll: 1 }"
        class="w-full"
      >
        <CarouselContent class="-ml-3">
          <CarouselItem
            v-for="auction in endingSoon"
            :key="auction.id"
            class="basis-[78%] pl-3 sm:basis-1/2 md:basis-1/3 lg:basis-1/5"
          >
            <AuctionCard
              :brand="auction.brand"
              :title="auction.title"
              :price="auction.price"
              :image-url="auction.imageUrl"
              :time-left="auction.timeLeft"
            />
          </CarouselItem>
        </CarouselContent>

        <div class="mt-6 flex items-center justify-between">
          <p class="text-sm text-foreground/70">
            {{ $t('home.scrollEnding') }}
          </p>

          <div class="flex items-center gap-2">
            <CarouselPrevious
              class="static h-10 w-10 translate-x-0 translate-y-0 rounded-full border bg-background text-foreground shadow-sm hover:bg-black hover:text-white"
            />
            <CarouselNext
              class="static h-10 w-10 translate-x-0 translate-y-0 rounded-full border bg-background text-foreground shadow-sm hover:bg-black hover:text-white"
            />
          </div>
        </div>
      </Carousel>
    </section> -->

    <section class="mx-auto max-w-3xl border-t pt-10 text-center">
      <p class="text-xs leading-7 text-foreground/65 sm:text-base">
        {{ $t('home.seoFooter') }}
      </p>
    </section>
  </div>
</template>
