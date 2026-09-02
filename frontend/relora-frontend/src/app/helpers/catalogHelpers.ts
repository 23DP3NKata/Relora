import type {
  LocationQuery,
  LocationQueryRaw,
} from 'vue-router'

import {
  CATALOG_SORT_VALUES,
  createEmptyCatalogFilters,
  type CatalogArrayFilterKey,
  type CatalogFilters,
  type CatalogLockedFilters,
  type CatalogSort,
} from '@/types/catalog'

const catalogArrayKeys: CatalogArrayFilterKey[] = [
  'departments',
  'genders',
  'categories',
  'rootCategorySlugs',
  'categorySlugs',
  'brands',
  'sizes',
  'conditions',
  'countries',
  'materialIds',
  'primaryColorIds',
]

const catalogFilterQueryKeys = [
  'departments',
  'genders',
  'categories',
  'rootCategorySlugs',
  'categorySlugs',
  'brands',
  'sizes',
  'conditions',
  'countries',
  'materialIds',
  'primaryColorIds',

  // Legacy singular query parameters are kept so old shared links still work.
  'department',
  'gender',
  'category',
  'rootCategory',
  'categorySlug',
  'brand',
  'size',
  'condition',
  'country',
  'materialId',
  'primaryColorId',

  'minPrice',
  'maxPrice',
  'productionYearFrom',
  'productionYearTo',
  'vintageOnly',
  'hasMeasurements',
  'hasProofOfOrigin',
  'endingSoon',
  'newlyListed',
]

function uniqueValues(values: string[]): string[] {
  return [...new Set(values)]
}

function normalizeQueryValues(
  value: unknown,
): string[] {
  if (typeof value === 'string') {
    const trimmedValue = value.trim()

    return trimmedValue
      ? [trimmedValue]
      : []
  }

  if (Array.isArray(value)) {
    return value
      .filter(
        (item): item is string =>
          typeof item === 'string',
      )
      .map(item => item.trim())
      .filter(Boolean)
  }

  return []
}

function readMany(
  query: LocationQuery,
  pluralKey: string,
  legacySingularKey?: string,
): string[] {
  const pluralValues = normalizeQueryValues(
    query[pluralKey],
  )

  const singularValues = legacySingularKey
    ? normalizeQueryValues(query[legacySingularKey])
    : []

  return uniqueValues([
    ...pluralValues,
    ...singularValues,
  ])
}

function readNumber(
  query: LocationQuery,
  key: string,
): number | null {
  const value = query[key]

  if (typeof value !== 'string') {
    return null
  }

  const parsedValue = Number(value)

  if (
    !Number.isFinite(parsedValue) ||
    parsedValue < 0
  ) {
    return null
  }

  return parsedValue
}

function readBoolean(
  query: LocationQuery,
  key: string,
): boolean {
  const value = query[key]

  return value === 'true' || value === '1'
}

export function getStringQueryParam(
  query: LocationQuery,
  key: string,
): string | undefined {
  const value = query[key]

  if (typeof value !== 'string') {
    return undefined
  }

  const trimmedValue = value.trim()

  return trimmedValue || undefined
}

export function parseCatalogFilters(
  query: LocationQuery,
): CatalogFilters {
  return {
    departments: readMany(
      query,
      'departments',
      'department',
    ),

    genders: readMany(
      query,
      'genders',
      'gender',
    ),

    categories: readMany(
      query,
      'categories',
      'category',
    ),

    rootCategorySlugs: readMany(
      query,
      'rootCategorySlugs',
      'rootCategory',
    ),

    categorySlugs: readMany(
      query,
      'categorySlugs',
      'categorySlug',
    ),

    brands: readMany(
      query,
      'brands',
      'brand',
    ),

    sizes: readMany(
      query,
      'sizes',
      'size',
    ),

    conditions: readMany(
      query,
      'conditions',
      'condition',
    ),

    countries: readMany(
      query,
      'countries',
      'country',
    ),

    materialIds: readMany(
      query,
      'materialIds',
      'materialId',
    ),

    primaryColorIds: readMany(
      query,
      'primaryColorIds',
      'primaryColorId',
    ),

    minPrice: readNumber(
      query,
      'minPrice',
    ),

    maxPrice: readNumber(
      query,
      'maxPrice',
    ),

    productionYearFrom: readNumber(
      query,
      'productionYearFrom',
    ),

    productionYearTo: readNumber(
      query,
      'productionYearTo',
    ),

    vintageOnly: readBoolean(
      query,
      'vintageOnly',
    ),

    hasMeasurements: readBoolean(
      query,
      'hasMeasurements',
    ),

    hasProofOfOrigin: readBoolean(
      query,
      'hasProofOfOrigin',
    ),

    endingSoon: readBoolean(
      query,
      'endingSoon',
    ),

    newlyListed: readBoolean(
      query,
      'newlyListed',
    ),
  }
}

export function parseCatalogSort(
  value: string | undefined,
): CatalogSort {
  if (
    value &&
    CATALOG_SORT_VALUES.includes(
      value as CatalogSort,
    )
  ) {
    return value as CatalogSort
  }

  return 'NewlyListed'
}

export function serializeCatalogFilters(
  filters: CatalogFilters,
): LocationQueryRaw {
  const query: LocationQueryRaw = {}

  if (filters.departments.length > 0) {
    query.departments = filters.departments
  }

  if (filters.genders.length > 0) {
    query.genders = filters.genders
  }

  if (filters.categories.length > 0) {
    query.categories = filters.categories
  }

  if (filters.rootCategorySlugs.length > 0) {
    query.rootCategorySlugs = filters.rootCategorySlugs
  }

  if (filters.categorySlugs.length > 0) {
    query.categorySlugs = filters.categorySlugs
  }

  if (filters.brands.length > 0) {
    query.brands = filters.brands
  }

  if (filters.sizes.length > 0) {
    query.sizes = filters.sizes
  }

  if (filters.conditions.length > 0) {
    query.conditions = filters.conditions
  }

  if (filters.countries.length > 0) {
    query.countries = filters.countries
  }

  if (filters.materialIds.length > 0) {
    query.materialIds = filters.materialIds
  }

  if (filters.primaryColorIds.length > 0) {
    query.primaryColorIds = filters.primaryColorIds
  }

  if (filters.minPrice !== null) {
    query.minPrice = String(filters.minPrice)
  }

  if (filters.maxPrice !== null) {
    query.maxPrice = String(filters.maxPrice)
  }

  if (filters.productionYearFrom !== null) {
    query.productionYearFrom = String(filters.productionYearFrom)
  }

  if (filters.productionYearTo !== null) {
    query.productionYearTo = String(filters.productionYearTo)
  }

  if (filters.vintageOnly) {
    query.vintageOnly = 'true'
  }

  if (filters.hasMeasurements) {
    query.hasMeasurements = 'true'
  }

  if (filters.hasProofOfOrigin) {
    query.hasProofOfOrigin = 'true'
  }

  if (filters.endingSoon) {
    query.endingSoon = 'true'
  }

  if (filters.newlyListed) {
    query.newlyListed = 'true'
  }

  return query
}

export function removeCatalogFilterQuery(
  query: LocationQuery,
): LocationQueryRaw {
  const result: LocationQueryRaw = {
    ...query,
  }

  for (const key of catalogFilterQueryKeys) {
    delete result[key]
  }

  return result
}

export function mergeLockedFilters(
  filters: CatalogFilters,
  locked: CatalogLockedFilters,
): CatalogFilters {
  const result: CatalogFilters = {
    departments: [...filters.departments],
    genders: [...filters.genders],
    categories: [...filters.categories],
    rootCategorySlugs: [...filters.rootCategorySlugs],
    categorySlugs: [...filters.categorySlugs],
    brands: [...filters.brands],
    sizes: [...filters.sizes],
    conditions: [...filters.conditions],
    countries: [...filters.countries],
    materialIds: [...filters.materialIds],
    primaryColorIds: [...filters.primaryColorIds],

    minPrice: filters.minPrice,
    maxPrice: filters.maxPrice,
    productionYearFrom: filters.productionYearFrom,
    productionYearTo: filters.productionYearTo,

    vintageOnly:
      locked.vintageOnly === true ||
      filters.vintageOnly,
    hasMeasurements: filters.hasMeasurements,
    hasProofOfOrigin: filters.hasProofOfOrigin,
    endingSoon: filters.endingSoon,
    newlyListed: filters.newlyListed,
  }

  for (const key of catalogArrayKeys) {
    const lockedValues = locked[key] ?? []

    result[key] = uniqueValues([
      ...result[key],
      ...lockedValues,
    ])
  }

  return result
}

export function stripLockedFilters(
  filters: CatalogFilters,
  locked: CatalogLockedFilters,
): CatalogFilters {
  const result: CatalogFilters = {
    ...createEmptyCatalogFilters(),

    minPrice: filters.minPrice,
    maxPrice: filters.maxPrice,
    productionYearFrom: filters.productionYearFrom,
    productionYearTo: filters.productionYearTo,

    vintageOnly:
      locked.vintageOnly === true
        ? false
        : filters.vintageOnly,
    hasMeasurements: filters.hasMeasurements,
    hasProofOfOrigin: filters.hasProofOfOrigin,
    endingSoon: filters.endingSoon,
    newlyListed: filters.newlyListed,
  }

  for (const key of catalogArrayKeys) {
    const lockedValues = new Set(
      locked[key] ?? [],
    )

    result[key] = filters[key].filter(
      value => !lockedValues.has(value),
    )
  }

  return result
}
