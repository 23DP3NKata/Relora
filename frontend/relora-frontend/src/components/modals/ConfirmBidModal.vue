<script setup lang="ts">
defineProps<{
  lotTitle: string
  amountLabel: string
  isLoading?: boolean
}>()

const emit = defineEmits<{
  (e: 'cancel'): void
  (e: 'confirm'): void
}>()
</script>

<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center px-4">
    <div class="absolute inset-0 bg-black/30 backdrop-blur-sm" @click="emit('cancel')" />

    <div
      class="relative z-10 w-full max-w-md rounded-[28px] border bg-background p-6 shadow-xl"
      role="dialog"
      aria-modal="true"
      aria-labelledby="confirm-bid-title"
    >
      <p class="text-xs font-medium uppercase tracking-[0.14em] text-foreground/45">
        {{ lotTitle }}
      </p>
      <h2 id="confirm-bid-title" class="mt-2 text-xl font-semibold tracking-tight">
        {{ $t('lot.confirmBidTitle') }}
      </h2>

      <p class="mt-4 text-3xl font-semibold tabular-nums">{{ amountLabel }}</p>

      <p class="mt-4 text-sm leading-6 text-foreground/70">
        {{ $t('lot.confirmBidWarning') }}
      </p>

      <div class="mt-6 flex justify-end gap-3">
        <button
          type="button"
          class="rounded-full border px-5 py-2.5 text-sm font-medium transition hover:bg-accent disabled:cursor-not-allowed disabled:opacity-50"
          :disabled="isLoading"
          @click="emit('cancel')"
        >
          {{ $t('common.cancel') }}
        </button>

        <button
          type="button"
          class="rounded-full bg-foreground px-5 py-2.5 text-sm font-medium text-background transition hover:opacity-80 disabled:cursor-not-allowed disabled:opacity-50"
          :disabled="isLoading"
          @click="emit('confirm')"
        >
          {{ isLoading ? $t('lot.placingBid') : $t('lot.confirmBid') }}
        </button>
      </div>
    </div>
  </div>
</template>
