<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

type LotMedia = {
  key: string
  type: string
  url: string
}

const props = withDefaults(
  defineProps<{
    brand: string
    title: string
    seller?: string
    price: number
    currency?: string
    media?: LotMedia[]
    timeLeft: string
  }>(),
  {
    seller: '',
    currency: 'EUR',
    media: () => [],
  },
)
const { locale } = useI18n()

/*
 * Берём только фотографии с нормальным URL.
 * DaisyUI hover-gallery поддерживает максимум 10 изображений.
 */
const images = computed(() => {
  return props.media
    .filter((item) => item.type === 'photo' && item.url)
    .slice(0, 10)
})

const formattedPrice = computed(() => {
  return new Intl.NumberFormat(locale.value, {
    minimumFractionDigits: 0,
    maximumFractionDigits: 2,
  }).format(props.price)
})
</script>

<template>
  <article class="group flex h-full w-full flex-col bg-background">
    <!-- Hover gallery -->
    <figure
      v-if="images.length"
      class="hover-gallery relative aspect-[4/5] w-full overflow-hidden bg-foreground/5
             [&>img]:h-full [&>img]:w-full [&>img]:object-cover"
    >
      <img
        v-for="(image, index) in images"
        :key="image.key || image.url"
        :src="image.url"
        :alt="index === 0 ? title : $t('auction.imageAlt', { title, number: index + 1 })"
        :loading="index === 0 ? 'eager' : 'lazy'"
        draggable="false"
      />
    </figure>

    <!-- No image -->
    <div
      v-else
      class="flex aspect-[4/5] w-full items-center justify-center bg-foreground/5 text-sm text-foreground/40"
    >
      {{ $t('common.noImage') }}
    </div>

    <!-- Information -->
    <div class="flex flex-1 flex-col px-1 pb-2 pt-4">
      <p
        class="truncate text-[11px] font-medium uppercase tracking-[0.18em] text-foreground/55"
      >
        {{ brand }}
      </p>

      <h3
        class="mt-1.5 line-clamp-2 min-h-12 text-base font-medium leading-6 text-foreground"
      >
        {{ title }}
      </h3>

      <p
        v-if="seller"
        class="mt-1 truncate text-xs text-foreground/45"
      >
        {{ $t('common.seller') }}: {{ seller }}
      </p>

      <div class="mt-auto pt-4">
        <p class="text-xs text-foreground/50">
          {{ $t('common.currentBid') }}
        </p>

        <div class="mt-1 flex items-end justify-between gap-3">
          <div class="flex items-baseline gap-1.5">
            <span class="text-xl font-semibold tracking-tight text-foreground">
              {{ formattedPrice }}
            </span>

            <span class="text-xs font-medium text-foreground/55">
              {{ currency }}
            </span>
          </div>

          <span class="whitespace-nowrap text-xs text-foreground/55">
            {{ timeLeft }}
          </span>
        </div>
      </div>
    </div>
  </article>
</template>
