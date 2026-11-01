<script setup lang="ts">
import HomeSectionHeader from '@/components/home/HomeSectionHeader.vue'

// reasons to use Relora, numbered like an editorial list
const reasons = ['curated', 'premium', 'trust', 'community']
</script>

<template>
  <section aria-labelledby="why-title">
    <HomeSectionHeader
      number="06"
      :eyebrow="$t('home.whyRelora')"
      :title="$t('home.whyShortTitle')"
      title-id="why-title"
    >
      <p class="max-w-sm text-sm leading-6 text-foreground/65 lg:text-right">
        {{ $t('home.whyDescription') }}
      </p>
    </HomeSectionHeader>

    <ol class="mt-10 grid gap-x-8 gap-y-10 sm:grid-cols-2 lg:mt-14 lg:grid-cols-4">
      <li
        v-for="(reason, index) in reasons"
        :key="reason"
        class="border-t border-foreground/10 pt-4"
      >
        <p class="font-mono text-[10px] tracking-[0.2em] text-foreground/45">
          0{{ index + 1 }}
        </p>
        <h3 class="mt-4 text-xl font-medium leading-tight tracking-[-0.02em] text-foreground">
          {{ $t(`home.${reason}Title`) }}
        </h3>
        <p class="mt-3 text-sm leading-6 text-foreground/65">
          {{ $t(`home.${reason}Description`) }}
        </p>
      </li>
    </ol>
  </section>
</template>
