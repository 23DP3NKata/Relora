<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'

import { adminService } from '@/services/adminService'
import { adminLotService } from '@/services/lotService'
import type { PendingLotDetails } from '@/types/lot'

const route = useRoute()
const router = useRouter()
const { locale, t } = useI18n()

const lotId = route.params.id as string

const categoryKeys = ['Tops', 'Bottoms', 'Outerwear', 'Shoes', 'Accessories', 'Vintage', 'Bags', 'Jewellery', 'Other']
const conditionKeys = ['New', 'Worn', 'Refurbished']

const reasonMaxLength = 2000

const mediaBaseUrl = import.meta.env.VITE_MEDIA_PUBLIC_BASE_URL ?? ''

const isLoading = ref(true)
const isActing = ref(false)
const errorMessage = ref('')
const lot = ref<PendingLotDetails | null>(null)
const selectedPhoto = ref('')

const isRejectOpen = ref(false)
const reason = ref('')
const reasonError = ref('')

const photoUrl = (key: string) => `${mediaBaseUrl}/${key}`

const priceLabel = computed(() => {
  if (!lot.value) return ''
  return new Intl.NumberFormat(locale.value, { style: 'currency', currency: lot.value.currency || 'EUR' }).format(lot.value.startingPrice)
})

const createdDate = computed(() => {
  if (!lot.value?.createdAt) return t('common.unavailable')
  return new Date(lot.value.createdAt).toLocaleString(locale.value, { dateStyle: 'medium', timeStyle: 'short' })
})

const categoryLabel = computed(() => {
  const key = lot.value?.category != null ? categoryKeys[lot.value.category] : undefined
  return key ? t(`admin.lotDetails.categories.${key}`) : t('common.unavailable')
})

const conditionLabel = computed(() => {
  const key = lot.value?.condition != null ? conditionKeys[lot.value.condition] : undefined
  return key ? t(`admin.lotDetails.conditions.${key}`) : t('common.unavailable')
})

const loadLot = async () => {
  try {
    isLoading.value = true
    errorMessage.value = ''
    lot.value = await adminLotService.getPendingLotDetails(lotId)
    selectedPhoto.value = lot.value.photoKeys[0] ?? lot.value.mainImageUrl ?? ''
  } catch (error) {
    console.error(error)
    errorMessage.value = t('admin.lotDetails.loadFailed')
  } finally {
    isLoading.value = false
  }
}

const approve = async () => {
  try {
    isActing.value = true
    errorMessage.value = ''
    await adminService.acceptLot(lotId)
    router.push('/admin/pending')
  } catch (error) {
    console.error(error)
    errorMessage.value = t('admin.pending.approveFailed')
  } finally {
    isActing.value = false
  }
}

const reject = async () => {
  const trimmed = reason.value.trim()

  if (!trimmed) {
    reasonError.value = t('admin.pending.rejectReasonRequired')
    return
  }

  try {
    isActing.value = true
    errorMessage.value = ''
    reasonError.value = ''
    await adminService.rejectLot(lotId, trimmed)
    router.push('/admin/pending')
  } catch (error) {
    console.error(error)
    errorMessage.value = t('admin.pending.rejectFailed')
  } finally {
    isActing.value = false
  }
}

const cancelReject = () => {
  isRejectOpen.value = false
  reason.value = ''
  reasonError.value = ''
}

onMounted(loadLot)
</script>

<template>
  <section class="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
    <RouterLink to="/admin/pending" class="text-sm text-neutral-500 underline-offset-4 hover:text-neutral-950 hover:underline">
      ← {{ $t('admin.lotDetails.back') }}
    </RouterLink>

    <p v-if="errorMessage" class="card mt-6 px-4 py-3 text-sm text-red-600">{{ errorMessage }}</p>

    <div v-if="isLoading" class="card mt-6 h-96 animate-pulse bg-neutral-50" />

    <div v-else-if="lot" class="mt-6 grid gap-8 lg:grid-cols-2">
      <div>
        <div class="aspect-[4/5] overflow-hidden rounded-2xl border border-neutral-200 bg-neutral-50">
          <img
            v-if="selectedPhoto"
            :src="photoUrl(selectedPhoto)"
            :alt="lot.title || $t('admin.pending.lotImage')"
            class="h-full w-full object-contain"
          />
          <div v-else class="flex h-full w-full items-center justify-center text-sm text-neutral-400">
            {{ $t('admin.pending.noImage') }}
          </div>
        </div>

        <div v-if="lot.photoKeys.length > 1" class="mt-3 flex flex-wrap gap-2">
          <button
            v-for="key in lot.photoKeys"
            :key="key"
            type="button"
            class="aspect-[4/5] w-16 overflow-hidden rounded-xl border bg-neutral-50"
            :class="key === selectedPhoto ? 'border-neutral-950' : 'border-neutral-200'"
            @click="selectedPhoto = key"
          >
            <img :src="photoUrl(key)" :alt="$t('admin.pending.lotImage')" class="h-full w-full object-cover" />
          </button>
        </div>
      </div>

      <div>
        <div class="flex items-start justify-between gap-3">
          <p class="eyebrow">{{ lot.brand || $t('admin.pending.unknownBrand') }}</p>
          <span class="tag">{{ $t('admin.pending.statusPending') }}</span>
        </div>
        <h1 class="mt-2 text-3xl font-semibold tracking-tight">{{ lot.title || $t('admin.pending.untitledLot') }}</h1>
        <p class="mt-3 text-2xl font-semibold tabular-nums">{{ priceLabel }}</p>

        <dl class="mt-6 grid grid-cols-2 gap-x-6 gap-y-4 border-t border-neutral-200 pt-6 text-sm">
          <div>
            <dt class="eyebrow">{{ $t('admin.lotDetails.category') }}</dt>
            <dd class="mt-1">{{ categoryLabel }}</dd>
          </div>
          <div>
            <dt class="eyebrow">{{ $t('admin.lotDetails.condition') }}</dt>
            <dd class="mt-1">{{ conditionLabel }}</dd>
          </div>
          <div>
            <dt class="eyebrow">{{ $t('admin.lotDetails.color') }}</dt>
            <dd class="mt-1">{{ lot.color || $t('common.unavailable') }}</dd>
          </div>
          <div>
            <dt class="eyebrow">{{ $t('admin.lotDetails.country') }}</dt>
            <dd class="mt-1">{{ lot.country || $t('common.unavailable') }}</dd>
          </div>
          <div>
            <dt class="eyebrow">{{ $t('admin.pending.seller') }}</dt>
            <dd class="mt-1">{{ lot.sellerUsername || lot.sellerId.slice(0, 8) }}</dd>
            <dd v-if="lot.sellerEmail" class="text-xs text-neutral-400">{{ lot.sellerEmail }}</dd>
          </div>
          <div>
            <dt class="eyebrow">{{ $t('admin.pending.created') }}</dt>
            <dd class="mt-1">{{ createdDate }}</dd>
          </div>
        </dl>

        <div class="mt-6 border-t border-neutral-200 pt-6">
          <p class="eyebrow">{{ $t('admin.lotDetails.description') }}</p>
          <p class="mt-2 whitespace-pre-line text-sm leading-6 text-neutral-600">
            {{ lot.descriptionPreview || $t('admin.lotDetails.noDescription') }}
          </p>
        </div>

        <div v-if="!isRejectOpen" class="mt-8 flex gap-2">
          <button type="button" class="btn flex-1" :disabled="isActing" @click="isRejectOpen = true">
            {{ $t('admin.pending.reject') }}
          </button>
          <button type="button" class="btn-primary flex-1" :disabled="isActing" @click="approve">
            {{ $t('admin.pending.approve') }}
          </button>
        </div>

        <form v-else class="mt-8 border-t border-neutral-200 pt-6" @submit.prevent="reject">
          <label for="reject-reason" class="text-sm font-medium">
            {{ $t('admin.pending.rejectReasonLabel') }}
          </label>
          <textarea
            id="reject-reason"
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
      </div>
    </div>
  </section>
</template>
