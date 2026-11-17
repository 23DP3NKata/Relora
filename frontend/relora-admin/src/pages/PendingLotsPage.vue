<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'

import PendingLotCard from '@/components/PendingLotCard.vue'
import { adminService } from '@/services/adminService'
import { adminLotService } from '@/services/lotService'
import type { LotPreview } from '@/types/lot'

const { t } = useI18n()
const isLoading = ref(true)
const isActing = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const lots = ref<LotPreview[]>([])

const clearMessages = () => {
  errorMessage.value = ''
  successMessage.value = ''
}

const loadPendingLots = async () => {
  try {
    isLoading.value = true
    clearMessages()
    lots.value = await adminLotService.getPendingLots()
  } catch (error) {
    console.error(error)
    errorMessage.value = t('admin.pending.loadFailed')
  } finally {
    isLoading.value = false
  }
}

const approve = async (lotId: string) => {
  try {
    isActing.value = true
    clearMessages()

    await adminService.acceptLot(lotId)
    lots.value = lots.value.filter((lot) => lot.id !== lotId)
    successMessage.value = t('admin.pending.approved')
  } catch (error) {
    console.error(error)
    errorMessage.value = t('admin.pending.approveFailed')
  } finally {
    isActing.value = false
  }
}

const reject = async (lotId: string, reason: string) => {
  try {
    isActing.value = true
    clearMessages()

    await adminService.rejectLot(lotId, reason)
    lots.value = lots.value.filter((lot) => lot.id !== lotId)
    successMessage.value = t('admin.pending.rejected')
  } catch (error) {
    console.error(error)
    errorMessage.value = t('admin.pending.rejectFailed')
  } finally {
    isActing.value = false
  }
}

onMounted(loadPendingLots)
</script>

<template>
  <section class="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
    <div class="mb-8 flex flex-wrap items-end justify-between gap-4">
      <div>
        <p class="eyebrow">{{ $t('admin.badge') }}</p>
        <h1 class="mt-2 text-3xl font-semibold tracking-tight">
          {{ $t('admin.pending.title') }}
          <span v-if="!isLoading" class="text-neutral-400">{{ lots.length }}</span>
        </h1>
        <p class="mt-2 text-sm text-neutral-500">{{ $t('admin.pending.description') }}</p>
      </div>
      <button type="button" class="btn" :disabled="isLoading || isActing" @click="loadPendingLots">
        {{ $t('common.refresh') }}
      </button>
    </div>

    <p v-if="successMessage" class="card mb-4 px-4 py-3 text-sm">{{ successMessage }}</p>
    <p v-if="errorMessage" class="card mb-4 px-4 py-3 text-sm text-red-600">{{ errorMessage }}</p>

    <div v-if="isLoading" class="space-y-3">
      <div v-for="n in 3" :key="n" class="card h-36 animate-pulse bg-neutral-50" />
    </div>

    <div v-else-if="lots.length === 0 && !errorMessage" class="card px-6 py-12 text-center">
      <p class="text-lg font-semibold">{{ $t('admin.pending.empty') }}</p>
      <p class="mt-2 text-sm text-neutral-500">{{ $t('admin.pending.emptyHint') }}</p>
    </div>

    <div v-else class="space-y-3">
      <PendingLotCard
        v-for="lot in lots"
        :key="lot.id"
        :lot="lot"
        :is-acting="isActing"
        @approve="approve"
        @reject="reject"
      />
    </div>
  </section>
</template>
