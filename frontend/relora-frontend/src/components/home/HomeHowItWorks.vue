<script setup lang="ts">
import { RouterLink } from 'vue-router'

import HomeSectionHeader from '@/components/home/HomeSectionHeader.vue'
import { useLocalePath } from '@/composables/useLocalePath'

const localePath = useLocalePath()

const steps = ['discover', 'bid', 'win']
</script>

<template>
  <section aria-labelledby="how-title">
    <HomeSectionHeader
      number="07"
      :eyebrow="$t('home.howItWorks.label')"
      :title="$t('home.howItWorks.title')"
      title-id="how-title"
    >
      <!-- entry point for sellers -->
      <RouterLink
        :to="localePath('/sell')"
        class="font-mono text-[11px] uppercase tracking-[0.2em] text-foreground/60 underline-offset-[6px] transition hover:text-foreground hover:underline"
      >
        {{ $t('home.sellAnItem') }} →
      </RouterLink>
    </HomeSectionHeader>

    <ol class="mt-10 grid gap-10 md:grid-cols-3 md:gap-8 lg:mt-14">
      <li
        v-for="step in steps"
        :key="step"
        class="border-t border-foreground/10 pt-4"
      >
        <p class="text-[clamp(3.5rem,7vw,6rem)] font-semibold leading-none tracking-[-0.06em] text-foreground/15">
          {{ $t(`home.howItWorks.steps.${step}.number`) }}
        </p>
        <h3 class="mt-6 text-2xl font-medium tracking-[-0.03em] text-foreground">
          {{ $t(`home.howItWorks.steps.${step}.title`) }}
        </h3>
        <p class="mt-2 max-w-xs text-sm leading-6 text-foreground/65">
          {{ $t(`home.howItWorks.steps.${step}.description`) }}
        </p>
      </li>
    </ol>
  </section>
</template>
