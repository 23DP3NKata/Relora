<script setup lang="ts">
import { categories } from '@/constants/categories'
import { useLocalePath } from '@/composables/useLocalePath'

const localePath = useLocalePath()

defineEmits<{
  close: []
}>()
</script>

<template>
  <div class="pb-6">
    <p class="mb-3 text-xs uppercase tracking-[0.24em] text-muted-foreground">
      {{ $t('notFound.categories') }}
    </p>

    <div class="space-y-2">
      <RouterLink
        v-for="item in categories"
        :key="item.labelKey"
        :to="localePath(item.to)"
        class="flex h-14 items-center justify-between rounded-2xl border border-border bg-muted/40 px-4 text-sm font-medium text-foreground transition hover:bg-accent hover:text-accent-foreground"
        @click="$emit('close')"
      >
        <span>{{ $t(item.labelKey) }}</span>
        <span
          class="text-muted-foreground"
          aria-hidden="true"
        >&rarr;</span>
      </RouterLink>
    </div>
  </div>
</template>
