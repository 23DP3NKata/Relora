<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/authStore'

import AnnouncementBanner from '@/components/layout/header/AnnouncementBanner.vue'
import Container from '@/components/ui/Container.vue'
import Input from '@/components/ui/input/Input.vue'
import ModeToggle from '@/components/theme/ModeToggle.vue'
import LanguageSwitcher from '@/components/layout/LanguageSwitcher.vue'
import Categories from '@/components/layout/CategoriesNavbar.vue'
import UserProfileIcon from '@/components/ui/icons/UserProfileIcon.vue'
import MobileMenu from './header/MobileMenu.vue'
import { useLocalePath } from '@/composables/useLocalePath'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'

const router = useRouter()
const localePath = useLocalePath()
const authStore = useAuthStore()
const { user, isAuthenticated } = storeToRefs(authStore)

const searchQuery = ref('')
const isMobileMenuOpen = ref(false)

function closeMobileMenu() {
  isMobileMenuOpen.value = false
}

function openMobileMenu() {
  isMobileMenuOpen.value = true
}

function handleLogout() {
  try {
    authStore.logout()
    closeMobileMenu()
    router.push(localePath('/home'))
  } catch (error) {
    console.error('Logout failed:', error)
  }
}

function handleSearch() {
  const query = searchQuery.value.trim()

  router.push(localePath({
    name: 'search',
    query: {
      query: query,
    },
  }))
}

function handleMobileOverlayKeydown(event: KeyboardEvent) {
  if (event.key === 'Escape') {
    closeMobileMenu()
  }
}

watch(isMobileMenuOpen, (isOpen) => {
  if (typeof document === 'undefined') {
    return
  }

  document.body.style.overflow = isOpen ? 'hidden' : ''
})

onMounted(() => {
  window.addEventListener('keydown', handleMobileOverlayKeydown)
})

onBeforeUnmount(() => {
  if (typeof document !== 'undefined') {
    document.body.style.overflow = ''
  }

  window.removeEventListener('keydown', handleMobileOverlayKeydown)
})

</script>

<template>
  <header class="sticky top-0 z-50 border-b border-border bg-background/95 backdrop-blur-md">
    <Container>

      <AnnouncementBanner
        :title="$t('announcement.title')"
        :description="$t('announcement.description')"
      />

      <div class="grid min-h-[76px] grid-cols-[1fr_auto_1fr] items-center gap-3 py-3 md:flex md:items-center md:justify-between md:gap-4">
        <div class="flex items-center gap-1 md:hidden">
          <button
            type="button"
            class="inline-flex h-10 w-10 items-center justify-center rounded-full border border-border bg-background text-foreground transition hover:bg-accent hover:text-accent-foreground"
            :aria-label="$t('common.openMenu')"
            @click="openMobileMenu"
          >
            <svg class="h-5 w-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round">
              <path d="M4 7h16" />
              <path d="M4 12h16" />
              <path d="M4 17h16" />
            </svg>
          </button>
        </div>

        <RouterLink
          :to="localePath('/home')"
          class="justify-self-center text-2xl font-semibold tracking-tight text-foreground transition hover:opacity-80 md:justify-self-start"
        >
          Relora
        </RouterLink>

        <div class="hidden md:flex md:flex-1 md:justify-center md:px-4">
          <div class="w-full max-w-lg">
            <div class="relative">
              <Input
                v-model="searchQuery"
                @keydown.enter="handleSearch"
                :placeholder="$t('search.placeholder')"
                class="h-10 rounded-full border border-input bg-muted pl-10 pr-20 text-sm text-foreground placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-0"
              />

              <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3.5">
                <svg class="h-4 w-4 text-muted-foreground" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
              </div>

              <button
                class="absolute right-1 top-1/2 -translate-y-1/2 rounded-full bg-primary px-3 py-1.5 text-xs font-medium text-primary-foreground transition hover:opacity-90"
                @click="handleSearch"
              >
                {{ $t('common.search') }}
              </button>
            </div>
          </div>
        </div>

        <div class="hidden shrink-0 items-center gap-2 sm:gap-3 md:flex">
          <RouterLink
            :to="localePath(isAuthenticated ? '/sell' : '/login')"
            class="inline-flex rounded-full bg-primary px-4 py-2 text-sm font-medium text-primary-foreground transition hover:opacity-90"
          >
            {{ $t('navigation.sellWithUs') }}
          </RouterLink>

          <RouterLink
            v-if="!isAuthenticated"
            :to="localePath('/login')"
            class="inline-flex rounded-full border border-border px-4 py-2 text-sm font-medium text-foreground transition hover:bg-accent hover:text-accent-foreground"
          >
            {{ $t('navigation.signIn') }}
          </RouterLink>

          <div v-if="isAuthenticated">
            <Select>
              <SelectTrigger class="h-10 w-full rounded-full border border-border bg-background px-3 text-sm">
                <SelectValue>
                  <UserProfileIcon />
                </SelectValue>
              </SelectTrigger>

              <SelectContent>
                <RouterLink :to="localePath(`/profile/${user?.username}`)">
                  <SelectItem value="profile">
                    {{ $t('navigation.profile') }}
                  </SelectItem>
                </RouterLink>

                <RouterLink :to="localePath('/orders')">
                  <SelectItem value="orders">
                    {{ $t('navigation.yourOrders') }}
                  </SelectItem>
                </RouterLink>

                <RouterLink :to="localePath('/listings')">
                  <SelectItem value="listings">
                    {{ $t('navigation.myListings') }}
                  </SelectItem>
                </RouterLink>

                <RouterLink :to="localePath('/settings')">
                  <SelectItem value="settings">
                    {{ $t('navigation.settings') }}
                  </SelectItem>
                </RouterLink>

                <SelectItem value="logout" @click="handleLogout">
                  {{ $t('navigation.signOut') }}
                </SelectItem>
              </SelectContent>
            </Select>
          </div>

          <LanguageSwitcher />
          <ModeToggle />
        </div>
      </div>
    </Container>

    <div class="hidden md:block md:justify-center">
      <Categories />
    </div>

    <MobileMenu
      v-model:search-query="searchQuery"
      :is-open="isMobileMenuOpen"
      :is-authenticated="isAuthenticated"
      :user="user"
      @close="closeMobileMenu"
      @search="handleSearch"
      @logout="handleLogout"
    />
  </header>
</template>
