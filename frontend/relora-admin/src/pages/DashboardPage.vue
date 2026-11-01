<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { adminService, type AdminDashboardData } from '@/services/adminService'

const { t } = useI18n()
const isLoading = ref(true)
const errorMessage = ref('')
const dashboard = ref<AdminDashboardData | null>(null)

const stats = computed(() => {
  if (!dashboard.value) {
    return []
  }

  return [
    { key: 'pending', label: t('admin.dashboard.pendingLotsCount'), value: dashboard.value.pendingLotsCount },
    { key: 'auctions', label: t('admin.dashboard.activeAuctionsCount'), value: dashboard.value.activeAuctionsCount },
    { key: 'users', label: t('admin.dashboard.usersCount'), value: dashboard.value.usersCount },
  ]
})

const loadDashboard = async () => {
  try {
    isLoading.value = true
    errorMessage.value = ''
    dashboard.value = await adminService.getDashboard()
  } catch (error) {
    console.error(error)
    errorMessage.value = t('admin.dashboard.loadFailed')
  } finally {
    isLoading.value = false
  }
}

onMounted(loadDashboard)
</script>

<template>
  <section class="mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8">
    <div class="mb-6 flex flex-wrap items-end justify-between gap-3">
      <div>
        <h2 class="text-2xl font-semibold tracking-tight">{{ $t('admin.dashboard.title') }}</h2>
        <p class="mt-1 text-sm text-slate-500">{{ $t('admin.dashboard.description') }}</p>
      </div>

      <button
        type="button"
        class="rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm font-medium text-slate-700 transition hover:bg-slate-50 disabled:opacity-50"
        :disabled="isLoading"
        @click="loadDashboard"
      >
        {{ $t('common.refresh') }}
      </button>
    </div>

    <div v-if="isLoading" class="grid gap-4 sm:grid-cols-3">
      <div v-for="n in 3" :key="n" class="h-32 animate-pulse rounded-2xl bg-white shadow-sm" />
    </div>

    <div v-else-if="errorMessage" class="rounded-2xl border border-red-200 bg-red-50 p-4 text-sm text-red-700">
      {{ errorMessage }}
    </div>

    <div v-else class="grid gap-4 sm:grid-cols-3">
      <article
        v-for="stat in stats"
        :key="stat.key"
        class="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
        :class="stat.key === 'pending' && stat.value > 0 ? 'border-amber-300 ring-1 ring-amber-200' : ''"
      >
        <p class="text-sm font-medium text-slate-500">{{ stat.label }}</p>
        <p class="mt-3 text-4xl font-semibold tracking-tight">{{ stat.value }}</p>

        <RouterLink
          v-if="stat.key === 'pending'"
          to="/admin/pending"
          class="mt-4 inline-flex text-sm font-medium text-amber-700 hover:text-amber-800"
        >
          {{ $t('admin.dashboard.goToPending') }} →
        </RouterLink>
      </article>
    </div>
  </section>
</template>
