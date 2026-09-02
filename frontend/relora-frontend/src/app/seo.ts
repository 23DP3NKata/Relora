import type {
  Router,
  RouteLocationNormalizedLoaded,
} from 'vue-router'

import {
  i18n,
  localizedFullPath,
  routeLocale,
  setLocale,
  supportedLocales,
  type AppLocale,
} from '@/translate'

const siteUrl = import.meta.env.VITE_SITE_URL
const siteName = import.meta.env.VITE_SITE_NAME

function text(key: string): string {
  return i18n.global.t(key)
}

function ensureMeta(selector: string, attribute: 'name' | 'property', value: string): HTMLMetaElement {
  let element = document.head.querySelector<HTMLMetaElement>(selector)

  if (!element) {
    element = document.createElement('meta')
    element.setAttribute(attribute, value)
    document.head.appendChild(element)
  }

  return element
}

function ensureLink(rel: string, hreflang?: string): HTMLLinkElement {
  const selector = hreflang
    ? `link[rel="${rel}"][hreflang="${hreflang}"]`
    : `link[rel="${rel}"]`

  let element = document.head.querySelector<HTMLLinkElement>(selector)

  if (!element) {
    element = document.createElement('link')
    element.rel = rel

    if (hreflang) {
      element.hreflang = hreflang
    }

    document.head.appendChild(element)
  }

  return element
}

function routeSeoKey(route: RouteLocationNormalizedLoaded): string {
  const seoKey = route.meta.seoKey

  if (typeof seoKey === 'string') {
    return seoKey
  }

  if (
    route.name === 'designer-catalog' ||
    String(route.name ?? '').startsWith('catalog')
  ) {
    return 'catalog'
  }

  if (typeof route.name === 'string') {
    return route.name
  }

  const path = route.path
    .split('/')
    .filter(Boolean)
    .filter(segment => !supportedLocales.includes(segment as AppLocale))
    .join('.')

  return path || 'home'
}

function absoluteUrl(fullPath: string): string {
  const url = new URL(fullPath, siteUrl)
  url.search = ''
  url.hash = ''

  return url.toString()
}

function updateSeo(route: RouteLocationNormalizedLoaded): void {
  if (typeof document === 'undefined') {
    return
  }

  const locale = routeLocale(route.params.locale) ?? 'en'
  const seoKey = routeSeoKey(route)
  const titleKey = `seo.${seoKey}.title`
  const descriptionKey = `seo.${seoKey}.description`
  const title = i18n.global.te(titleKey)
    ? text(titleKey)
    : text('seo.default.title')
  const description = i18n.global.te(descriptionKey)
    ? text(descriptionKey)
    : text('seo.default.description')

  const canonical = absoluteUrl(
    localizedFullPath(route.fullPath, locale),
  )

  document.documentElement.lang = locale
  document.title = title

  ensureMeta('meta[name="description"]', 'name', 'description').content = description
  ensureMeta('meta[property="og:title"]', 'property', 'og:title').content = title
  ensureMeta('meta[property="og:description"]', 'property', 'og:description').content = description
  ensureMeta('meta[property="og:url"]', 'property', 'og:url').content = canonical
  ensureMeta('meta[name="twitter:title"]', 'name', 'twitter:title').content = title
  ensureMeta('meta[name="twitter:description"]', 'name', 'twitter:description').content = description

  ensureLink('canonical').href = canonical

  for (const alternateLocale of supportedLocales) {
    ensureLink('alternate', alternateLocale).href = absoluteUrl(
      localizedFullPath(route.fullPath, alternateLocale),
    )
  }

  ensureLink('alternate', 'x-default').href = absoluteUrl(
    localizedFullPath(route.fullPath, 'en'),
  )
}

export function formatSeoTitle(title: string): string {
  const trimmedTitle = title.trim()

  return trimmedTitle
    ? `${trimmedTitle} \u2014 ${siteName}`
    : text('seo.default.title')
}

export function setPageTitle(title: string): void {
  if (typeof document === 'undefined') {
    return
  }

  const nextTitle = formatSeoTitle(title)

  document.title = nextTitle
  ensureMeta('meta[property="og:title"]', 'property', 'og:title').content = nextTitle
  ensureMeta('meta[name="twitter:title"]', 'name', 'twitter:title').content = nextTitle
}

export function installSeo(router: Router): void {
  router.afterEach((route) => {
    const locale = routeLocale(route.params.locale) ?? 'en'

    setLocale(locale)
    updateSeo(route)
  })
}
