<script setup lang="ts">
</script>

<template>
  <div class="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
    <!-- Hero -->
    <section class="flex flex-col items-center justify-center py-20 text-center sm:py-28">
      <p class="text-xs uppercase tracking-[0.35em] text-black/45">
        {{ $t('landing.eyebrow') }}
      </p>

      <h1 class="mt-4 text-5xl font-extrabold tracking-tight sm:text-6xl md:text-7xl">
        Relora
      </h1>

      <p class="mt-6 max-w-2xl text-base leading-7 text-black/70 sm:text-lg">
        {{ $t('landing.heroDescription') }}
      </p>

      <p class="mt-3 text-sm text-black/50 sm:text-base">
        {{ $t('landing.heroTagline') }}
      </p>

      <div class="mt-8 flex flex-col gap-3 sm:flex-row">
        <a
          href="#early-access"
          class="inline-flex items-center justify-center rounded-full bg-black px-8 py-3 text-sm font-medium text-white transition hover:bg-black/90"
        >
          {{ $t('landing.getEarlyAccess') }}
        </a>

        <a
          href="#about-relora"
          class="inline-flex items-center justify-center rounded-full border border-black/15 px-8 py-3 text-sm font-medium text-black transition hover:border-black"
        >
          {{ $t('common.learnMore') }}
        </a>
      </div>
    </section>

    <!-- Brand statement -->
    <section
      class="my-16 flex flex-col items-center justify-center gap-2 overflow-hidden py-16 text-center"
      aria-labelledby="brand-story-title"
    >
      <h2
        id="brand-story-title"
        class="max-w-4xl text-3xl font-semibold leading-tight text-black sm:text-4xl md:text-5xl"
      >
        {{ $t('landing.brandTitle') }}
      </h2>

      <p class="mt-4 max-w-2xl text-sm leading-7 text-black/75 sm:text-base">
        {{ $t('landing.brandDescription') }}
      </p>
    </section>

    <!-- About / positioning -->
    <section
      id="about-relora"
      class="grid gap-10 border-y border-black/10 py-16 md:grid-cols-2 md:gap-16"
    >
      <div>
        <p class="text-xs uppercase tracking-[0.3em] text-black/45">
          {{ $t('navigation.about') }}
        </p>

        <h2 class="mt-4 text-2xl font-semibold tracking-tight text-black sm:text-3xl">
          {{ $t('landing.aboutTitle') }}
        </h2>
      </div>

      <div class="space-y-5 text-sm leading-7 text-black/75 sm:text-base">
        <p>
          {{ $t('landing.aboutFirst') }}
        </p>

        <p>
          {{ $t('landing.aboutSecond') }}
        </p>
      </div>
    </section>

    <!-- Values -->
    <section class="py-16">
      <div class="mb-10 text-center">
        <p class="text-xs uppercase tracking-[0.3em] text-black/45">
          {{ $t('landing.whyEyebrow') }}
        </p>

        <h2 class="mt-4 text-2xl font-semibold tracking-tight text-black sm:text-3xl">
          {{ $t('landing.whyTitle') }}
        </h2>
      </div>

      <div class="grid gap-6 md:grid-cols-3">
        <div class="rounded-2xl border border-black/10 p-6 transition hover:border-black/30">
          <h3 class="text-lg font-semibold text-black">{{ $t('landing.values.archive.title') }}</h3>
          <p class="mt-3 text-sm leading-7 text-black/70">
            {{ $t('landing.values.archive.description') }}
          </p>
        </div>

        <div class="rounded-2xl border border-black/10 p-6 transition hover:border-black/30">
          <h3 class="text-lg font-semibold text-black">{{ $t('landing.values.rare.title') }}</h3>
          <p class="mt-3 text-sm leading-7 text-black/70">
            {{ $t('landing.values.rare.description') }}
          </p>
        </div>

        <div class="rounded-2xl border border-black/10 p-6 transition hover:border-black/30">
          <h3 class="text-lg font-semibold text-black">{{ $t('landing.values.market.title') }}</h3>
          <p class="mt-3 text-sm leading-7 text-black/70">
            {{ $t('landing.values.market.description') }}
          </p>
        </div>
      </div>
    </section>

    <!-- Second hero -->
    <section class="my-8 py-16 text-center">
      <h2 class="mx-auto max-w-4xl text-3xl font-semibold leading-tight text-black sm:text-4xl md:text-5xl">
        {{ $t('landing.demandTitle') }}
      </h2>

      <p class="mx-auto mt-4 max-w-2xl text-sm leading-7 text-black/75 sm:text-base">
        {{ $t('landing.demandDescription') }}
      </p>
    </section>

    <!-- Early access -->
    <section
      id="early-access"
      class="my-16 rounded-3xl border border-black/10 bg-black px-6 py-12 text-center text-white sm:px-10 sm:py-16"
    >
      <p class="text-xs uppercase tracking-[0.3em] text-white/50">
        {{ $t('landing.earlyAccess') }}
      </p>

      <h2 class="mx-auto mt-4 max-w-3xl text-3xl font-semibold leading-tight sm:text-4xl">
        {{ $t('landing.earlyTitle') }}
      </h2>

      <p class="mx-auto mt-4 max-w-2xl text-sm leading-7 text-white/75 sm:text-base">
        {{ $t('landing.earlyDescription') }}
      </p>

      <form
        action="https://formspree.io/f/mkoqkokp"
        method="POST"
        class="mx-auto mt-8 max-w-2xl space-y-4"
      >
        <div class="flex flex-col gap-3 sm:flex-row">
          <input
            type="email"
            name="email"
            required
            :placeholder="$t('landing.emailPlaceholder')"
            class="w-full rounded-full border border-white/20 bg-white px-5 py-3 text-sm text-black outline-none placeholder:text-black/40"
          />

          <button
            type="submit"
            class="inline-flex items-center justify-center rounded-full bg-white px-6 py-3 text-sm font-medium text-black transition hover:bg-white/90"
          >
            {{ $t('landing.requestAccess') }}
          </button>
        </div>

        <p class="text-xs text-white/50">
          {{ $t('landing.consent') }}
        </p>
      </form>
    </section>

    <!-- Footer text / SEO -->
    <section class="mx-auto mt-20 max-w-3xl text-center">
      <p class="text-sm leading-7 text-black/60">
        {{ $t('landing.footerDescription') }}
      </p>
    </section>

    <!-- Footer -->
    <footer class="mt-16 border-t border-black/10 pt-8">
      <div class="flex flex-col items-center justify-between gap-4 text-sm text-black/55 sm:flex-row">
        <p>{{ $t('common.copyright', { year: 2026 }) }}.</p>
        <p>{{ $t('landing.location') }}</p>
      </div>
    </footer>
  </div>
</template>
