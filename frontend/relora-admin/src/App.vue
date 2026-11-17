<script setup lang="ts">
import { useRoute } from 'vue-router'

import LanguageSwitcher from '@/components/LanguageSwitcher.vue'

const route = useRoute()

const links = [
  { to: '/admin/dashboard', label: 'navigation.dashboard' },
  { to: '/admin/pending', label: 'navigation.pendingLots' },
  { to: '/admin/disputes', label: 'navigation.disputes' },
  { to: '/admin/overdue-shipments', label: 'navigation.overdueShipments' },
]
</script>

<template>
  <div class="min-h-screen">
    <header class="sticky top-0 z-10 border-b border-neutral-200 bg-white/95 backdrop-blur">
      <div class="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-x-6 gap-y-3 px-4 py-4 sm:px-6 lg:px-8">
        <RouterLink to="/admin/dashboard" class="flex items-baseline gap-2">
          <span class="text-2xl font-semibold tracking-tight">Relora</span>
          <span class="eyebrow">{{ $t('admin.badge') }}</span>
        </RouterLink>

        <LanguageSwitcher />

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
