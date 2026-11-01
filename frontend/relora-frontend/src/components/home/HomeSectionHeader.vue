<script setup lang="ts">
// section header in hero style: "03 — eyebrow" + big title, extra content on the right
defineProps<{
  number: string
  eyebrow: string
  title: string
  titleId?: string
}>()
</script>

<template>
  <header class="grid gap-6 border-t border-foreground/10 pt-6 lg:grid-cols-12 lg:items-end lg:gap-10">
    <div class="lg:col-span-8">
      <p class="font-mono text-[10px] uppercase tracking-[0.24em] text-foreground/55 sm:text-[11px]">
        {{ number }} — {{ eyebrow }}
      </p>
      <h2
        :id="titleId"
        class="mt-4 text-[clamp(2.2rem,5.5vw,4.5rem)] font-semibold uppercase leading-[0.9] tracking-[-0.06em] text-foreground"
      >
        {{ title }}
      </h2>
    </div>

    <div v-if="$slots.default" class="flex flex-col items-start gap-5 lg:col-span-4 lg:items-end lg:pb-2">
      <slot />
    </div>
  </header>
</template>
