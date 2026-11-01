<script setup lang="ts">
import LanguageSwitcher from '@/components/LanguageSwitcher.vue'
</script>

<template>
  <div class="min-h-screen bg-slate-100 text-slate-900">
    <header class="sticky top-0 z-10 border-b border-slate-200 bg-white/90 backdrop-blur">
      <div class="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-3 px-4 py-3 sm:px-6 lg:px-8">
        <div class="flex items-center gap-3">
          <span class="flex h-9 w-9 items-center justify-center rounded-lg bg-slate-900 text-sm font-bold text-white">R</span>
          <h1 class="text-lg font-semibold tracking-tight">{{ $t('admin.title') }}</h1>
        </div>

        <nav class="flex flex-wrap items-center gap-1 text-sm">
          <RouterLink
            to="/admin/dashboard"
            class="rounded-lg px-3 py-2 font-medium text-slate-600 transition hover:bg-slate-100 hover:text-slate-900"
            active-class="bg-slate-900 text-white hover:bg-slate-900 hover:text-white"
          >
            {{ $t('navigation.dashboard') }}
          </RouterLink>
          <RouterLink
            to="/admin/pending"
            class="rounded-lg px-3 py-2 font-medium text-slate-600 transition hover:bg-slate-100 hover:text-slate-900"
            active-class="bg-slate-900 text-white hover:bg-slate-900 hover:text-white"
          >
            {{ $t('navigation.pendingLots') }}
          </RouterLink>
          <span class="mx-2 hidden h-6 w-px bg-slate-200 sm:block" />
          <LanguageSwitcher />
        </nav>
      </div>
    </header>

    <main>
      <RouterView />
    </main>
  </div>
</template>
