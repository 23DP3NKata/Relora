<script setup lang="ts">
import { RouterLink } from 'vue-router'
import { useLocalePath } from '@/composables/useLocalePath'

const localePath = useLocalePath()
</script>

<template>
  <div class="mx-auto flex min-h-[70vh] max-w-7xl items-center justify-center px-4 py-10 sm:px-6 lg:px-8">
    <section
      class="relative w-full max-w-4xl overflow-hidden rounded-[32px] border border-border bg-background px-6 py-10 text-center md:px-10 md:py-14"
    >
      <div class="relative z-10">
        <p class="text-[12px] uppercase tracking-[0.28em] text-muted-foreground">
          Relora
        </p>

        <h1 class="mt-4 text-6xl font-semibold tracking-tight text-foreground sm:text-7xl md:text-8xl">
          404
        </h1>

        <h2 class="mt-4 text-2xl font-semibold tracking-tight text-foreground sm:text-3xl md:text-4xl">
          {{ $t('notFound.title') }}
        </h2>

        <p class="mx-auto mt-4 max-w-2xl text-sm leading-6 text-muted-foreground sm:text-base">
          {{ $t('notFound.description') }}
        </p>

        <div class="mt-8 flex flex-col items-center justify-center gap-3 sm:flex-row">
          <RouterLink
            :to="localePath('/')"
            class="rounded-full bg-primary px-5 py-3 text-sm font-medium text-primary-foreground transition hover:opacity-90"
          >
            {{ $t('notFound.backHome') }}
          </RouterLink>

          <RouterLink
            :to="localePath('/auctions')"
            class="rounded-full border border-border bg-card px-5 py-3 text-sm font-medium text-card-foreground transition hover:bg-accent hover:text-accent-foreground"
          >
            {{ $t('notFound.browseAuctions') }}
          </RouterLink>
        </div>

        <div class="mt-10 grid gap-4 sm:grid-cols-3">
          <div class="rounded-[24px] border border-border bg-card p-5 text-left">
            <p class="text-[11px] uppercase tracking-[0.22em] text-muted-foreground">
              {{ $t('notFound.explore') }}
            </p>
            <h3 class="mt-3 text-lg font-semibold text-card-foreground">
              {{ $t('notFound.trendingLots') }}
            </h3>
            <p class="mt-2 text-sm leading-6 text-muted-foreground">
              {{ $t('notFound.trendingDescription') }}
            </p>
          </div>

          <div class="rounded-[24px] border border-border bg-card p-5 text-left">
            <p class="text-[11px] uppercase tracking-[0.22em] text-muted-foreground">
              {{ $t('notFound.discover') }}
            </p>
            <h3 class="mt-3 text-lg font-semibold text-card-foreground">
              {{ $t('notFound.newListings') }}
            </h3>
            <p class="mt-2 text-sm leading-6 text-muted-foreground">
              {{ $t('notFound.newListingsDescription') }}
            </p>
          </div>

          <div class="rounded-[24px] border border-border bg-card p-5 text-left">
            <p class="text-[11px] uppercase tracking-[0.22em] text-muted-foreground">
              {{ $t('notFound.refine') }}
            </p>
            <h3 class="mt-3 text-lg font-semibold text-card-foreground">
              {{ $t('notFound.categories') }}
            </h3>
            <p class="mt-2 text-sm leading-6 text-muted-foreground">
              {{ $t('notFound.categoriesDescription') }}
            </p>
          </div>
        </div>

        <div class="mt-10 border-t border-border pt-6">
          <p class="text-sm text-muted-foreground">
            {{ $t('notFound.tagline') }}
          </p>
        </div>
      </div>
    </section>
  </div>
</template>
