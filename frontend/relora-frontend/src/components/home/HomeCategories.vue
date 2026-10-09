<script setup lang="ts">
import { RouterLink } from 'vue-router'

import HomeSectionHeader from '@/components/home/HomeSectionHeader.vue'
import { useLocalePath } from '@/composables/useLocalePath'

const localePath = useLocalePath()

const imageBase = 'https://pub-d44c1b06b612479f8e654eedc923ffa1.r2.dev/display'

// only categories that have their own catalog page
const categories = [
  { labelKey: 'navigation.vintage', to: '/vintage', imageUrl: `${imageBase}/Vintage_Category_Icon.png` },
  { labelKey: 'navigation.shoes', to: '/shoes', imageUrl: `${imageBase}/Shoes_Category_Icon.png` },
  { labelKey: 'navigation.accessories', to: '/accessories', imageUrl: `${imageBase}/Acessories_Categorie_Icon.png` },
  { labelKey: 'navigation.designers', to: '/designers', imageUrl: `${imageBase}/Designer_Category_Icon.png` },
]
</script>

<template>
  <section aria-labelledby="categories-title">
    <HomeSectionHeader
      number="04"
      :eyebrow="$t('home.categories')"
      :title="$t('home.shopByCategory')"
      title-id="categories-title"
    >
      <p class="max-w-sm text-sm leading-6 text-foreground/65 lg:text-right">
        {{ $t('home.shopByCategoryDescription') }}
      </p>
    </HomeSectionHeader>

    <div class="mt-10 grid grid-cols-2 gap-x-4 gap-y-8 lg:mt-14 lg:grid-cols-4 lg:gap-x-8">
      <RouterLink
        v-for="(category, index) in categories"
        :key="category.to"
        :to="localePath(category.to)"
        class="group block focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-[var(--auction-accent)]"
      >
        <div class="aspect-square overflow-hidden rounded-[6px] bg-white dark:bg-neutral-900">
          <img
            :src="category.imageUrl"
            :alt="$t(category.labelKey)"
            loading="lazy"
            decoding="async"
            class="h-full w-full object-cover transition duration-700 ease-[cubic-bezier(0.22,1,0.36,1)] group-hover:scale-[1.05]"
          />
        </div>

        <div class="mt-4 flex items-baseline justify-between gap-3 border-t border-foreground/10 pt-3">
          <span class="text-lg font-medium tracking-[-0.02em] text-foreground lg:text-xl">
            <span class="mr-2 font-mono text-[10px] tracking-[0.2em] text-foreground/45">0{{ index + 1 }}</span>
            {{ $t(category.labelKey) }}
          </span>
          <span class="text-foreground/55 transition-transform duration-500 group-hover:rotate-45" aria-hidden="true">↗</span>
        </div>
      </RouterLink>
    </div>
  </section>
</template>
