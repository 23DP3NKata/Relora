import type { LotDepartment } from '@/types/lotCatalog'

export type LotStatusLabel =
  | 'Draft'
  | 'Pending'
  | 'Approved'
  | 'Active'
  | 'Rejected'
  | 'Sold'
  | 'Expired'
  | 'Unknown'

export type Lot = {
  id: string
  title: string
  description: string
  price: number
  currency: string
  category: number
  categoryName: string
  gender: number
  genderName: string
  size: number
  sizeName: string
  brand: string
  condition: number
  conditionName: string
  color: string
  categoryId: string
  rootCategoryId?: string | null
  categorySlug?: string | null
  categoryNameKey?: string | null
  rootCategorySlug?: string | null
  rootCategoryNameKey?: string | null
  department: LotDepartment
  departmentName: string
  primaryColorId: string
  primaryColor?: LotColor | null
  secondaryColors?: LotColor[]
  modelName?: string | null
  acquisitionYear?: number | null
  productionYear?: number | null
  isVintage?: boolean
  vintageNotes?: string | null
  materials?: LotMaterial[]
  measurements?: LotMeasurement[]
  proofStatus?: string
  proofDocuments?: LotProofDocument[]
  status?: number | string
  statusName?: string
  media: LotMedia[]
  seller: LotSeller
  sellerId?: string
  country: string
  city: string
  age: string
  style: string
  shippingPrice: number
  shippingCurrency: string
  shippingOriginCountry: string
  shipsToCountries: string
  shippingHandlingDays: number
  createdAt?: string
}

export type LotPreview = {
  id: string
  title: string
  price: number
  currency: string
  category?: number
  categoryName?: string
  gender?: number
  genderName?: string
  size?: number
  sizeName?: string
  brand: string
  condition?: number
  conditionName?: string
  color?: string
  categoryId?: string
  rootCategoryId?: string | null
  categorySlug?: string | null
  categoryNameKey?: string | null
  rootCategorySlug?: string | null
  rootCategoryNameKey?: string | null
  department?: LotDepartment
  departmentName?: string
  primaryColorId?: string
  productionYear?: number | null
  isVintage?: boolean
  hasMeasurements?: boolean
  proofStatus?: string
  status?: number | string
  statusName?: string
  media: LotMedia[]
  sellerId?: string
  seller?: LotSeller
  auctionId?: string
  country?: string
  city?: string
  age?: string
  style?: string
  shippingPrice?: number
  shippingCurrency?: string
  shippingOriginCountry?: string
  shipsToCountries?: string
  shippingHandlingDays?: number
  createdAt?: string
}

export type LotMedia = {
  id?: string
  type: string
  key: string
  url: string
  sortOrder?: number
  isCover?: boolean
}

export type LotSeller = {
  id: string
  username: string
  name: string
}

export type LotMaterial = {
  materialId: string
  code: string
  nameKey: string
  percentage?: number | null
  otherName?: string | null
}

export type LotColor = {
  colorId: string
  code: string
  nameKey: string
}

export type LotMeasurement = {
  key: string
  value: number
  unit: string
  labelKey?: string | null
}

export type LotProofDocument = {
  id: string
  documentTypeId: string
  typeCode: string
  typeNameKey: string
  originalFileName: string
  mimeType: string
  sizeBytes: number
  createdAtUtc: string
}


export type LotsFilters = {
  search?: string;
  brand?: string;
  category?: string | number;
  gender?: string | number;
  size?: string | number;
  condition?: string | number;
  status?: string | number;
};
