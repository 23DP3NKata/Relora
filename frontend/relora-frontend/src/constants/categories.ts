import type { RouteLocationRaw } from 'vue-router'

type NavigationCategory = {
  labelKey: string
  to: RouteLocationRaw
}

export const categories: NavigationCategory[] = [
  {
    labelKey: 'navigation.women',
    to: {
      name: 'catalog-gender',
      params: {
        gender: 'women',
      },
    },
  },
  {
    labelKey: 'navigation.men',
    to: {
      name: 'catalog-gender',
      params: {
        gender: 'men',
      },
    },
  },
  {
    labelKey: 'navigation.bags',
    to: {
      name: 'catalog-global-category',
      params: {
        category: 'bags',
      },
    },
  },
  {
    labelKey: 'navigation.shoes',
    to: {
      name: 'catalog-global-category',
      params: {
        category: 'shoes',
      },
    },
  },
  {
    labelKey: 'navigation.jewelryWatches',
    to: {
      name: 'catalog-global-category',
      params: {
        category: 'jewellery',
      },
    },
  },
  {
    labelKey: 'navigation.vintage',
    to: {
      name: 'catalog-global-category',
      params: {
        category: 'vintage',
      },
    },
  },
  {
    labelKey: 'navigation.accessories',
    to: {
      name: 'catalog-global-category',
      params: {
        category: 'accessories',
      },
    },
  },
  {
    labelKey: 'navigation.designers',
    to: {
      name: 'designers',
    },
  },
]
