import type { CatalogSort } from '@/types/catalog'

export type CatalogOption = {
  value: string
  labelKey?: string
}

export const genderOptions: CatalogOption[] = [
  {
    value: 'Women',
    labelKey: 'catalog.options.Women',
  },
  {
    value: 'Men',
    labelKey: 'catalog.options.Men',
  },
  {
    value: 'Unisex',
    labelKey: 'catalog.options.Unisex',
  },
]

export const categoryOptions: CatalogOption[] = [
  {
    value: 'Tops',
    labelKey: 'catalog.options.Tops',
  },
  {
    value: 'Bottoms',
    labelKey: 'catalog.options.Bottoms',
  },
  {
    value: 'Shoes',
    labelKey: 'catalog.options.Shoes',
  },
  {
    value: 'Bags',
    labelKey: 'catalog.options.Bags',
  },
  {
    value: 'Accessories',
    labelKey: 'catalog.options.Accessories',
  },
  {
    value: 'Jewellery',
    labelKey: 'catalog.options.Jewellery',
  },
  {
    value: 'Vintage',
    labelKey: 'catalog.options.Vintage',
  },
]

export const sizeOptions: CatalogOption[] = [
  {
    value: 'XS',
    labelKey: 'catalog.options.XS',
  },
  {
    value: 'S',
    labelKey: 'catalog.options.S',
  },
  {
    value: 'M',
    labelKey: 'catalog.options.M',
  },
  {
    value: 'L',
    labelKey: 'catalog.options.L',
  },
  {
    value: 'XL',
    labelKey: 'catalog.options.XL',
  },
  {
    value: 'XXL',
    labelKey: 'catalog.options.XXL',
  },
]

export const conditionOptions: CatalogOption[] = [
  {
    value: 'New',
    labelKey: 'catalog.options.New',
  },
  {
    value: 'Worn',
    labelKey: 'catalog.options.Worn',
  },
  {
    value: 'Refurbished',
    labelKey: 'catalog.options.Refurbished',
  },
]

export const brandOptions: CatalogOption[] = [
  { value: 'Balenciaga' },
  { value: 'Rick Owens' },
  { value: 'Prada' },
  { value: 'Gucci' },
  { value: 'Saint Laurent' },
  { value: 'Maison Margiela' },
  { value: 'Raf Simons' },
  { value: 'Stone Island' },
]

export const countryOptions: CatalogOption[] = [
  {
    value: 'Latvia',
    labelKey: 'catalog.options.Latvia',
  },
  {
    value: 'Lithuania',
    labelKey: 'catalog.options.Lithuania',
  },
  {
    value: 'Estonia',
    labelKey: 'catalog.options.Estonia',
  },
  {
    value: 'Germany',
    labelKey: 'catalog.options.Germany',
  },
  {
    value: 'France',
    labelKey: 'catalog.options.France',
  },
  {
    value: 'Italy',
    labelKey: 'catalog.options.Italy',
  },
  {
    value: 'Spain',
    labelKey: 'catalog.options.Spain',
  },
  {
    value: 'Netherlands',
    labelKey: 'catalog.options.Netherlands',
  },
  {
    value: 'Belgium',
    labelKey: 'catalog.options.Belgium',
  },
  {
    value: 'Poland',
    labelKey: 'catalog.options.Poland',
  },
]

export const sortOptions: Array<{
  value: CatalogSort
  labelKey: string
}> = [
  {
    value: 'EndingSoon',
    labelKey: 'catalog.sort.EndingSoon',
  },
  {
    value: 'NewlyListed',
    labelKey: 'catalog.sort.NewlyListed',
  },
  {
    value: 'PriceLowToHigh',
    labelKey: 'catalog.sort.PriceLowToHigh',
  },
  {
    value: 'PriceHighToLow',
    labelKey: 'catalog.sort.PriceHighToLow',
  },
  {
    value: 'MostBids',
    labelKey: 'catalog.sort.MostBids',
  },
]
