export type NumericOption = {
  labelKey: string
  value: number
}

export const CATEGORY_OPTIONS = [
  { labelKey: "catalog.options.Tops", value: 0 },
  { labelKey: "catalog.options.Bottoms", value: 1 },
  { labelKey: "catalog.options.Outerwear", value: 2 },
  { labelKey: "catalog.options.Footwear", value: 3 },
  { labelKey: "catalog.options.Accessories", value: 4 },
  { labelKey: "catalog.options.Other", value: 5 },
]

export const GENDER_OPTIONS = [
  { labelKey: "catalog.options.Women", value: 1 },
  { labelKey: "catalog.options.Men", value: 2 },
  { labelKey: "catalog.options.Unisex", value: 3 },
]

export const SIZE_OPTIONS = [
  { labelKey: "catalog.options.XXS", value: 0 },
  { labelKey: "catalog.options.XS", value: 1 },
  { labelKey: "catalog.options.S", value: 2 },
  { labelKey: "catalog.options.M", value: 3 },
  { labelKey: "catalog.options.L", value: 4 },
  { labelKey: "catalog.options.XL", value: 5 },
  { labelKey: "catalog.options.XXL", value: 6 },
]

export const CONDITION_OPTIONS = [
  { labelKey: "catalog.options.New", value: 0 },
  { labelKey: "catalog.options.Worn", value: 1 },
  { labelKey: "catalog.options.Refurbished", value: 2 },
]

export const getOptionLabel = (
  value: number | null,
  options: NumericOption[],
): string => {
  if (value === null) {
    return ""
  }

  return options.find((option) => option.value === value)?.labelKey ?? ""
}
