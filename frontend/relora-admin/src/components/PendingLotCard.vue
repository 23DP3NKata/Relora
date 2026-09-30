<script setup lang="ts">
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'

import type { LotPreview } from '@/types/lot'

const props = defineProps<{
  lot: LotPreview
  isActing: boolean
}>()

const { locale, t } = useI18n()

const emit = defineEmits<{
  approve: [lotId: string]
  reject: [lotId: string, reason: string]
}>()

// backend pieņem līdz 2000 simboliem
const reasonMaxLength = 2000

const isRejectOpen = ref(false)
const reason = ref('')
const reasonError = ref('')

const mediaBaseUrl = import.meta.env.VITE_MEDIA_PUBLIC_BASE_URL ?? ''

// api atdod tikai bildes key R2
const imageUrl = computed(() => (props.lot.mainPhotoKey ? `${mediaBaseUrl}/${props.lot.mainPhotoKey}` : ''))

const createdDate = computed(() => {
  if (!props.lot.createdAt) return t('common.unavailable')
  return new Date(props.lot.createdAt).toLocaleString(locale.value, { dateStyle: 'medium', timeStyle: 'short' })
})

const priceLabel = computed(() =>
  new Intl.NumberFormat(locale.value, { style: 'currency', currency: props.lot.currency || 'EUR' }).format(props.lot.priceAmount),
)

const shortSellerId = computed(() => props.lot.sellerId.slice(0, 8))

const openReject = () => {
  isRejectOpen.value = true
  reasonError.value = ''
}

const cancelReject = () => {
  isRejectOpen.value = false
  reason.value = ''
  reasonError.value = ''
}

const submitReject = () => {
  const trimmed = reason.value.trim()

  if (!trimmed) {
    reasonError.value = t('admin.pending.rejectReasonRequired')
    return
  }

  reasonError.value = ''
  emit('reject', props.lot.id, trimmed)
}
</script>

<template>
  <article class="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
    <div class="flex flex-col gap-4 p-4 sm:flex-row sm:items-center">
      <div class="h-40 w-full shrink-0 overflow-hidden rounded-xl bg-slate-100 sm:h-28 sm:w-28">
        <img v-if="imageUrl" :src="imageUrl" :alt="lot.title || $t('admin.pending.lotImage')" class="h-full w-full object-cover" />
        <div v-else class="flex h-full w-full items-center justify-center text-xs text-slate-400">
          {{ $t('admin.pending.noImage') }}
        </div>
      </div>

      <div class="min-w-0 flex-1">
        <div class="flex flex-wrap items-center gap-2">
          <h3 class="truncate text-base font-semibold">{{ lot.title || $t('admin.pending.untitledLot') }}</h3>
          <span class="rounded-full bg-amber-100 px-2 py-0.5 text-xs font-medium text-amber-800">
            {{ $t('admin.pending.statusPending') }}
          </span>
        </div>

        <p class="mt-0.5 text-sm text-slate-500">{{ lot.brand || $t('admin.pending.unknownBrand') }}</p>
        <p class="mt-2 text-lg font-semibold">{{ priceLabel }}</p>

        <dl class="mt-2 flex flex-wrap gap-x-4 gap-y-1 text-xs text-slate-500">
          <div class="flex gap-1">
            <dt>{{ $t('admin.pending.seller') }}:</dt>
            <dd class="font-mono text-slate-700" :title="lot.sellerId">{{ shortSellerId }}</dd>
          </div>
          <div class="flex gap-1">
            <dt>{{ $t('admin.pending.created') }}:</dt>
            <dd class="text-slate-700">{{ createdDate }}</dd>
          </div>
        </dl>
      </div>

      <div v-if="!isRejectOpen" class="flex gap-2 sm:flex-col">
        <button
          type="button"
          class="flex-1 rounded-lg bg-emerald-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:opacity-50"
          :disabled="isActing"
          @click="emit('approve', lot.id)"
        >
          {{ $t('admin.pending.approve') }}
        </button>

        <button
          type="button"
          class="flex-1 rounded-lg border border-red-200 bg-white px-4 py-2 text-sm font-medium text-red-600 transition hover:bg-red-50 disabled:cursor-not-allowed disabled:opacity-50"
          :disabled="isActing"
          @click="openReject"
        >
          {{ $t('admin.pending.reject') }}
        </button>
      </div>
    </div>

    <form v-if="isRejectOpen" class="border-t border-slate-100 bg-slate-50 p-4" @submit.prevent="submitReject">
      <label :for="`reject-reason-${lot.id}`" class="text-sm font-medium text-slate-700">
        {{ $t('admin.pending.rejectReasonLabel') }}
      </label>
      <textarea
        :id="`reject-reason-${lot.id}`"
        v-model="reason"
        rows="3"
        :maxlength="reasonMaxLength"
        :placeholder="$t('admin.pending.rejectReasonPlaceholder')"
        class="mt-2 w-full rounded-lg border bg-white px-3 py-2 text-sm focus:outline-none focus:ring-2"
        :class="reasonError ? 'border-red-300 focus:ring-red-200' : 'border-slate-200 focus:ring-slate-200'"
      />

      <div class="mt-1 flex justify-between text-xs">
        <span class="text-red-600">{{ reasonError }}</span>
        <span class="text-slate-400">{{ reason.length }}/{{ reasonMaxLength }}</span>
      </div>

      <div class="mt-3 flex justify-end gap-2">
        <button
          type="button"
          class="rounded-lg border border-slate-200 bg-white px-4 py-2 text-sm font-medium text-slate-700 transition hover:bg-slate-100 disabled:opacity-50"
          :disabled="isActing"
          @click="cancelReject"
        >
          {{ $t('common.cancel') }}
        </button>
        <button
          type="submit"
          class="rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-red-700 disabled:cursor-not-allowed disabled:opacity-50"
          :disabled="isActing"
        >
          {{ $t('admin.pending.confirmReject') }}
        </button>
      </div>
    </form>
  </article>
</template>
