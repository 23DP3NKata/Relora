<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

import type { SupportRequest, SupportRequestStatus } from '@/types/support'

const props = defineProps<{
  request: SupportRequest
  isActing: boolean
}>()

const emit = defineEmits<{
  changeStatus: [requestId: string, status: SupportRequestStatus]
}>()

const { locale } = useI18n()

const createdDate = computed(() =>
  new Date(props.request.createdAt).toLocaleString(locale.value, { dateStyle: 'medium', timeStyle: 'short' }),
)

// atver e-pastu ar jau ierakstītu tēmu
const mailtoLink = computed(() => `mailto:${props.request.email}?subject=${encodeURIComponent('Re: ' + props.request.subject)}`)
</script>

<template>
  <article class="card p-4 sm:p-5">
    <div class="flex items-start justify-between gap-3">
      <p class="eyebrow">{{ request.category }}</p>
      <span class="tag shrink-0">{{ $t(`admin.support.status.${request.statusName}`) }}</span>
    </div>

    <h3 class="mt-1 text-base font-semibold sm:text-lg">{{ request.subject }}</h3>

    <p class="mt-2 text-xs text-neutral-400">
      {{ request.email }}
      <template v-if="request.username">
        <span class="mx-1.5">·</span>
        {{ request.username }}
      </template>
      <template v-else>
        <span class="mx-1.5">·</span>
        {{ $t('admin.support.guest') }}
      </template>
      <span class="mx-1.5">·</span>
      {{ createdDate }}
    </p>

    <p class="mt-3 whitespace-pre-line text-sm leading-6 text-neutral-600">{{ request.message }}</p>

    <div class="mt-4 flex flex-wrap justify-end gap-2 border-t border-neutral-200 pt-4">
      <a :href="mailtoLink" class="btn">
        {{ $t('admin.support.reply') }}
      </a>

      <button
        v-if="request.statusName === 'New'"
        type="button"
        class="btn"
        :disabled="isActing"
        @click="emit('changeStatus', request.id, 'InProgress')"
      >
        {{ $t('admin.support.takeInProgress') }}
      </button>

      <button
        v-if="request.statusName !== 'Closed'"
        type="button"
        class="btn-primary"
        :disabled="isActing"
        @click="emit('changeStatus', request.id, 'Closed')"
      >
        {{ $t('admin.support.close') }}
      </button>

      <button
        v-else
        type="button"
        class="btn"
        :disabled="isActing"
        @click="emit('changeStatus', request.id, 'InProgress')"
      >
        {{ $t('admin.support.reopen') }}
      </button>
    </div>
  </article>
</template>
