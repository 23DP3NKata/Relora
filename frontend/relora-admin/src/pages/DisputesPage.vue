<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'

import DisputeCard from '@/components/DisputeCard.vue'
import { disputeService } from '@/services/disputeService'
import type { DisputeDecision, OrderDispute } from '@/types/dispute'

const { t } = useI18n()
const isLoading = ref(true)
const isActing = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const disputes = ref<OrderDispute[]>([])
const showResolved = ref(false)

const openCount = computed(() => disputes.value.filter((d) => d.disputeStatusName === 'Open').length)

const visibleDisputes = computed(() =>
  showResolved.value ? disputes.value : disputes.value.filter((d) => d.disputeStatusName === 'Open'),
)

const clearMessages = () => {
  errorMessage.value = ''
  successMessage.value = ''
}

const loadDisputes = async () => {
  try {
    isLoading.value = true
    clearMessages()
    disputes.value = await disputeService.getDisputes()
  } catch (error) {
    console.error(error)
    errorMessage.value = t('admin.disputes.loadFailed')
  } finally {
    isLoading.value = false
  }
}

const resolve = async (disputeId: string, decision: DisputeDecision, reason: string) => {
  try {
    isActing.value = true
    clearMessages()

    await disputeService.resolveDispute(disputeId, decision, reason)
    // ielādē vēlreiz, lai redz lēmumu
    disputes.value = await disputeService.getDisputes()
    successMessage.value = t('admin.disputes.resolved')
  } catch (error) {
    console.error(error)
    errorMessage.value = t('admin.disputes.resolveFailed')
  } finally {
    isActing.value = false
  }
}

onMounted(loadDisputes)
</script>

<template>
  <section class="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
    <div class="mb-8 flex flex-wrap items-end justify-between gap-4">
      <div>
        <p class="eyebrow">{{ $t('admin.badge') }}</p>
        <h1 class="mt-2 text-3xl font-semibold tracking-tight">
          {{ $t('admin.disputes.title') }}
          <span v-if="!isLoading" class="text-neutral-400">{{ openCount }}</span>
        </h1>
        <p class="mt-2 text-sm text-neutral-500">{{ $t('admin.disputes.description') }}</p>
      </div>

      <div class="flex items-center gap-4">
        <label class="flex items-center gap-2 text-sm text-neutral-500">
          <input v-model="showResolved" type="checkbox" class="rounded border-neutral-300 accent-neutral-950" />
          {{ $t('admin.disputes.showResolved') }}
        </label>
        <button type="button" class="btn" :disabled="isLoading || isActing" @click="loadDisputes">
          {{ $t('common.refresh') }}
        </button>
      </div>
    </div>

    <p v-if="successMessage" class="card mb-4 px-4 py-3 text-sm">{{ successMessage }}</p>
    <p v-if="errorMessage" class="card mb-4 px-4 py-3 text-sm text-red-600">{{ errorMessage }}</p>

    <div v-if="isLoading" class="space-y-3">
      <div v-for="n in 3" :key="n" class="card h-48 animate-pulse bg-neutral-50" />
    </div>

    <div v-else-if="visibleDisputes.length === 0 && !errorMessage" class="card px-6 py-12 text-center">
      <p class="text-lg font-semibold">{{ $t('admin.disputes.empty') }}</p>
      <p class="mt-2 text-sm text-neutral-500">{{ $t('admin.disputes.emptyHint') }}</p>
    </div>

    <div v-else class="space-y-3">
      <DisputeCard
        v-for="dispute in visibleDisputes"
        :key="dispute.disputeId"
        :dispute="dispute"
        :is-acting="isActing"
        @resolve="resolve"
      />
    </div>
  </section>
</template>
