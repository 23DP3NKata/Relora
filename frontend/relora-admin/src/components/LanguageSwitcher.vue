<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

import { setLocale, supportedLocales, type AppLocale } from '@/translate'

const { locale, t } = useI18n()

const options = computed(() =>
  supportedLocales.map((code) => ({
    code,
    label: code.toUpperCase(),
  })),
)

const onChange = (event: Event) => {
  const value = (event.target as HTMLSelectElement).value as AppLocale
  setLocale(value)
}
</script>

<template>
  <label class="flex items-center gap-2 text-sm text-slate-500">
    <span class="sr-only sm:not-sr-only">{{ t('common.language') }}</span>
    <select
      class="rounded-lg border border-slate-200 bg-white px-2 py-1.5 text-slate-900 focus:border-slate-400 focus:outline-none"
      :value="locale"
      @change="onChange"
    >
      <option
        v-for="option in options"
        :key="option.code"
        :value="option.code"
      >
        {{ option.label }}
      </option>
    </select>
  </label>
</template>
