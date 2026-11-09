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
  <article class="card p-4 sm:p-5">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:gap-5">
      <div class="aspect-[4/5] w-24 shrink-0 overflow-hidden rounded-xl border border-neutral-200 bg-neutral-50">
        <img v-if="imageUrl" :src="imageUrl" :alt="lot.title || $t('admin.pending.lotImage')" class="h-full w-full object-cover" />
        <div v-else class="flex h-full w-full items-center justify-center p-2 text-center text-xs text-neutral-400">
          {{ $t('admin.pending.noImage') }}
        </div>
      </div>

      <div class="min-w-0 flex-1">
        <p class="eyebrow truncate">{{ lot.brand || $t('admin.pending.unknownBrand') }}</p>
        <h3 class="mt-1 truncate text-base font-semibold sm:text-lg">{{ lot.title || $t('admin.pending.untitledLot') }}</h3>
        <p class="mt-1 text-lg font-semibold tabular-nums">{{ priceLabel }}</p>
        <p class="mt-2 text-xs text-neutral-400">
          {{ $t('admin.pending.seller') }} <span class="font-mono" :title="lot.sellerId">{{ shortSellerId }}</span>
          <span class="mx-1.5">·</span>
          {{ createdDate }}
        </p>
      </div>

      <div v-if="!isRejectOpen" class="flex flex-wrap gap-2">
        <button type="button" class="btn" :disabled="isActing" @click="openReject">
          {{ $t('admin.pending.reject') }}
        </button>
        <button type="button" class="btn-primary" :disabled="isActing" @click="emit('approve', lot.id)">
          {{ $t('admin.pending.approve') }}
        </button>
      </div>
    </div>

    <form v-if="isRejectOpen" class="mt-4 border-t border-neutral-200 pt-4" @submit.prevent="submitReject">
      <label :for="`reject-reason-${lot.id}`" class="text-sm font-medium">
        {{ $t('admin.pending.rejectReasonLabel') }}
      </label>
      <textarea
        :id="`reject-reason-${lot.id}`"
        v-model="reason"
        rows="3"
        :maxlength="reasonMaxLength"
        :placeholder="$t('admin.pending.rejectReasonPlaceholder')"
        class="field mt-2"
      />
      <div class="mt-1 flex justify-between text-xs">
        <span class="text-red-600">{{ reasonError }}</span>
        <span class="text-neutral-400">{{ reason.length }}/{{ reasonMaxLength }}</span>
      </div>
      <div class="mt-3 flex justify-end gap-2">
        <button type="button" class="btn" :disabled="isActing" @click="cancelReject">
          {{ $t('common.cancel') }}
        </button>
        <button type="submit" class="btn-primary" :disabled="isActing">
          {{ $t('admin.pending.confirmReject') }}
        </button>
      </div>
    </form>
  </article>
</template>
