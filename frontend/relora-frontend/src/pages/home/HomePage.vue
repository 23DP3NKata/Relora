<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'
import { getCookie, setCookie } from '@/app/services/cookieService'
import api from '@/api'
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

    <HomeCategories />

    <HomeBrands />

    <HomeWhyRelora />

    <HomeHowItWorks />

    <section class="border-t border-foreground/10 pt-6">
      <p class="max-w-3xl text-sm leading-7 text-foreground/55">
        {{ $t('home.seoFooter') }}
      </p>
    </section>
  </div>
</template>
