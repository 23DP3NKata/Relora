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
import { getBidCount, getHomeLiveLots, getShowcaseLot } from '@/app/services/homeService'
import { auctionRealtimeService } from '@/app/services/auctionRealtimeService'
import type { HomeLiveLot } from '@/types/home'

import SelectPreferenceModal from '@/components/modals/SelectPreferenceModal.vue'
import HomeHero from '@/components/home/HomeHero.vue'
import HomeLiveAuctions from '@/components/home/HomeLiveAuctions.vue'
import HomeNewLots from '@/components/home/HomeNewLots.vue'
import HomeCategories from '@/components/home/HomeCategories.vue'
import HomeBrands from '@/components/home/HomeBrands.vue'
import HomeWhyRelora from '@/components/home/HomeWhyRelora.vue'
import HomeHowItWorks from '@/components/home/HomeHowItWorks.vue'
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

    <HomeCategories />

    <HomeBrands />

    <HomeWhyRelora />

    <HomeHowItWorks />

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

    <section class="border-t border-foreground/10 pt-6">
      <p class="max-w-3xl text-sm leading-7 text-foreground/55">
        {{ $t('home.seoFooter') }}
      </p>
    </section>
  </div>
</template>
