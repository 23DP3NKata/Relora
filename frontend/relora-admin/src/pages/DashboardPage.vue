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
    { key: 'support', label: t('admin.dashboard.newSupportRequestsCount'), value: dashboard.value.newSupportRequestsCount },
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
  <section class="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
    <div class="mb-8 flex flex-wrap items-end justify-between gap-4">
      <div>
        <p class="eyebrow">{{ $t('admin.badge') }}</p>
        <h1 class="mt-2 text-3xl font-semibold tracking-tight">{{ $t('admin.dashboard.title') }}</h1>
        <p class="mt-2 text-sm text-neutral-500">{{ $t('admin.dashboard.description') }}</p>
      </div>
      <button type="button" class="btn" :disabled="isLoading" @click="loadDashboard">
        {{ $t('common.refresh') }}
      </button>
    </div>

    <div v-if="isLoading" class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <div v-for="n in 4" :key="n" class="card h-32 animate-pulse bg-neutral-50" />
    </div>

    <p v-else-if="errorMessage" class="card p-5 text-sm text-red-600">{{ errorMessage }}</p>

    <div v-else class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <article v-for="stat in stats" :key="stat.key" class="card p-5">
        <p class="eyebrow">{{ stat.label }}</p>
        <p class="mt-3 text-4xl font-semibold tracking-tight tabular-nums">{{ stat.value }}</p>
        <RouterLink
          v-if="stat.key === 'support'"
          to="/admin/support"
          class="mt-4 inline-flex text-sm text-neutral-500 underline-offset-4 hover:text-neutral-950 hover:underline"
        >
          {{ $t('admin.dashboard.goToSupport') }} →
        </RouterLink>
        <RouterLink
          v-if="stat.key === 'pending'"
          to="/admin/pending"
          class="mt-4 inline-flex text-sm text-neutral-500 underline-offset-4 hover:text-neutral-950 hover:underline"
        >
          {{ $t('admin.dashboard.goToPending') }} →
        </RouterLink>
      </article>
    </div>
  </section>
</template>
