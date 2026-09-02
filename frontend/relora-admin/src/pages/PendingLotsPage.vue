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

const loadPendingLots = async () => {
  try {
    isLoading.value = true
    errorMessage.value = ''
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
    errorMessage.value = ''
    successMessage.value = ''

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

const reject = async (lotId: string) => {
  try {
    isActing.value = true
    errorMessage.value = ''
    successMessage.value = ''

    await adminService.rejectLot(lotId)
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
  <section class="mx-auto max-w-6xl px-4 py-6 sm:px-6 lg:px-8">
    <div class="mb-4">
      <h2 class="text-2xl font-semibold">{{ $t('admin.pending.title') }}</h2>
      <p class="text-sm text-slate-600">{{ $t('admin.pending.description') }}</p>
    </div>

    <div v-if="isLoading" class="rounded-xl border bg-white p-4 text-sm text-slate-600">
      {{ $t('admin.pending.loading') }}
    </div>

    <div v-else>
      <div v-if="successMessage" class="mb-3 rounded-xl border border-emerald-300 bg-emerald-50 p-3 text-sm text-emerald-700">
        {{ successMessage }}
      </div>

      <div v-if="errorMessage" class="mb-3 rounded-xl border border-red-300 bg-red-50 p-3 text-sm text-red-700">
        {{ errorMessage }}
      </div>

      <div v-if="lots.length === 0" class="rounded-xl border bg-white p-4 text-sm text-slate-600">
        {{ $t('admin.pending.empty') }}
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
    </div>
  </section>
</template>
