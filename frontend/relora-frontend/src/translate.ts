import { createI18n } from 'vue-i18n'

import de from './locales/de.json'
import en from './locales/en.json'
import fr from './locales/fr.json'

export const supportedLocales = ['en', 'de', 'fr'] as const

export type AppLocale = (typeof supportedLocales)[number]

const localeStorageKey = 'relora.locale'

export function isSupportedLocale(value: unknown): value is AppLocale {
  return (
    typeof value === 'string' &&
    supportedLocales.includes(value as AppLocale)
  )
}

export function getStoredLocale(): AppLocale | null {
  if (typeof localStorage === 'undefined') {
    return null
  }

  const storedLocale = localStorage.getItem(localeStorageKey)

  return isSupportedLocale(storedLocale) ? storedLocale : null
}

export function getBrowserLocale(): AppLocale {
  if (typeof navigator === 'undefined') {
    return 'en'
  }

  const browserLocales = [
    navigator.language,
    ...Array.from(navigator.languages ?? []),
  ]

  for (const browserLocale of browserLocales) {
    const locale = browserLocale.split('-')[0]

    if (isSupportedLocale(locale)) {
      return locale
    }
  }

  return 'en'
}

export function getPreferredLocale(): AppLocale {
  return getStoredLocale() ?? getBrowserLocale()
}

export const i18n = createI18n({
  legacy: false,
  globalInjection: true,
  locale: getPreferredLocale(),
  fallbackLocale: 'en',
  messages: {
    en,
    de,
    fr,
  },
})

export function setLocale(locale: AppLocale, persist = false): void {
  i18n.global.locale.value = locale

  if (typeof document !== 'undefined') {
    document.documentElement.lang = locale
  }

  if (persist && typeof localStorage !== 'undefined') {
    localStorage.setItem(localeStorageKey, locale)
  }
}

export function routeLocale(value: unknown): AppLocale | null {
  if (Array.isArray(value)) {
    return routeLocale(value[0])
  }

  return isSupportedLocale(value) ? value : null
}

export function localizedFullPath(fullPath: string, locale: AppLocale): string {
  const url = new URL(fullPath, 'https://relora.eu')
  const segments = url.pathname.split('/').filter(Boolean)

  if (isSupportedLocale(segments[0])) {
    segments[0] = locale
  } else {
    segments.unshift(locale)
  }

  const path = `/${segments.join('/')}`

  return `${path}${url.search}${url.hash}`
}

export function unlocalizedPath(path: string): string {
  const url = new URL(path, 'https://relora.eu')
  const segments = url.pathname.split('/').filter(Boolean)

  if (isSupportedLocale(segments[0])) {
    segments.shift()
  }

  const nextPath = segments.length ? `/${segments.join('/')}` : '/'

  return `${nextPath}${url.search}${url.hash}`
}
