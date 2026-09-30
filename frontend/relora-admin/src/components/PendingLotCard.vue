<script setup lang="ts">
import { useI18n } from 'vue-i18n'

import type { LotPreview } from '@/types/lot'

defineProps<{
  lot: LotPreview
  isActing: boolean
}>()

const { locale, t } = useI18n()

const emit = defineEmits<{
  approve: [lotId: string]
  reject: [lotId: string]
}>()

const mediaBaseUrl = import.meta.env.VITE_MEDIA_PUBLIC_BASE_URL ?? ''

// api atdod tikai bildes key R2
const imageUrl = (lot: LotPreview) => (lot.mainPhotoKey ? `${mediaBaseUrl}/${lot.mainPhotoKey}` : '')

const createdDate = (lot: LotPreview) => {
  if (!lot.createdAt) return t('common.unavailable')
  return new Date(lot.createdAt).toLocaleString(locale.value)
}

const priceLabel = (lot: LotPreview) => `${lot.currency} ${lot.priceAmount}`
</script>

<template>
  <article class="rounded-xl border bg-white p-4">
    <div class="grid gap-4 sm:grid-cols-[120px_1fr_auto] sm:items-center">
      <img :src="imageUrl(lot)" :alt="lot.title || $t('admin.pending.lotImage')" class="h-24 w-24 rounded-lg border object-cover" />

      <div>
        <div class="flex items-center gap-2">
          <h2 class="text-lg font-semibold">{{ lot.title || $t('admin.pending.untitledLot') }}</h2>
          <span class="rounded-full border border-amber-300 bg-amber-100 px-2 py-0.5 text-xs text-amber-800">
            {{ $t('admin.pending.statusPending') }}
          </span>
        </div>
        <p class="text-sm text-slate-600">{{ lot.brand || $t('admin.pending.unknownBrand') }}</p>
        <p class="text-sm text-slate-700">{{ priceLabel(lot) }}</p>
        <p class="text-xs text-slate-500">{{ $t('admin.pending.seller') }}: {{ lot.sellerId }}</p>
        <p class="text-xs text-slate-500">{{ $t('admin.pending.created') }}: {{ createdDate(lot) }}</p>
      </div>

      <div class="flex gap-2">
        <button
          type="button"
          class="rounded-md bg-emerald-600 px-3 py-2 text-sm font-medium text-white disabled:cursor-not-allowed disabled:opacity-50"
          :disabled="isActing"
          @click="emit('approve', lot.id)"
        >
          {{ $t('admin.pending.approve') }}
        </button>

        <button
          type="button"
          class="rounded-md bg-red-600 px-3 py-2 text-sm font-medium text-white disabled:cursor-not-allowed disabled:opacity-50"
          :disabled="isActing"
          @click="emit('reject', lot.id)"
        >
          {{ $t('admin.pending.reject') }}
        </button>
      </div>
    </div>
  </article>
</template>
