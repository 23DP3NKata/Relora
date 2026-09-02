<script setup lang="ts">
defineProps<{ isAuthenticated: boolean }>()

const emit = defineEmits<{
  start: []
  manage: []
}>()

const steps = [
  { number: '01', titleKey: 'sell.landing.steps.create.title', descriptionKey: 'sell.landing.steps.create.description' },
  { number: '02', titleKey: 'sell.landing.steps.review.title', descriptionKey: 'sell.landing.steps.review.description' },
  { number: '03', titleKey: 'sell.landing.steps.live.title', descriptionKey: 'sell.landing.steps.live.description' },
]
</script>

<template>
  <div class="space-y-10 pb-6 sm:space-y-8 sm:pb-10">
    <section
      class="relative overflow-hidden rounded-[30px] border border-border bg-foreground/[0.025] px-6 py-8 sm:px-10 sm:py-12 lg:px-14 lg:py-14"
      aria-labelledby="sell-hero-title"
    >
      <div class="grid gap-8 lg:grid-cols-[minmax(0,1.35fr)_minmax(280px,0.65fr)] lg:gap-16">
        <div class="flex min-h-[360px] flex-col justify-between sm:min-h-[390px]">
          <p class="text-[10px] font-medium uppercase tracking-[0.28em] text-foreground/45">
            {{ $t('sell.landing.eyebrow') }}
          </p>

          <div class="max-w-3xl py-8 lg:py-12">
            <h1
              id="sell-hero-title"
              class="text-4xl font-semibold leading-[1.03] tracking-[-0.045em] sm:text-5xl lg:text-6xl"
            >
              {{ $t('sell.landing.title') }}
            </h1>

            <p class="mt-6 max-w-xl text-sm leading-7 text-foreground/62 sm:text-base">
              {{ $t('sell.landing.description') }}
            </p>

            <div class="mt-8 flex flex-wrap items-center gap-3">
              <button
                type="button"
                class="group inline-flex min-h-12 items-center justify-center gap-3 rounded-full bg-foreground px-6 text-sm font-medium text-background transition hover:opacity-80 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
                @click="emit('start')"
              >
                {{ $t('sell.landing.start') }}
                <span aria-hidden="true" class="transition-transform group-hover:translate-x-0.5 motion-reduce:transform-none">→</span>
              </button>

              <button
                v-if="isAuthenticated"
                type="button"
                class="inline-flex min-h-12 items-center justify-center rounded-full border border-border bg-background px-6 text-sm font-medium transition hover:bg-foreground hover:text-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
                @click="emit('manage')"
              >
                {{ $t('sell.landing.manage') }}
              </button>
            </div>

            <p class="mt-4 text-xs leading-5 text-foreground/45">
              {{ $t('sell.landing.note') }}
            </p>
          </div>
        </div>

        <aside class="hidden border-l border-border pl-10 pt-1 lg:block" aria-labelledby="selling-process-title">
          <p class="text-[10px] font-medium uppercase tracking-[0.25em] text-foreground/45">
            {{ $t('sell.landing.processEyebrow') }}
          </p>
          <h2 id="selling-process-title" class="mt-3 text-xl font-semibold tracking-tight">
            {{ $t('sell.landing.processTitle') }}
          </h2>

          <ol class="mt-7 divide-y divide-border">
            <li v-for="step in steps" :key="step.number" class="grid grid-cols-[32px_1fr] gap-4 py-5 first:pt-0">
              <span class="pt-0.5 text-xs tabular-nums text-foreground/35">{{ step.number }}</span>
              <div>
                <h3 class="text-sm font-medium">{{ $t(step.titleKey) }}</h3>
                <p class="mt-1.5 text-sm leading-6 text-foreground/55">
                  {{ $t(step.descriptionKey) }}
                </p>
              </div>
            </li>
          </ol>
        </aside>
      </div>
    </section>

    <section class="border-y border-border py-2 lg:hidden" aria-labelledby="selling-process-mobile-title">
      <p class="text-[10px] font-medium uppercase tracking-[0.25em] text-foreground/45">
        {{ $t('sell.landing.processEyebrow') }}
      </p>
      <h2 id="selling-process-mobile-title" class="mt-3 text-2xl font-semibold tracking-tight">
        {{ $t('sell.landing.processTitle') }}
      </h2>

      <ol class="mt-6 divide-y divide-border">
        <li v-for="step in steps" :key="`mobile-${step.number}`" class="grid grid-cols-[32px_1fr] gap-4 py-4 first:pt-0">
          <span class="pt-0.5 text-xs tabular-nums text-foreground/35">{{ step.number }}</span>
          <div>
            <h3 class="text-sm font-medium">{{ $t(step.titleKey) }}</h3>
            <p class="mt-1 text-sm leading-6 text-foreground/55">{{ $t(step.descriptionKey) }}</p>
          </div>
        </li>
      </ol>
    </section>
  </div>
</template>
