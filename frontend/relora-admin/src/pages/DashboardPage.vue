<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { adminService, type AdminDashboardData } from '@/services/adminService'

const { t } = useI18n()
const isLoading = ref(true)
const errorMessage = ref('')
const dashboard = ref<AdminDashboardData | null>(null)

const entries = computed(() => {
  if (!dashboard.value) {
    return []
  }

  return Object.entries(dashboard.value)
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
  <section class="mx-auto max-w-6xl px-4 py-6 sm:px-6 lg:px-8">
    <div class="mb-4">
      <h2 class="text-2xl font-semibold">{{ $t('admin.dashboard.title') }}</h2>
      <p class="text-sm text-slate-600">{{ $t('admin.dashboard.description') }}</p>
    </div>

    <div v-if="isLoading" class="rounded-xl border bg-white p-4 text-sm text-slate-600">
      {{ $t('admin.dashboard.loading') }}
    </div>

    <div v-else-if="errorMessage" class="rounded-xl border border-red-300 bg-red-50 p-4 text-sm text-red-700">
      {{ errorMessage }}
    </div>

    <div v-else-if="entries.length === 0" class="rounded-xl border bg-white p-4 text-sm text-slate-600">
      {{ $t('admin.dashboard.empty') }}
    </div>

    <div v-else class="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
      <article v-for="[key, value] in entries" :key="key" class="rounded-xl border bg-white p-4">
        <p class="text-xs uppercase tracking-wide text-slate-500">{{ key }}</p>
        <p class="mt-2 text-xl font-semibold">{{ value ?? '—' }}</p>
      </article>
    </div>
  </section>
</template>
