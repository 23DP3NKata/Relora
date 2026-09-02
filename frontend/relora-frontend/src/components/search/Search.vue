<script setup lang="ts">
import Input from '@/components/ui/input/Input.vue'

defineProps<{
  modelValue: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
  search: []
}>()

function handleKeyDown(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    emit('search')
  }
}
</script>

<template>
  <div class="w-full max-w-lg">
    <div class="relative">
      <Input
        :model-value="modelValue"
        @update:model-value="emit('update:modelValue', String($event))"
        @keydown="handleKeyDown"
        :placeholder="$t('search.placeholder')"
        class="h-10 rounded-full border border-input bg-muted pl-10 pr-20 text-sm text-foreground placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-0"
      />

      <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3.5">
        <svg class="h-4 w-4 text-muted-foreground" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
      </div>

      <button
        type="button"
        @click="emit('search')"
        class="absolute right-1 top-1/2 -translate-y-1/2 rounded-full bg-primary px-3 py-1.5 text-xs font-medium text-primary-foreground transition hover:opacity-90"
      >
        {{ $t('common.search') }}
      </button>
    </div>
  </div>
</template>
