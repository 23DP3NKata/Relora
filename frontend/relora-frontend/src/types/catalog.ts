export const CATALOG_SORT_VALUES = [
  'EndingSoon',
  'NewlyListed',
  'PriceLowToHigh',
  'PriceHighToLow',
  'MostBids',
] as const

export type CatalogSort =
  (typeof CATALOG_SORT_VALUES)[number]

export type CatalogArrayFilterKey =
  | 'departments'
  | 'genders'
  | 'categories'
  | 'rootCategorySlugs'
  | 'categorySlugs'
  | 'brands'
  | 'sizes'
  | 'conditions'
  | 'countries'
  | 'materialIds'
  | 'primaryColorIds'

export type CatalogFilterKey =
  | CatalogArrayFilterKey
  | 'price'
  | 'productionYear'
  | 'vintageOnly'
  | 'hasMeasurements'
  | 'hasProofOfOrigin'
  | 'endingSoon'
  | 'newlyListed'

export type CatalogFilters = {
  departments: string[]
  genders: string[]
  categories: string[]
  rootCategorySlugs: string[]
  categorySlugs: string[]
  brands: string[]
  sizes: string[]
  conditions: string[]
  countries: string[]
  materialIds: string[]
  primaryColorIds: string[]

  minPrice: number | null
  maxPrice: number | null
  productionYearFrom: number | null
  productionYearTo: number | null

  vintageOnly: boolean
  hasMeasurements: boolean
  hasProofOfOrigin: boolean
  endingSoon: boolean
  newlyListed: boolean
}

export type CatalogLockedFilters = Partial<
  Pick<
    CatalogFilters,
    | 'departments'
    | 'genders'
    | 'categories'
    | 'rootCategorySlugs'
    | 'categorySlugs'
    | 'brands'
    | 'sizes'
    | 'conditions'
    | 'countries'
    | 'materialIds'
    | 'primaryColorIds'
    | 'vintageOnly'
  >
>

export type CatalogActiveFilter = {
  id: string
  key: CatalogFilterKey
  value?: string
  label: string
  removable: boolean
}

export type GetLotsListParams = {
  search?: string

  departments?: string[]
  genders?: string[]
  categories?: string[]
  rootCategorySlugs?: string[]
  categorySlugs?: string[]
  brands?: string[]
  sizes?: string[]
  conditions?: string[]
  countries?: string[]
  materialIds?: string[]
  primaryColorIds?: string[]

  minPrice?: number
  maxPrice?: number
  productionYearFrom?: number
  productionYearTo?: number

  vintageOnly?: boolean
  hasMeasurements?: boolean
  hasProofOfOrigin?: boolean
  endingSoon?: boolean
  newlyListed?: boolean

  sort?: CatalogSort

  page: number
  pageSize: number
}

export function createEmptyCatalogFilters(): CatalogFilters {
  return {
    departments: [],
    genders: [],
    categories: [],
    rootCategorySlugs: [],
    categorySlugs: [],
    brands: [],
    sizes: [],
    conditions: [],
    countries: [],
    materialIds: [],
    primaryColorIds: [],

    minPrice: null,
    maxPrice: null,
    productionYearFrom: null,
    productionYearTo: null,

    vintageOnly: false,
    hasMeasurements: false,
    hasProofOfOrigin: false,
    endingSoon: false,
    newlyListed: false,
  }
}

export function cloneCatalogFilters(
  filters: CatalogFilters,
): CatalogFilters {
  return {
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

    vintageOnly: filters.vintageOnly,
    hasMeasurements: filters.hasMeasurements,
    hasProofOfOrigin: filters.hasProofOfOrigin,
    endingSoon: filters.endingSoon,
    newlyListed: filters.newlyListed,
  }
}

export function isCatalogArrayFilterKey(
  key: CatalogFilterKey,
): key is CatalogArrayFilterKey {
  return [
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
  ].includes(key)
}
