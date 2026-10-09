<script setup lang="ts">
import { RouterLink } from 'vue-router'

import HomeSectionHeader from '@/components/home/HomeSectionHeader.vue'
import { useLocalePath } from '@/composables/useLocalePath'

const localePath = useLocalePath()

// slug is built the same way as on the designers page
const brands = [
  { name: 'Prada', slug: 'prada', logo: '/brands/prada-logo.svg' },
  { name: 'Gucci', slug: 'gucci', logo: '/brands/gucci-logo.svg' },
  { name: 'Dolce & Gabbana', slug: 'dolce-and-gabbana', logo: '/brands/dolce-gabbana-logo.svg' },
  { name: 'Dior', slug: 'dior', logo: '/brands/dior-logo.svg' },
  { name: 'Chanel', slug: 'chanel', logo: '/brands/chanel-2-logo.svg' },
]
</script>

<template>
  <section aria-labelledby="brands-title">
    <HomeSectionHeader
      number="05"
      :eyebrow="$t('home.brands')"
      :title="$t('home.shopByBrand')"
      title-id="brands-title"
    >
      <RouterLink
        :to="localePath('/designers')"
        class="font-mono text-[11px] uppercase tracking-[0.2em] text-foreground/60 underline-offset-[6px] transition hover:text-foreground hover:underline"
      >
        {{ $t('home.browseBrands') }} →
      </RouterLink>
    </HomeSectionHeader>

    <!-- 1px gap on a grey background gives thin dividers between logos -->
    <div class="mt-10 grid grid-cols-2 gap-px border-y border-foreground/10 bg-foreground/10 sm:grid-cols-5 lg:mt-14">
      <RouterLink
        v-for="brand in brands"
        :key="brand.slug"
        :to="localePath(`/designers/${brand.slug}`)"
        class="group flex h-32 items-center justify-center bg-background px-6 last:col-span-2 sm:last:col-span-1 transition-colors duration-300 hover:bg-[#efefec] focus-visible:outline-2 focus-visible:-outline-offset-2 focus-visible:outline-[var(--auction-accent)] lg:h-40 dark:hover:bg-neutral-900"
      >
        <img
          :src="brand.logo"
          :alt="brand.name"
          loading="lazy"
          class="max-h-12 max-w-[130px] object-contain opacity-75 grayscale transition duration-300 group-hover:opacity-100 dark:invert"
        />
      </RouterLink>
    </div>
  </section>
</template>
