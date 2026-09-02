import type {
  LotDepartment,
  LotMaterialInput,
  LotMeasurementInput,
  ProofDocumentUploadInput,
  UploadedProofDocument,
} from "@/types/lotCatalog"

export type SellStepKey =
  | "intro"
  | "category"
  | "details"
  | "measurements"
  | "photos"
  | "pricing"
  | "review"

export type UploadedPhoto = {
  id: string
  key: string
  previewUrl: string
  fileName: string
  file?: File
  uploadStatus?: "uploading" | "uploaded" | "failed"
  errorMessage?: string
  isCover?: boolean
  isExisting?: boolean
}

export type CreateLotPayload = {
  title: string
  description: string
  amount: number
  currency: string
  category: number
  gender: number
  size: number
  brand: string
  condition: number
  color: string
  country: string
  city: string
  categoryId: string
  department: LotDepartment
  primaryColorId: string
  modelName?: string | null
  acquisitionYear?: number | null
  productionYear?: number | null
  isVintage: boolean
  vintageNotes?: string | null
  age: string
  style: string
  shippingPrice: number
  shippingCurrency: string
  shippingOriginCountry: string
  shipsToCountries: string
  shippingHandlingDays: number
  photoKeys: string[]
  coverPhotoKey?: string | null
  materials: LotMaterialInput[]
  measurements: LotMeasurementInput[]
  proofDocuments: ProofDocumentUploadInput[]
}

export type CreateLotResponse = string

export type CreateLotFormState = {
  title: string
  description: string
  amount: number | null
  currency: string
  category: number | null
  gender: number | null
  size: number | null
  brand: string
  condition: number | null
  color: string
  country: string
  city: string
  rootCategoryId: string
  categoryId: string
  department: LotDepartment | null
  primaryColorId: string
  modelName: string
  acquisitionYear: number | null
  productionYear: number | null
  isVintage: boolean
  vintageNotes: string
  materials: LotMaterialInput[]
  measurements: LotMeasurementInput[]
  proofDocuments: UploadedProofDocument[]
  age: string
  style: string
  shippingPrice: number | null
  shippingCurrency: string
  shippingOriginCountry: string
  shipsToCountries: string
  shippingHandlingDays: number
  photoKeys: string[]
  coverPhotoKey: string
}

export const SELL_STEPS: SellStepKey[] = [
  "intro",
  "category",
  "details",
  "measurements",
  "photos",
  "pricing",
  "review",
]

export const DEFAULT_CREATE_LOT_FORM: CreateLotFormState = {
  title: "",
  description: "",
  amount: null,
  currency: "EUR",
  category: 8,
  gender: null,
  size: null,
  brand: "",
  condition: null,
  color: "",
  rootCategoryId: "",
  categoryId: "",
  department: null,
  primaryColorId: "",
  modelName: "",
  acquisitionYear: null,
  productionYear: null,
  isVintage: false,
  vintageNotes: "",
  materials: [],
  measurements: [],
  proofDocuments: [],
  photoKeys: [],
  coverPhotoKey: "",
  country: "",
  city: "",
  age: "",
  style: "",
  shippingPrice: null,
  shippingCurrency: "EUR",
  shippingOriginCountry: "",
  shipsToCountries: "",
  shippingHandlingDays: 3,
}
