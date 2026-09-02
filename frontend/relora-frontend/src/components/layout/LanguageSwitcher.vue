<script setup lang="ts">
import { computed } from 'vue'
import {
  useRoute,
  useRouter,
} from 'vue-router'

import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectSeparator,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'

import {
  localizedFullPath,
  routeLocale,
  setLocale,
  supportedLocales,
  type AppLocale,
} from '@/translate'

import { useCurrentLocale } from '@/composables/useLocalePath'

type LanguageOption = {
  code: AppLocale
  label: string
  nativeLabel: string
}

const route = useRoute()
const router = useRouter()
const currentLocale = useCurrentLocale()

const fallbackLanguage: LanguageOption = {
  code: 'en',
  label: 'English',
  nativeLabel: 'English',
}

const languages: readonly LanguageOption[] = [
  fallbackLanguage,
  {
    code: 'de',
    label: 'German',
    nativeLabel: 'Deutsch',
  },
  {
    code: 'fr',
    label: 'French',
    nativeLabel: 'Français',
  },
]

const selectedLocale = computed<AppLocale>({
  get: () => currentLocale.value,

  set: (locale) => {
    const fromRoute = routeLocale(route.params.locale)

    setLocale(locale, true)

    if (fromRoute === locale) {
      return
    }

    void router.push(
      localizedFullPath(route.fullPath, locale),
    )
  },
})

const selectedLanguage = computed<LanguageOption>(() => {
  return (
    languages.find(
      (language) => language.code === selectedLocale.value,
    ) ?? fallbackLanguage
  )
})
</script>

<template>
  <Select v-model="selectedLocale">
      <SelectTrigger
        class="
          h-10 w-auto min-w-[92px] rounded-full
          border-border bg-background px-3
          shadow-none transition
          hover:border-foreground/30 hover:bg-accent
          focus:ring-0 focus:ring-offset-0
          sm:min-w-[150px]
        "
        :aria-label="$t('common.language')"
      >
        <div class="flex min-w-0 items-center gap-2">
          <span
            class="
              flex h-7 w-7 shrink-0 items-center justify-center
              rounded-full bg-muted text-muted-foreground
            "
            aria-hidden="true"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="1.8"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="h-4 w-4"
            >
              <circle cx="12" cy="12" r="10" />
              <path d="M2 12h20" />
              <path
                d="M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10
                  15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10Z"
              />
            </svg>
          </span>

          <span
            class="
              hidden min-w-0 truncate text-sm font-medium
              text-foreground sm:block
            "
          >
            {{ selectedLanguage.nativeLabel }}
          </span>

          <span
            class="
              text-xs font-semibold uppercase tracking-wide
              text-muted-foreground sm:hidden
            "
          >
            {{ selectedLanguage.code }}
          </span>
        </div>

        <SelectValue class="sr-only" />
  </SelectTrigger>

    <SelectContent
      position="popper"
      align="end"
      :side-offset="8"
      class="min-w-[210px] rounded-2xl border-border p-1.5 shadow-xl"
    >
      <SelectGroup>
        <SelectItem
          v-for="language in languages"
          :key="language.code"
          :value="language.code"
          :text-value="language.nativeLabel"
          class="
            cursor-pointer rounded-xl px-3 py-2.5
            focus:bg-accent
          "
        >
          <div class="flex w-full items-center gap-3">
            <div class="flex min-w-0 flex-1 flex-col">
              <span class="truncate text-sm font-medium text-foreground">
                {{ language.nativeLabel }}
              </span>
            </div>

            <span
              class="
                rounded-md bg-muted px-1.5 py-0.5
                text-[10px] font-semibold uppercase
                tracking-wider text-muted-foreground
              "
            >
              {{ language.code }}
            </span>
          </div>
        </SelectItem>
      </SelectGroup>
    </SelectContent>
  </Select>
</template>
