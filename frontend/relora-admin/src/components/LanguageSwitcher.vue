<script setup lang="ts">
import { useI18n } from 'vue-i18n'

import { setLocale, supportedLocales, type AppLocale } from '@/translate'

const { locale, t } = useI18n()

const onChange = (event: Event) => {
  const value = (event.target as HTMLSelectElement).value as AppLocale
  setLocale(value)
}
</script>

<template>
  <select
    class="rounded-full border border-neutral-200 bg-white px-3 py-1.5 text-sm focus:border-neutral-400 focus:outline-none"
    :aria-label="t('common.language')"
    :value="locale"
    @change="onChange"
  >
    <option v-for="code in supportedLocales" :key="code" :value="code">
      {{ code.toUpperCase() }}
    </option>
  </select>
</template>
