import { createI18n } from 'vue-i18n'

import de from './locales/de.json'
import en from './locales/en.json'
import fr from './locales/fr.json'

export const supportedLocales = ['en', 'de', 'fr'] as const
export type AppLocale = (typeof supportedLocales)[number]

const localeStorageKey = 'relora-admin-locale'

export const isSupportedLocale = (locale: string): locale is AppLocale =>
  supportedLocales.includes(locale as AppLocale)

const browserLocale = (): AppLocale => {
  const locale = navigator.language.split('-')[0]
  return isSupportedLocale(locale) ? locale : 'en'
}

const storedLocale = (): AppLocale | null => {
  const locale = localStorage.getItem(localeStorageKey)
  return locale && isSupportedLocale(locale) ? locale : null
}

export const i18n = createI18n({
  legacy: false,
  locale: storedLocale() ?? browserLocale(),
  fallbackLocale: 'en',
  messages: {
    en,
    de,
    fr,
  },
})

document.documentElement.lang = i18n.global.locale.value

export const setLocale = (locale: AppLocale) => {
  i18n.global.locale.value = locale
  localStorage.setItem(localeStorageKey, locale)
  document.documentElement.lang = locale
}
