<script setup lang="ts">
import LanguageSwitcher from '@/components/LanguageSwitcher.vue'
</script>

<template>
  <div class="min-h-screen bg-slate-50 text-slate-900">
    <header class="border-b bg-white">
      <div class="mx-auto flex max-w-6xl items-center justify-between px-4 py-4 sm:px-6 lg:px-8">
        <h1 class="text-xl font-semibold">{{ $t('admin.title') }}</h1>

        <nav class="flex items-center gap-2 text-sm">
          <RouterLink to="/admin/pending" class="rounded-md border px-3 py-1.5 hover:bg-slate-100">
            {{ $t('navigation.pendingLots') }}
          </RouterLink>
          <RouterLink to="/admin/dashboard" class="rounded-md border px-3 py-1.5 hover:bg-slate-100">
            {{ $t('navigation.dashboard') }}
          </RouterLink>
          <LanguageSwitcher />
        </nav>
      </div>
    </header>

    <main>
      <RouterView />
    </main>
  </div>
</template>
