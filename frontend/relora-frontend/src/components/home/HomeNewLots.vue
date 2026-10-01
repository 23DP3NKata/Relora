<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { RouterLink } from 'vue-router'

import HomeLotCard from '@/components/home/HomeLotCard.vue'
import HomeSectionHeader from '@/components/home/HomeSectionHeader.vue'
import { useLocalePath } from '@/composables/useLocalePath'
import { getNewLots } from '@/app/services/homeService'
import type { LotPreview } from '@/types/lot'

type Preference = 'men' | 'women'

// newest lots for the chosen catalog (women / men)
const props = defineProps<{
  preference: Preference
}>()

const emit = defineEmits<{
  change: [value: Preference]
}>()

const localePath = useLocalePath()
const lots = ref<LotPreview[]>([])
const loaded = ref(false)

const options: Preference[] = ['women', 'men']

// catalog page for women or men
const catalogLink = computed(() => localePath(`/${props.preference}`))

async function loadLots() {
  loaded.value = false
  lots.value = await getNewLots(props.preference)
  loaded.value = true
}

onMounted(loadLots)
watch(() => props.preference, loadLots)
</script>

<template>
  <section aria-labelledby="new-lots-title">
    <HomeSectionHeader
      number="03"
      :eyebrow="$t('home.newLots.eyebrow')"
      :title="$t('home.newLots.title')"
      title-id="new-lots-title"
    >
      <div
        class="inline-flex rounded-full border border-foreground/15 p-1"
        role="group"
        :aria-label="$t('home.newLots.catalogLabel')"
      >
        <button
          v-for="option in options"
          :key="option"
          type="button"
          class="h-10 rounded-full px-5 font-mono text-[11px] uppercase tracking-[0.18em] transition"
          :class="preference === option
            ? 'bg-[#111111] text-white dark:bg-white dark:text-[#111111]'
            : 'text-foreground/60 hover:text-foreground'"
          :aria-pressed="preference === option"
          @click="emit('change', option)"
        >
          {{ $t(`navigation.${option}`) }}
        </button>
      </div>

      <RouterLink
        :to="catalogLink"
        class="font-mono text-[11px] uppercase tracking-[0.2em] text-foreground/60 underline-offset-[6px] transition hover:text-foreground hover:underline"
      >
        {{ $t('home.newLots.viewAll') }} →
      </RouterLink>
    </HomeSectionHeader>

    <!-- horizontal scroll on mobile, grid on desktop -->
    <div
      v-if="lots.length"
      class="-mx-4 mt-10 flex snap-x snap-mandatory gap-4 overflow-x-auto px-4 pb-2 lg:mx-0 lg:mt-14 lg:grid lg:grid-cols-4 lg:gap-x-8 lg:gap-y-12 lg:overflow-visible lg:px-0"
    >
      <div
        v-for="lot in lots"
        :key="lot.id"
        class="w-[68%] shrink-0 snap-start sm:w-[42%] lg:w-auto"
      >
        <HomeLotCard :lot="lot" />
      </div>
    </div>

    <div
      v-else-if="loaded"
      class="mt-10 flex flex-col items-start gap-4 border-b border-foreground/10 py-10 sm:flex-row sm:items-center sm:justify-between lg:mt-14"
    >
      <p class="text-lg font-medium tracking-[-0.02em] text-foreground">
        {{ $t('home.newLots.empty') }}
      </p>
      <RouterLink
        :to="localePath('/catalog')"
        class="font-mono text-[11px] uppercase tracking-[0.2em] text-foreground/60 underline-offset-[6px] transition hover:text-foreground hover:underline"
      >
        {{ $t('home.newLots.browseAll') }} →
      </RouterLink>
    </div>
  </section>
</template>
