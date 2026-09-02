<script setup lang="ts">
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import VueEasyLightbox from 'vue-easy-lightbox'

interface LotImage {
  url: string
  key?: string | null
}

const props = defineProps<{
  images: LotImage[]
  title: string
}>()

const { t } = useI18n()
const selectedImageIndex = ref(0)
const isLightboxOpen = ref(false)

const selectedImage = computed(
  () => props.images[selectedImageIndex.value]?.url ?? '',
)

const lightboxImages = computed(() =>
  props.images.map((image, index) => ({
    src: image.url,
    title: t('lot.imageCount', {
      title: props.title,
      number: index + 1,
      total: props.images.length,
    }),
  })),
)

const selectImage = (index: number) => {
  selectedImageIndex.value = index
}

const openLightbox = (index = selectedImageIndex.value) => {
  selectedImageIndex.value = index
  isLightboxOpen.value = true
}

const closeLightbox = () => {
  isLightboxOpen.value = false
}
</script>

<template>
  <section class="min-w-0 space-y-4">
    <button
      type="button"
      class="group relative block h-[420px] w-full overflow-hidden rounded-3xl border border-border bg-background shadow-sm sm:h-[520px]"
      :disabled="!selectedImage"
      @click="openLightbox()"
    >
      <img
        v-if="selectedImage"
        :src="selectedImage"
        :alt="title"
        draggable="false"
        class="h-full w-full cursor-zoom-in select-none object-contain transition duration-300 group-hover:scale-[1.01]"
      />

      <div
        v-else
        class="flex h-full items-center justify-center text-sm text-muted-foreground"
      >
        {{ $t('lot.noImageAvailable') }}
      </div>

      <div
        v-if="selectedImage"
        class="pointer-events-none absolute bottom-4 right-4 rounded-full bg-black/65 px-4 py-2 text-xs font-medium text-white backdrop-blur"
      >
        {{ $t('lot.openGallery') }}
      </div>
    </button>

    <div
      v-if="images.length > 1"
      class="flex gap-3 overflow-x-auto pb-2"
    >
      <button
        v-for="(image, index) in images"
        :key="image.key ?? index"
        type="button"
        class="h-24 w-24 shrink-0 overflow-hidden rounded-2xl border bg-background transition"
        :class="
          selectedImageIndex === index
            ? 'border-primary ring-2 ring-primary/30'
            : 'border-border opacity-70 hover:opacity-100'
        "
        :aria-label="$t('lot.selectImage', { number: index + 1 })"
        @click="selectImage(index)"
        @dblclick="openLightbox(index)"
      >
        <img
          :src="image.url"
          :alt="`${title} ${index + 1}`"
          draggable="false"
          class="h-full w-full select-none object-cover"
        />
      </button>
    </div>

    <VueEasyLightbox
      :visible="isLightboxOpen"
      :imgs="lightboxImages"
      :index="selectedImageIndex"
      :esc-disabled="false"
      :move-disabled="false"
      :rotate-disabled="false"
      @hide="closeLightbox"
    />
  </section>
</template>

<style>
.vel-modal {
  z-index: 200 !important;
}
</style>
