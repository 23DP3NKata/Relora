<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'

import SupportRequestCard from '@/components/SupportRequestCard.vue'
import { supportService } from '@/services/supportService'
import type { SupportRequest, SupportRequestStatus } from '@/types/support'

const { t } = useI18n()
const isLoading = ref(true)
const isActing = ref(false)
const errorMessage = ref('')
const requests = ref<SupportRequest[]>([])
const showClosed = ref(false)

const openCount = computed(() => requests.value.filter((r) => r.statusName !== 'Closed').length)

// slēgtos rāda tikai ja ieķeksē
const visibleRequests = computed(() =>
  showClosed.value ? requests.value : requests.value.filter((r) => r.statusName !== 'Closed'),
)

const loadRequests = async () => {
  try {
    isLoading.value = true
    errorMessage.value = ''
    requests.value = await supportService.getSupportRequests()
  } catch (error) {
    console.error(error)
    errorMessage.value = t('admin.support.loadFailed')
  } finally {
    isLoading.value = false
  }
}

const changeStatus = async (requestId: string, status: SupportRequestStatus) => {
  try {
    isActing.value = true
    errorMessage.value = ''
    await supportService.changeStatus(requestId, status)

    const request = requests.value.find((r) => r.id === requestId)
    if (request) {
      request.statusName = status
    }
  } catch (error) {
    console.error(error)
    errorMessage.value = t('admin.support.changeFailed')
  } finally {
    isActing.value = false
  }
}

onMounted(loadRequests)
</script>

<template>
  <section class="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
    <div class="mb-8 flex flex-wrap items-end justify-between gap-4">
      <div>
        <p class="eyebrow">{{ $t('admin.badge') }}</p>
        <h1 class="mt-2 text-3xl font-semibold tracking-tight">
          {{ $t('admin.support.title') }}
          <span v-if="!isLoading" class="text-neutral-400">{{ openCount }}</span>
        </h1>
        <p class="mt-2 text-sm text-neutral-500">{{ $t('admin.support.description') }}</p>
      </div>

      <div class="flex items-center gap-4">
        <label class="flex items-center gap-2 text-sm text-neutral-500">
          <input v-model="showClosed" type="checkbox" class="rounded border-neutral-300 accent-neutral-950" />
          {{ $t('admin.support.showClosed') }}
        </label>
        <button type="button" class="btn" :disabled="isLoading || isActing" @click="loadRequests">
          {{ $t('common.refresh') }}
        </button>
      </div>
    </div>

    <p v-if="errorMessage" class="card mb-4 px-4 py-3 text-sm text-red-600">{{ errorMessage }}</p>

    <div v-if="isLoading" class="space-y-3">
      <div v-for="n in 3" :key="n" class="card h-40 animate-pulse bg-neutral-50" />
    </div>

    <div v-else-if="visibleRequests.length === 0 && !errorMessage" class="card px-6 py-12 text-center">
      <p class="text-lg font-semibold">{{ $t('admin.support.empty') }}</p>
      <p class="mt-2 text-sm text-neutral-500">{{ $t('admin.support.emptyHint') }}</p>
    </div>

    <div v-else class="space-y-3">
      <SupportRequestCard
        v-for="request in visibleRequests"
        :key="request.id"
        :request="request"
        :is-acting="isActing"
        @change-status="changeStatus"
      />
    </div>
  </section>
</template>
