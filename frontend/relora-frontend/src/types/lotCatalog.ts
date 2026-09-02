export type LotDepartment = "Women" | "Men" | "Unisex"

export type LookupOption = {
  id: string
  code: string
  nameKey: string
  sortOrder: number
  isActive: boolean
}

export type CategoryNode = {
  id: string
  parentId: string | null
  slug: string
  nameKey: string
  sortOrder: number
  isActive: boolean
  measurementProfile: string
  allowedDepartments: LotDepartment[]
  children: CategoryNode[]
}

export type MeasurementDefinition = {
  id: string
  profile: string
  key: string
  labelKey: string
  isRequired: boolean
  sortOrder: number
  unit: string
}

export type SizeOption = {
  code: string
  nameKey: string
  categoryGroup: string
}

export type LotFormLookups = {
  departments: LotDepartment[]
  categories: CategoryNode[]
  materials: LookupOption[]
  colors: LookupOption[]
  proofDocumentTypes: LookupOption[]
  measurementDefinitions: MeasurementDefinition[]
  sizeOptions: SizeOption[]
}

export type LotMaterialInput = {
  materialId: string
  percentage: number | null
  otherName?: string | null
}

export type LotMeasurementInput = {
  key: string
  value: number
  unit?: string | null
}

export type ProofDocumentUploadInput = {
  uploadId: string
}

export type UploadedProofDocument = {
  id: string
  uploadId: string
  documentTypeId: string
  originalFileName: string
  mimeType: string
  sizeBytes: number
  createdAtUtc: string
  isExisting?: boolean
}
