import { computed } from 'vue'
import {
  useRoute,
  type RouteLocationRaw,
} from 'vue-router'

import {
  getPreferredLocale,
  localizedFullPath,
  routeLocale,
  type AppLocale,
} from '@/translate'

export function useCurrentLocale() {
  const route = useRoute()

  return computed<AppLocale>(
    () => routeLocale(route.params.locale) ?? getPreferredLocale(),
  )
}

export function useLocalePath() {
  const locale = useCurrentLocale()

  return (to: RouteLocationRaw): RouteLocationRaw => {
    if (typeof to === 'string') {
      if (
        to.startsWith('#') ||
        to.startsWith('http://') ||
        to.startsWith('https://') ||
        to.startsWith('mailto:')
      ) {
        return to
      }

      return localizedFullPath(to, locale.value)
    }

    const target = to as {
      path?: string
      params?: Record<string, unknown>
      [key: string]: unknown
    }

    if (target.path) {
      return {
        ...target,
        path: localizedFullPath(target.path, locale.value),
      } as RouteLocationRaw
    }

    return {
      ...target,
      params: {
        ...(target.params ?? {}),
        locale: locale.value,
      },
    } as RouteLocationRaw
  }
}
