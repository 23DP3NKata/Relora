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
  <section class="mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8">
    <div class="mb-6 flex flex-wrap items-end justify-between gap-3">
      <div>
        <div class="flex items-center gap-2">
          <h2 class="text-2xl font-semibold tracking-tight">{{ $t('admin.pending.title') }}</h2>
          <span
            v-if="!isLoading"
            class="rounded-full bg-slate-900 px-2.5 py-0.5 text-xs font-semibold text-white"
          >
            {{ lots.length }}
          </span>
        </div>
        <p class="mt-1 text-sm text-slate-500">{{ $t('admin.pending.description') }}</p>
      </div>

      <button
        type="button"
        class="rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm font-medium text-slate-700 transition hover:bg-slate-50 disabled:opacity-50"
        :disabled="isLoading || isActing"
        @click="loadPendingLots"
      >
        {{ $t('common.refresh') }}
      </button>
    </div>

    <div
      v-if="successMessage"
      class="mb-4 flex items-center justify-between gap-3 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700"
    >
      <span>{{ successMessage }}</span>
      <button type="button" class="text-emerald-700/70 hover:text-emerald-900" :aria-label="$t('common.close')" @click="successMessage = ''">✕</button>
    </div>

    <div
      v-if="errorMessage"
      class="mb-4 flex items-center justify-between gap-3 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
    >
      <span>{{ errorMessage }}</span>
      <button type="button" class="text-red-700/70 hover:text-red-900" :aria-label="$t('common.close')" @click="errorMessage = ''">✕</button>
    </div>

    <div v-if="isLoading" class="space-y-3">
      <div v-for="n in 3" :key="n" class="h-36 animate-pulse rounded-2xl bg-white shadow-sm" />
    </div>

    <div
      v-else-if="lots.length === 0 && !errorMessage"
      class="rounded-2xl border border-dashed border-slate-300 bg-white px-6 py-12 text-center"
    >
      <p class="text-base font-medium">{{ $t('admin.pending.empty') }}</p>
      <p class="mt-1 text-sm text-slate-500">{{ $t('admin.pending.emptyHint') }}</p>
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
