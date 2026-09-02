<script setup lang="ts">
import { computed } from "vue"

const props = defineProps<{
  modelValue: string
  max: number
}>()

const left = computed(() => props.max - props.modelValue.length)
</script>

<template>
  <span
    class="text-xs"
    :class="'text-foreground/50'"
  >
    {{ $t('sell.charactersLeft', { count: left }) }}
  </span>
</template>
