<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { ProfileService } from '@/app/services/profileService'
import type { UserProfileDto } from '@/types/userProfile'
import { useLocalePath } from '@/composables/useLocalePath'

const route = useRoute()
const localePath = useLocalePath()
const { locale, t } = useI18n()
const profileService = ProfileService()

const profile = ref<UserProfileDto | null>(null)
const isLoading = ref(true)
const errorMessage = ref('')

const activeListings = computed(() => profile.value?.activeListings ?? [])
const soldListings = computed(() => profile.value?.soldListings ?? [])

async function loadProfile() {
  try {
    isLoading.value = true
    errorMessage.value = ''

    const username = route.params.username as string
    profile.value = await profileService.getUserProfile(username)
  } catch (error) {
    console.error('Failed to load profile', error)
    errorMessage.value = t('profile.failed')
  } finally {
    isLoading.value = false
  }
}

function formatPrice(amount: number, currency: string): string {
  return new Intl.NumberFormat(locale.value, {
    style: 'currency',
    currency,
    maximumFractionDigits: 0,
  }).format(amount)
}

function resolveImageUrl(path: string | null | undefined): string {
  if (!path) {
    return '/placeholder-image.jpg'
  }

  return `https://your-public-r2-url/${path}`
}

onMounted(loadProfile)
</script>

<template>
  <main class="bg-background">
    <div class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div
        v-if="isLoading"
        class="flex min-h-[520px] items-center justify-center rounded-[32px] border border-border bg-muted/30"
      >
        <p class="text-sm text-muted-foreground">
          {{ $t('common.loadingProfile') }}
        </p>
      </div>

      <div
        v-else-if="errorMessage"
        class="flex min-h-[520px] flex-col items-center justify-center rounded-[32px] border border-border bg-muted/30 px-6 text-center"
      >
        <h1 class="text-2xl font-semibold text-foreground">
          {{ $t('profile.notFound') }}
        </h1>

        <p class="mt-2 max-w-md text-sm text-muted-foreground">
          {{ errorMessage }}
        </p>

        <button
          class="mt-6 rounded-full bg-primary px-5 py-2.5 text-sm font-medium text-primary-foreground transition hover:opacity-90"
          @click="loadProfile"
        >
          {{ $t('common.tryAgain') }}
        </button>
      </div>

      <div v-else-if="profile" class="space-y-8">
        <section class="overflow-hidden rounded-[32px] border border-border bg-muted/30 p-6 sm:p-8 lg:p-10">
          <div class="grid gap-8 lg:grid-cols-[1fr_auto] lg:items-end">
            <div>
              <p class="text-[11px] uppercase tracking-[0.24em] text-muted-foreground">
                {{ $t('profile.userProfile') }}
              </p>

              <h1 class="mt-3 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl">
                {{ profile.name }}
              </h1>

              <div class="mt-4 flex flex-wrap items-center gap-2">
                <span class="rounded-full border border-border bg-background px-3 py-1 text-sm text-muted-foreground">
                  @{{ profile.username }}
                </span>

                <span class="rounded-full border border-border bg-background px-3 py-1 text-sm text-muted-foreground">
                  {{ $t('profile.verifiedSeller') }}
                </span>

                <span class="rounded-full border border-border bg-background px-3 py-1 text-sm text-muted-foreground">
                  {{ $t('profile.securePayments') }}
                </span>
              </div>

              <p class="mt-5 max-w-2xl text-sm leading-6 text-muted-foreground sm:text-base">
                {{ $t('profile.description') }}
              </p>
            </div>

            <div class="flex flex-col gap-3 sm:flex-row lg:justify-end">
              <button class="disabled rounded-full bg-primary px-5 py-3 text-sm font-medium text-primary-foreground transition hover:opacity-90">
                {{ $t('profile.follow') }}
              </button>

              <button class="rounded-full border border-border bg-background px-5 py-3 text-sm font-medium text-foreground transition hover:bg-accent hover:text-accent-foreground">
                {{ $t('profile.message') }}
              </button>
            </div>
          </div>
        </section>

        <section class="grid gap-4 sm:grid-cols-3">
          <div class="rounded-[24px] border border-border bg-background p-5">
            <p class="text-xs uppercase tracking-[0.2em] text-muted-foreground">
              {{ $t('profile.activeListings') }}
            </p>

            <p class="mt-3 text-3xl font-semibold text-foreground">
              {{ profile.stats.activeListingsCount }}
            </p>
          </div>

          <div class="rounded-[24px] border border-border bg-background p-5">
            <p class="text-xs uppercase tracking-[0.2em] text-muted-foreground">
              {{ $t('profile.soldItems') }}
            </p>

            <p class="mt-3 text-3xl font-semibold text-foreground">
              {{ profile.stats.soldItemsCount }}
            </p>
          </div>

          <div class="rounded-[24px] border border-border bg-background p-5">
            <p class="text-xs uppercase tracking-[0.2em] text-muted-foreground">
              {{ $t('profile.bidsPlaced') }}
            </p>

            <p class="mt-3 text-3xl font-semibold text-foreground">
              {{ profile.stats.bidsPlaced }}
            </p>
          </div>
        </section>

        <section class="grid gap-8 lg:grid-cols-[360px_1fr]">
          <aside class="space-y-6">
            <div class="rounded-[28px] border border-border bg-background p-6">
              <h2 class="text-xl font-semibold tracking-tight text-foreground">
                {{ $t('profile.aboutSeller') }}
              </h2>

              <div class="mt-6 space-y-5">
                <div>
                  <p class="text-xs uppercase tracking-[0.2em] text-muted-foreground">
                    {{ $t('profile.focus') }}
                  </p>

                  <p class="mt-2 text-sm leading-6 text-muted-foreground">
                    {{ $t('profile.focusDescription') }}
                  </p>
                </div>

                <div>
                  <p class="text-xs uppercase tracking-[0.2em] text-muted-foreground">
                    {{ $t('profile.accountStatus') }}
                  </p>

                  <div class="mt-3 flex flex-wrap gap-2">
                    <span class="rounded-full bg-muted px-3 py-1 text-sm text-muted-foreground">
                      {{ $t('profile.verifiedSeller') }}
                    </span>

                    <span class="rounded-full bg-muted px-3 py-1 text-sm text-muted-foreground">
                      {{ $t('profile.trustedHistory') }}
                    </span>

                    <span class="rounded-full bg-muted px-3 py-1 text-sm text-muted-foreground">
                      {{ $t('profile.buyerProtection') }}
                    </span>
                  </div>
                </div>
              </div>
            </div>

            <div class="rounded-[28px] border border-border bg-background p-6">
              <h2 class="text-xl font-semibold tracking-tight text-foreground">
                {{ $t('profile.sellerSummary') }}
              </h2>

              <div class="mt-5 space-y-3 text-sm text-muted-foreground">
                <div class="flex items-center justify-between gap-4">
                  <span>{{ $t('profile.liveLots') }}</span>
                  <span class="font-medium text-foreground">{{ activeListings.length }}</span>
                </div>

                <div class="flex items-center justify-between gap-4">
                  <span>{{ $t('profile.completedSales') }}</span>
                  <span class="font-medium text-foreground">{{ soldListings.length }}</span>
                </div>

                <div class="flex items-center justify-between gap-4">
                  <span>{{ $t('navigation.profile') }}</span>
                  <span class="font-medium text-foreground">{{ $t('profile.publicProfile') }}</span>
                </div>
              </div>
            </div>
          </aside>

          <div class="space-y-8">
            <section class="rounded-[28px] border border-border bg-background p-6">
              <div class="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
                <div>
                  <p class="text-[11px] uppercase tracking-[0.22em] text-muted-foreground">
                    {{ $t('profile.liveInventory') }}
                  </p>

                  <h2 class="mt-2 text-2xl font-semibold tracking-tight text-foreground">
                    {{ $t('profile.activeListings') }}
                  </h2>

                  <p class="mt-1 text-sm text-muted-foreground">
                    {{ $t('profile.activeListingsDescription') }}
                  </p>
                </div>

                <button class="text-sm font-medium text-foreground transition hover:text-muted-foreground">
                  {{ $t('profile.viewAll') }}
                </button>
              </div>

              <div v-if="activeListings.length" class="mt-6 grid gap-5 sm:grid-cols-2 xl:grid-cols-3">
                <RouterLink
                  v-for="listing in activeListings"
                  :key="listing.lotId"
                  :to="localePath(`/lots/${listing.lotId}`)"
                  class="group overflow-hidden rounded-[24px] border border-border bg-background transition hover:-translate-y-0.5 hover:shadow-sm"
                >
                  <div class="overflow-hidden bg-muted">
                    <img
                      :src="resolveImageUrl(listing.thumbnailUrl)"
                      :alt="listing.title"
                      class="h-64 w-full object-cover transition duration-300 group-hover:scale-[1.03]"
                    >
                  </div>

                  <div class="p-4">
                    <h3 class="line-clamp-2 text-base font-medium text-foreground">
                      {{ listing.title }}
                    </h3>

                    <p class="mt-2 text-sm text-muted-foreground">
                      {{ listing.brand || $t('profile.noBrand') }}
                    </p>

                    <div class="mt-4 flex items-center justify-between gap-3">
                      <span class="text-sm font-semibold text-foreground">
                        {{ formatPrice(listing.currentPrice, listing.currency) }}
                      </span>

                      <span class="rounded-full bg-muted px-2.5 py-1 text-xs text-muted-foreground">
                        {{ $t('profile.active') }}
                      </span>
                    </div>
                  </div>
                </RouterLink>
              </div>

              <div
                v-else
                class="mt-6 flex min-h-[220px] flex-col items-center justify-center rounded-[24px] border border-dashed border-border px-6 py-10 text-center"
              >
                <h3 class="text-base font-semibold text-foreground">
                  {{ $t('profile.noActiveListings') }}
                </h3>

                <p class="mt-2 max-w-sm text-sm leading-6 text-muted-foreground">
                  {{ $t('profile.noActiveDescription') }}
                </p>
              </div>
            </section>

            <section class="rounded-[28px] border border-border bg-background p-6">
              <div class="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
                <div>
                  <p class="text-[11px] uppercase tracking-[0.22em] text-muted-foreground">
                    {{ $t('profile.sellerHistory') }}
                  </p>

                  <h2 class="mt-2 text-2xl font-semibold tracking-tight text-foreground">
                    {{ $t('profile.soldItems') }}
                  </h2>

                  <p class="mt-1 text-sm text-muted-foreground">
                    {{ $t('profile.soldDescription') }}
                  </p>
                </div>

                <button class="text-sm font-medium text-foreground transition hover:text-muted-foreground">
                  {{ $t('profile.viewArchive') }}
                </button>
              </div>

              <div v-if="soldListings.length" class="mt-6 grid gap-5 sm:grid-cols-2 xl:grid-cols-3">
                <RouterLink
                  v-for="listing in soldListings"
                  :key="listing.lotId"
                  :to="localePath(`/lots/${listing.lotId}`)"
                  class="group overflow-hidden rounded-[24px] border border-border bg-background transition hover:-translate-y-0.5 hover:shadow-sm"
                >
                  <div class="relative overflow-hidden bg-muted">
                    <img
                      :src="resolveImageUrl(listing.thumbnailUrl)"
                      :alt="listing.title"
                      class="h-64 w-full object-cover transition duration-300 group-hover:scale-[1.03]"
                    >

                    <div class="absolute left-3 top-3 rounded-full bg-background px-3 py-1 text-xs font-medium text-foreground shadow-sm">
                      {{ $t('profile.sold') }}
                    </div>
                  </div>

                  <div class="p-4">
                    <h3 class="line-clamp-2 text-base font-medium text-foreground">
                      {{ listing.title }}
                    </h3>

                    <p class="mt-2 text-sm text-muted-foreground">
                      {{ listing.brand || $t('profile.noBrand') }}
                    </p>

                    <div class="mt-4 flex items-center justify-between gap-3">
                      <span class="text-sm font-semibold text-foreground">
                        {{ formatPrice(listing.currentPrice, listing.currency) }}
                      </span>

                      <span class="rounded-full bg-muted px-2.5 py-1 text-xs text-muted-foreground">
                        {{ $t('profile.sold') }}
                      </span>
                    </div>
                  </div>
                </RouterLink>
              </div>

              <div
                v-else
                class="mt-6 flex min-h-[220px] flex-col items-center justify-center rounded-[24px] border border-dashed border-border px-6 py-10 text-center"
              >
                <h3 class="text-base font-semibold text-foreground">
                  {{ $t('profile.noSoldItems') }}
                </h3>

                <p class="mt-2 max-w-sm text-sm leading-6 text-muted-foreground">
                  {{ $t('profile.noSoldDescription') }}
                </p>
              </div>
            </section>
          </div>
        </section>
      </div>
    </div>
  </main>
</template>
