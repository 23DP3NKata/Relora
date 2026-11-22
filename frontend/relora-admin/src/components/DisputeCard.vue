<script setup lang="ts">
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'

import type { DisputeDecision, OrderDispute } from '@/types/dispute'

const props = defineProps<{
  dispute: OrderDispute
  isActing: boolean
}>()

const emit = defineEmits<{
  resolve: [disputeId: string, decision: DisputeDecision, reason: string]
}>()

const { locale, t } = useI18n()

const reasonMaxLength = 2000

const mediaBaseUrl = import.meta.env.VITE_MEDIA_PUBLIC_BASE_URL ?? ''

const decision = ref<DisputeDecision | null>(null)
const reason = ref('')
const reasonError = ref('')

const isOpen = computed(() => props.dispute.disputeStatusName === 'Open')

const photoUrl = (key: string) => `${mediaBaseUrl}/${key}`

const formatDate = (value: string | null) => {
  if (!value) return t('common.unavailable')
  return new Date(value).toLocaleString(locale.value, { dateStyle: 'medium', timeStyle: 'short' })
}

const priceLabel = computed(() =>
  new Intl.NumberFormat(locale.value, { style: 'currency', currency: props.dispute.currency || 'EUR' }).format(props.dispute.totalPrice),
)

const shortId = (id: string) => id.slice(0, 8)

const chooseDecision = (value: DisputeDecision) => {
  decision.value = value
  reasonError.value = ''
}

const cancelDecision = () => {
  decision.value = null
  reason.value = ''
  reasonError.value = ''
}

const submit = () => {
  const trimmed = reason.value.trim()

  if (!trimmed) {
    reasonError.value = t('admin.disputes.reasonRequired')
    return
  }

  if (decision.value) {
    emit('resolve', props.dispute.disputeId, decision.value, trimmed)
  }
}
</script>

<template>
  <article class="card p-4 sm:p-5">
    <div class="flex items-start gap-4 sm:gap-5">
      <div class="aspect-[4/5] w-20 shrink-0 sm:w-24 overflow-hidden rounded-xl border border-neutral-200 bg-neutral-50">
        <img
          v-if="dispute.lotPhotoKey"
          :src="photoUrl(dispute.lotPhotoKey)"
          :alt="dispute.lotTitle || $t('admin.pending.lotImage')"
          class="h-full w-full object-cover"
        />
        <div v-else class="flex h-full w-full items-center justify-center p-2 text-center text-xs text-neutral-400">
          {{ $t('admin.pending.noImage') }}
        </div>
      </div>

      <div class="min-w-0 flex-1">
        <div class="flex items-start justify-between gap-3">
          <p class="eyebrow truncate">{{ dispute.lotBrand || $t('admin.pending.unknownBrand') }}</p>
          <span class="tag shrink-0">{{ $t(`admin.disputes.status.${dispute.disputeStatusName}`) }}</span>
        </div>
        <h3 class="mt-1 truncate text-base font-semibold sm:text-lg">{{ dispute.lotTitle || $t('admin.pending.untitledLot') }}</h3>
        <p class="mt-1 text-lg font-semibold tabular-nums">{{ priceLabel }}</p>

        <p class="mt-2 text-xs leading-5 text-neutral-400">
          {{ $t('admin.disputes.order') }} <span class="font-mono" :title="dispute.orderId">{{ shortId(dispute.orderId) }}</span>
          <span class="mx-1.5">·</span>
          {{ $t('admin.disputes.buyer') }} <span class="font-mono" :title="dispute.buyerId">{{ shortId(dispute.buyerId) }}</span>
          <span class="mx-1.5">·</span>
          {{ $t('admin.pending.seller') }} <span class="font-mono" :title="dispute.sellerId">{{ shortId(dispute.sellerId) }}</span>
          <span class="mx-1.5">·</span>
          {{ $t('admin.disputes.openedAt') }} {{ formatDate(dispute.openedAtUtc) }}
          <template v-if="dispute.trackingNumber">
            <span class="mx-1.5">·</span>
            {{ $t('admin.disputes.tracking') }} {{ dispute.carrierName }} {{ dispute.trackingNumber }}
          </template>
        </p>
      </div>
    </div>

    <div class="mt-4 border-t border-neutral-200 pt-4">
      <p class="eyebrow">{{ $t('admin.disputes.reason') }}</p>
      <p class="mt-1 text-sm font-medium">{{ $t(`admin.disputes.reasons.${dispute.reasonName}`) }}</p>
      <p class="mt-1 whitespace-pre-line text-sm leading-6 text-neutral-600">{{ dispute.description }}</p>

      <template v-if="dispute.evidence.length > 0">
        <p class="eyebrow mt-4">{{ $t('admin.disputes.evidence') }}</p>
        <div class="mt-2 flex flex-wrap gap-2">
          <a
            v-for="item in dispute.evidence"
            :key="item.id"
            :href="photoUrl(item.key)"
            target="_blank"
            rel="noopener"
            class="h-20 w-20 overflow-hidden rounded-xl border border-neutral-200 bg-neutral-50"
          >
            <img :src="photoUrl(item.key)" :alt="$t('admin.disputes.evidence')" class="h-full w-full object-cover" />
          </a>
        </div>
      </template>
      <p v-else class="mt-3 text-xs text-neutral-400">{{ $t('admin.disputes.noEvidence') }}</p>
    </div>

    <!-- ja strīds jau atrisināts, parāda lēmumu -->
    <div v-if="!isOpen" class="mt-4 border-t border-neutral-200 pt-4">
      <p class="eyebrow">{{ $t('admin.disputes.decision') }}</p>
      <p class="mt-1 text-sm font-medium">
        {{ dispute.decisionName ? $t(`admin.disputes.decisions.${dispute.decisionName}`) : $t('common.unavailable') }}
      </p>
      <p v-if="dispute.decisionReason" class="mt-1 whitespace-pre-line text-sm text-neutral-600">{{ dispute.decisionReason }}</p>
      <p class="mt-1 text-xs text-neutral-400">{{ formatDate(dispute.resolvedAtUtc) }}</p>
    </div>

    <!-- admins izvēlas, kam taisnība -->
    <div v-else-if="!decision" class="mt-4 flex flex-wrap justify-end gap-2 border-t border-neutral-200 pt-4">
      <button type="button" class="btn flex-1 sm:flex-none" :disabled="isActing" @click="chooseDecision('CancelOrder')">
        {{ $t('admin.disputes.forBuyer') }}
      </button>
      <button type="button" class="btn flex-1 sm:flex-none" :disabled="isActing" @click="chooseDecision('CompleteOrder')">
        {{ $t('admin.disputes.forSeller') }}
      </button>
    </div>

    <form v-else class="mt-4 border-t border-neutral-200 pt-4" @submit.prevent="submit">
      <p class="text-sm text-neutral-600">
        {{ decision === 'CancelOrder' ? $t('admin.disputes.forBuyerHint') : $t('admin.disputes.forSellerHint') }}
      </p>

      <label :for="`dispute-reason-${dispute.disputeId}`" class="mt-3 block text-sm font-medium">
        {{ $t('admin.disputes.reasonLabel') }}
      </label>
      <textarea
        :id="`dispute-reason-${dispute.disputeId}`"
        v-model="reason"
        rows="3"
        :maxlength="reasonMaxLength"
        :placeholder="$t('admin.disputes.reasonPlaceholder')"
        class="field mt-2"
      />
      <div class="mt-1 flex justify-between text-xs">
        <span class="text-red-600">{{ reasonError }}</span>
        <span class="text-neutral-400">{{ reason.length }}/{{ reasonMaxLength }}</span>
      </div>

      <div class="mt-3 flex justify-end gap-2">
        <button type="button" class="btn" :disabled="isActing" @click="cancelDecision">
          {{ $t('common.cancel') }}
        </button>
        <button type="submit" class="btn-primary" :disabled="isActing">
          {{ $t('admin.disputes.confirm') }}
        </button>
      </div>
    </form>
  </article>
</template>
