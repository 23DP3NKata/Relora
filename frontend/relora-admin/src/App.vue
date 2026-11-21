<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router'

import LanguageSwitcher from '@/components/LanguageSwitcher.vue'
import { authService, currentUser } from '@/services/authService'

const route = useRoute()
const router = useRouter()

const links = [
  { to: '/admin/dashboard', label: 'navigation.dashboard' },
  { to: '/admin/pending', label: 'navigation.pendingLots' },
  { to: '/admin/disputes', label: 'navigation.disputes' },
  { to: '/admin/overdue-shipments', label: 'navigation.overdueShipments' },
  { to: '/admin/support', label: 'navigation.support' },
]

const logout = async () => {
  await authService.logout()
  router.push('/login')
}
</script>

<template>
  <div class="min-h-screen">
    <header v-if="!route.meta.public" class="sticky top-0 z-10 border-b border-neutral-200 bg-white/95 backdrop-blur">
      <div class="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-x-6 gap-y-3 px-4 py-4 sm:px-6 lg:px-8">
        <RouterLink to="/admin/dashboard" class="flex items-baseline gap-2">
          <span class="text-2xl font-semibold tracking-tight">Relora</span>
          <span class="eyebrow">{{ $t('admin.badge') }}</span>
        </RouterLink>

        <div class="flex items-center gap-3">
          <span v-if="currentUser" class="hidden text-sm text-neutral-500 sm:inline">{{ currentUser.username }}</span>
          <LanguageSwitcher />
          <button type="button" class="btn py-1.5" @click="logout">{{ $t('login.signOut') }}</button>
        </div>

        <nav class="flex w-full flex-wrap gap-x-5 gap-y-2 text-sm">
          <RouterLink
            v-for="link in links"
            :key="link.to"
            :to="link.to"
            class="border-b pb-1 transition hover:text-neutral-950"
            :class="route.path.startsWith(link.to) ? 'border-neutral-950 text-neutral-950' : 'border-transparent text-neutral-500'"
          >
            {{ $t(link.label) }}
          </RouterLink>
        </nav>
      </div>
    </header>

    <main>
      <RouterView />
    </main>
  </div>
</template>
