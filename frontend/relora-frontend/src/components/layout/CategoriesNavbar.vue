<script setup lang="ts">
import Container from '@/components/ui/Container.vue'
import { RouterLink } from 'vue-router'
import { categories } from '@/constants/categories'
import { useLocalePath } from '@/composables/useLocalePath'

const localePath = useLocalePath()
</script>

<template>
<nav class="border-y border-border/10 bg-background/95 backdrop-blur-md">
  <Container>
    <div class="overflow-x-auto scrollbar-none">
      <div class="flex justify-center min-w-max gap-12 py-3">
        <RouterLink
          v-for="item in categories"
          :key="item.labelKey"
          :to="localePath(item.to)"
          class="group relative whitespace-nowrap text-sm font-medium tracking-[0.01em] text-foreground transition-colors duration-300 hover:text-primary"
        >
          <span>{{ $t(item.labelKey) }}</span>

          <span
            class="absolute -bottom-1 left-0 h-px w-full origin-left scale-x-0 bg-primary transition-transform duration-300 ease-out group-hover:scale-x-100"
          />
        </RouterLink>
      </div>
    </div>
  </Container>
</nav>
</template>
