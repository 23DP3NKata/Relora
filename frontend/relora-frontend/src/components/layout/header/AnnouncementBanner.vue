<script setup lang="ts">
import { computed, ref } from 'vue'
import { XMarkIcon } from '@heroicons/vue/20/solid'
import { useI18n } from 'vue-i18n'

const props = withDefaults(
  defineProps<{
    title: string
    description: string
    buttonText?: string
    buttonHref?: string
    buttonTo?: string
    dismissible?: boolean
  }>(),
  {
    buttonText: '',
    buttonHref: '',
    buttonTo: '',
    dismissible: true,
  },
)

const { t } = useI18n()
const emit = defineEmits<{
  (event: 'close'): void
}>()

const isVisible = ref(true)
const resolvedButtonText = computed(() => props.buttonText || t('common.learnMore'))

function closeBanner() {
  isVisible.value = false
  emit('close')
}
</script>

<template>
  <div
    v-if="isVisible"
    class="relative isolate flex min-h-8 items-center gap-x-2 overflow-hidden border-b border-border bg-background px-4 sm:before:flex-1"
  >
    <div
      class="flex flex-1 flex-wrap items-center justify-center gap-x-4 gap-y-2"
    >
      <p class="text-center text-sm leading-6 text-foreground">
        <strong class="font-semibold">
          {{ title }}
        </strong>

        <svg
          viewBox="0 0 2 2"
          class="mx-2 inline size-0.5 fill-current"
          aria-hidden="true"
        >
        </svg>

        {{ description }}
      </p>

      <RouterLink
        v-if="buttonTo"
        :to="buttonTo"
        class="flex-none rounded-full bg-primary px-3.5 py-1 text-sm font-semibold text-primary-foreground shadow-xs transition hover:opacity-90 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ring"
      >
        {{ resolvedButtonText }}
        <span aria-hidden="true">→</span>
      </RouterLink>

      <a
        v-else-if="buttonHref"
        :href="buttonHref"
        target="_blank"
        rel="noopener noreferrer"
        class="flex-none rounded-full bg-primary px-3.5 py-1 text-sm font-semibold text-primary-foreground shadow-xs transition hover:opacity-90 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ring"
      >
        {{ resolvedButtonText }}
        <span aria-hidden="true">→</span>
      </a>
    </div>

    <div
      v-if="dismissible"
      class="flex justify-end sm:flex-1"
    >
      <button
        type="button"
        class="-m-2.5 rounded-full p-2.5 text-muted-foreground transition hover:bg-accent hover:text-accent-foreground focus-visible:outline-2 focus-visible:outline-ring"
        :aria-label="$t('common.closeAnnouncement')"
        @click="closeBanner"
      >
        <XMarkIcon
          class="size-5"
          aria-hidden="true"
        />
      </button>
    </div>
  </div>
</template>
