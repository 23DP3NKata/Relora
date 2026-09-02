<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import type { Auction } from '@/types/auction'
import { useAuthStore } from '@/stores/authStore'
import { storeToRefs } from 'pinia'
import { getAuctions } from '@/app/services/auctionService'
import { RouterLink } from "vue-router"
import { getCookie } from '@/app/services/cookieService'
import api from '@/api'
import { useLocalePath } from '@/composables/useLocalePath'
import { Search, Compass, Trophy, ArrowRight } from 'lucide-vue-next'
import heroProductImage from '@/assets/brands/hero_brand.png'

import SelectPreferenceModal from '@/components/modals/SelectPreferenceModal.vue'
import AuctionCard from '@/components/auctions/AuctionCard.vue'
import TrendingUpIcon from '@/components/ui/icons/TrendingUpIcon.vue'
import RefreshCwIcon from '@/components/ui/icons/RefreshCwIcon.vue'
import TimerIcon from '@/components/ui/icons/TimerIcon.vue'
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from '@/components/ui/carousel'

const authStore = useAuthStore()
const localePath = useLocalePath()
const { t } = useI18n()
const { user, isAuthenticated } = storeToRefs(authStore)

type UserPreference = 'men' | 'women'

const userPreference = ref<UserPreference>('women')
const showPreferenceModal = ref(false)

const heroCountdownTarget = Date.now() + ((2 * 3600) + (14 * 60) + 37) * 1000
const heroCountdown = ref('02 : 14 : 37')
let heroCountdownTimer: ReturnType<typeof setInterval> | null = null

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
  await api.post('/api/auth/set-preference', {
    preference,
  })

  userPreference.value = preference
  showPreferenceModal.value = false
}

function formatHeroCountdown(value: number) {
  const hours = Math.floor(value / 3600)
  const minutes = Math.floor((value % 3600) / 60)
  const seconds = value % 60

  return [hours, minutes, seconds]
    .map((part) => part.toString().padStart(2, '0'))
    .join(' : ')
}

function updateHeroCountdown() {
  const remainingSeconds = Math.max(0, Math.floor((heroCountdownTarget - Date.now()) / 1000))
  heroCountdown.value = formatHeroCountdown(remainingSeconds)
}

onMounted(() => {
  updateHeroCountdown()
  heroCountdownTimer = setInterval(updateHeroCountdown, 1000)
})

onBeforeUnmount(() => {
  if (heroCountdownTimer) {
    clearInterval(heroCountdownTimer)
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

type HomeAuctionItem = Omit<Auction, 'title' | 'timeLeft'> & {
  titleKey: string
  timeLeftKey: string
}

const auctionItems: HomeAuctionItem[] = [
  {
    id: 1,
    brand: "Hermes",
    titleKey: 'home.mockAuctions.1.title',
    price: 1000000,
    imageUrl: "https://media-photos.depop.com/b1/28724162/3191473831_ade6609dc27340629bc6d4af9ac1b841/P0.jpg",
    timeLeftKey: 'home.mockAuctions.1.timeLeft',
  },
  {
    id: 2,
    brand: "Gucci",
    titleKey: 'home.mockAuctions.2.title',
    price: 320,
    imageUrl: "https://media-photos.depop.com/b1/38992639/3121126790_b608df72f79c4f8b9cb99737678b74f5/P0.jpg",
    timeLeftKey: 'home.mockAuctions.2.timeLeft',
  },
  {
    id: 3,
    brand: "Rolex",
    titleKey: 'home.mockAuctions.3.title',
    price: 5400,
    imageUrl: "https://media-photos.depop.com/b1/40086058/3512274199_d42fb73add7043d586f1be825bfb1f68/P0.jpg",
    timeLeftKey: 'home.mockAuctions.3.timeLeft',
  },
  {
    id: 4,
    brand: "Chanel",
    titleKey: 'home.mockAuctions.4.title',
    price: 280,
    imageUrl: "https://media-photos.depop.com/b1/43448124/3109199960_e0915b4487004c61a6f7613dca8db4a9/P0.jpg",
    timeLeftKey: 'home.mockAuctions.4.timeLeft',
  },
  {
    id: 5,
    brand: "Tiffany & Co.",
    titleKey: 'home.mockAuctions.5.title',
    price: 650,
    imageUrl: "https://media-photos.depop.com/b1/51377749/3251202528_7ca0ed8a90b8496c9b8793bd28d3de17/P0.jpg",
    timeLeftKey: 'home.mockAuctions.5.timeLeft',
  },
  {
    id: 6,
    brand: "Louis Vuitton",
    titleKey: 'home.mockAuctions.6.title',
    price: 2100,
    imageUrl: "https://images.thebestshops.com/product_images/original/SL12226-044_01-339d21.jpg",
    timeLeftKey: 'home.mockAuctions.6.timeLeft',
  },
  {
    id: 7,
    brand: "The Beatles",
    titleKey: 'home.mockAuctions.7.title',
    price: 3200,
    imageUrl: "https://media-photos.depop.com/b1/45498419/3517277733_6056232abec546e987925ec83630f810/P0.jpg",
    timeLeftKey: 'home.mockAuctions.7.timeLeft',
  },
  {
    id: 8,
    brand: "Prada",
    titleKey: 'home.mockAuctions.8.title',
    price: 420,
    imageUrl: "https://media-photos.depop.com/b1/36719830/3498593736_9131744e9156480d90e996a6dcb6ec67/P0.jpg",
    timeLeftKey: 'home.mockAuctions.8.timeLeft',
  },
  {
    id: 9,
    brand: "Yeezy",
    titleKey: 'home.mockAuctions.9.title',
    price: 180,
    imageUrl: "https://media-photos.depop.com/b1/448068812/3474488748_5e0e9243711c4cbcb6dce7afdff37d6d/P0.jpg",
    timeLeftKey: 'home.mockAuctions.9.timeLeft',
  },
  {
    id: 10,
    brand: "Balenciaga",
    titleKey: 'home.mockAuctions.10.title',
    price: 240,
    imageUrl: "https://media-photos.depop.com/r1/341927691/3481391174_2e97b19297a44aae8e7e46d6e737e530/P6.jpg",
    timeLeftKey: 'home.mockAuctions.10.timeLeft',
  },
  {
    id: 11,
    brand: "Cartier",
    titleKey: 'home.mockAuctions.11.title',
    price: 890,
    imageUrl: "https://media-photos.depop.com/b1/20411984/2751785262_45ca34b51be54edbad51742f72c8c676/P0.jpg",
    timeLeftKey: 'home.mockAuctions.11.timeLeft',
  },
  {
    id: 12,
    brand: "Gucci",
    titleKey: 'home.mockAuctions.12.title',
    price: 1200,
    imageUrl: "https://media-photos.depop.com/b1/51416371/3553513106_b896da1472a94103a15b55b05dd159f1/P0.jpg",
    timeLeftKey: 'home.mockAuctions.12.timeLeft',
  },
]

const auctions = computed<Auction[]>(() =>
  auctionItems.map((auction) => ({
    ...auction,
    title: t(auction.titleKey),
    timeLeft: t(auction.timeLeftKey),
  })),
)

const steps = [
  { key: 'discover', icon: Search },
  { key: 'bid', icon: Compass },
  { key: 'win', icon: Trophy },
]

const loading = ref(false)
const error = ref('')

const trendingAuctions = computed(() => auctions.value.slice(0, 10))
const newListings = computed(() => auctions.value.slice(2, 12))
const endingSoon = computed(() => auctions.value.slice(0, 10))
</script>

<template>

  <SelectPreferenceModal
    v-if="showPreferenceModal"
    @select="selectUserPreference"
  />

  <div class="space-y-16">
    <section class="relative overflow-hidden rounded-[32px] border border-[#E5E5E3] bg-[#F7F7F5] px-6 py-6 shadow-[0_8px_30px_rgba(0,0,0,0.03)] sm:px-8 sm:py-8 lg:min-h-[660px] lg:px-12 lg:py-12">
      <div class="absolute inset-0 bg-[radial-gradient(circle_at_top_left,_rgba(255,255,255,0.95),_transparent_42%),radial-gradient(circle_at_80%_15%,_rgba(255,255,255,0.8),_transparent_34%),linear-gradient(135deg,rgba(255,255,255,0.75),rgba(248,248,247,0.92))]" />
      <div class="absolute inset-0 bg-[linear-gradient(180deg,rgba(255,255,255,0.42)_0%,rgba(255,255,255,0)_34%,rgba(0,0,0,0.02)_100%)]" />

      <div class="relative grid gap-10 lg:min-h-[596px] lg:grid-cols-[0.94fr_1.06fr] lg:items-center xl:gap-14">
        <div class="flex flex-col justify-center gap-8 lg:self-center lg:pr-6 xl:pr-10">
          <div class="max-w-2xl pt-8 sm:pt-10 lg:pt-4">
            <p class="text-[10px] font-medium uppercase tracking-[0.42em] text-[#777777]">
              RELORA
            </p>

            <h1
              v-if="!isAuthenticated"
              class="mt-6 max-w-xl text-[clamp(3.6rem,7vw,4.75rem)] font-semibold leading-[0.95] tracking-[-0.07em] text-[#111111]"
            >
              {{ $t('home.heroTitleLine1') }}<br />
              <span class="font-medium">{{ $t('home.heroTitleLine2') }}</span>
            </h1>

            <h1
              v-else
              class="mt-6 max-w-xl text-[clamp(3.6rem,7vw,4.75rem)] font-semibold leading-[0.95] tracking-[-0.07em] text-[#111111]"
            >
              {{ $t('home.heroTitleLine1') }}<br />
              <span class="font-medium">{{ $t('home.heroTitleLine2') }}</span>
            </h1>

            <p class="mt-5 max-w-lg text-[clamp(1rem,1.2vw,1.125rem)] leading-8 text-[#4f4f4f]">
              {{ $t('home.heroDescriptionLine1') }}<br />
              {{ $t('home.heroDescriptionLine2') }}
            </p>

            <div class="mt-7 flex flex-wrap gap-3">
              <RouterLink
                :to="localePath('/catalog')"
                class="inline-flex h-12 items-center justify-center rounded-full bg-black px-6 text-sm font-medium text-white transition duration-200 hover:-translate-y-0.5 hover:bg-black/90"
              >
                {{ $t('home.exploreAuctions') }} →
              </RouterLink>

              <RouterLink
                :to="localePath('/sell')"
                class="inline-flex h-12 items-center justify-center rounded-full border border-[#E5E5E3] bg-white px-6 text-sm font-medium text-[#111111] transition duration-200 hover:-translate-y-0.5 hover:border-[#cacac6] hover:bg-[#fbfbfa]"
              >
                {{ $t('home.sellAnItem') }}
              </RouterLink>
            </div>
          </div>

        </div>

        <div class="hero-product-stage relative flex min-h-[520px] items-center justify-center overflow-visible -mr-6 -mb-6 sm:-mr-8 sm:-mb-8 lg:-mr-12 lg:-mb-12 lg:min-h-[596px] lg:justify-end">
          <div class="absolute inset-x-[12%] bottom-[12%] top-[14%] rounded-full bg-[radial-gradient(circle,_rgba(255,255,255,0.95)_0%,_rgba(247,247,245,0.92)_35%,_rgba(224,224,220,0.45)_100%)] blur-3xl" />

          <div class="hero-product-frame relative flex h-full w-full items-center justify-center overflow-visible">
            <div class="hero-auction-card absolute left-0 top-[33%] z-20 w-[min(100%,21.5rem)] rounded-[20px] border border-[#E5E5E3] bg-white/96 p-5 shadow-[0_20px_60px_rgba(0,0,0,0.08)] backdrop-blur-[2px] sm:left-[2%] sm:p-6 lg:left-[2%] xl:left-[4%]">
              <div class="flex items-center gap-2 text-[11px] font-medium uppercase tracking-[0.28em] text-[#777777]">
                <span class="hero-live-dot h-2 w-2 rounded-full bg-black" />
                <span>{{ $t('home.liveAuction') }}</span>
              </div>

              <h2 class="mt-5 text-[clamp(1.5rem,2vw,1.9rem)] font-medium leading-[1.02] tracking-[-0.05em] text-[#111111]">
                {{ $t('home.heroLotTitleLine1') }}<br />
                {{ $t('home.heroLotTitleLine2') }}
              </h2>

              <div class="mt-8 flex items-end justify-between gap-4">
                <div>
                  <p class="text-xs uppercase tracking-[0.22em] text-[#777777]">
                    {{ $t('home.currentBid') }}
                  </p>
                  <p class="mt-2 text-[clamp(2.2rem,3.2vw,2.6rem)] font-semibold leading-none tracking-[-0.06em] text-[#111111]">
                    €520
                  </p>
                </div>

                <p class="pb-1 font-mono text-sm tabular-nums text-[#777777]">
                  {{ $t('home.bidCount', { count: 12 }) }}
                </p>
              </div>

              <div class="mt-6 h-px bg-[#E5E5E3]" />

              <div class="mt-5 flex items-end justify-between gap-4">
                <div>
                  <p class="font-mono text-[clamp(1.35rem,1.8vw,1.65rem)] leading-none tracking-[0.04em] text-[#111111] tabular-nums">
                    {{ heroCountdown }}
                  </p>
                  <p class="mt-2 text-xs uppercase tracking-[0.22em] text-[#777777]">
                    {{ $t('home.left') }}
                  </p>
                </div>

                <button class="inline-flex h-11 items-center justify-center rounded-full bg-black px-5 text-sm font-medium text-white transition duration-200 hover:-translate-y-0.5 hover:bg-black/90">
                  {{ $t('home.viewLot') }}
                </button>
              </div>
            </div>

            <div
              class="group relative h-full min-h-[520px] w-full overflow-visible lg:min-h-[596px]"
            >
              <img
                :src="heroProductImage"
                :alt="$t('home.heroLotAlt')"
                class="hero-product-image absolute right-0 bottom-0 z-10 h-[115%] w-auto max-w-none select-none drop-shadow-[0_35px_50px_rgba(0,0,0,0.16)] transition-transform duration-700 ease-out group-hover:scale-[1.015]"
                style="
                  right: -10px;
                  bottom: -90px;
                "
              />
            </div>
          </div>
        </div>
      </div>
    </section>

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
    
    <section class="rounded-[28px] border px-4 py-6 md:px-6">
      <div class="mb-6 flex items-end justify-between gap-3">
        <div>
          <div class="flex items-center gap-2">
            <h2 class="text-2xl font-semibold tracking-tight">{{ $t('home.trendingAuctions') }}</h2>
            <TrendingUpIcon class="h-5 w-5" />
          </div>
          <p class="mt-2 text-sm text-foreground/70 sm:text-base">
            {{ $t('home.trendingDescription') }}
          </p>
        </div>

        <button class="text-sm text-foreground/70 transition hover:text-foreground">
          {{ $t('home.browseMore') }}
        </button>
      </div>

      <div v-if="error" class="mb-4 rounded-2xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-600 dark:border-red-900/40 dark:bg-red-950/30 dark:text-red-300">
        {{ error }}
      </div>

      <Carousel
        :opts="{ loop: true, align: 'start', slidesToScroll: 1 }"
        class="w-full"
      >
        <CarouselContent class="-ml-3">
          <CarouselItem
            v-for="auction in trendingAuctions"
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
            {{ $t('home.scrollTrending') }}
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

        <section class="rounded-[28px] border bg-background px-6 py-8 text-center">
      <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/50">
        {{ $t('home.catalogPreference') }}
      </p>

      <h2 class="mt-2 text-2xl font-semibold tracking-tight">
        {{ $t('home.currentlyBrowsing', { preference: $t(userPreference === 'women' ? 'navigation.women' : 'navigation.men') }) }}
      </h2>

      <p class="mx-auto mt-3 max-w-xl text-sm leading-6 text-foreground/65">
        {{ $t('home.preferenceDescription') }}
      </p>

      <div class="mt-6 flex justify-center gap-3">
        <button
          class="rounded-full px-5 py-2.5 text-sm font-medium transition"
          :class="userPreference === 'women'
            ? 'bg-black text-white dark:bg-white dark:text-black'
            : 'border bg-background text-foreground hover:bg-black hover:text-white dark:hover:bg-white dark:hover:text-black'"
          @click="changeUserPreference('women')"
        >
          {{ $t('navigation.women') }}
        </button>

        <button
          class="rounded-full px-5 py-2.5 text-sm font-medium transition"
          :class="userPreference === 'men'
            ? 'bg-black text-white dark:bg-white dark:text-black'
            : 'border bg-background text-foreground hover:bg-black hover:text-white dark:hover:bg-white dark:hover:text-black'"
          @click="changeUserPreference('men')"
        >
          {{ $t('navigation.men') }}
        </button>
      </div>
    </section>

    <section class="mx-auto max-w-3xl border-t pt-10 text-center">
      <p class="text-xs leading-7 text-foreground/65 sm:text-base">
        {{ $t('home.seoFooter') }}
      </p>
    </section>
  </div>
</template>

<style scoped>
.hero-live-dot {
  animation: hero-live-dot-blink 1.8s infinite;
}

@keyframes hero-live-dot-blink {
  0%,
  72%,
  100% {
    opacity: 1;
  }

  36% {
    opacity: 0.25;
  }
}

.hero-product-stage:hover .hero-auction-card {
  transform: translateY(-1px);
}

.hero-auction-card {
  transition: transform 300ms ease, box-shadow 300ms ease;
}

.hero-product-image {
  transform-origin: center bottom;
}

@media (min-width: 1024px) {
  .hero-product-stage:hover .hero-product-image {
    transform: scale(1.015) translateY(-2px);
  }
}
</style>
