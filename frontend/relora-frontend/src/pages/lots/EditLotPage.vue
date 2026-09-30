<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, reactive, ref, watch } from "vue"
import { useRoute, useRouter } from "vue-router"
import { storeToRefs } from "pinia"
import { toast } from "vue-sonner"
import { useI18n } from "vue-i18n"

import SellCategoryStep from "@/components/sell/SellCategoryStep.vue"
import SellDetailsStep from "@/components/sell/SellDetailsStep.vue"
import SellMeasurementsStep from "@/components/sell/SellMeasurementsStep.vue"
import SellPhotosStep from "@/components/sell/SellPhotosStep.vue"
import SellPricingStep from "@/components/sell/SellPricingStep.vue"
import SellReviewStep from "@/components/sell/SellReviewStep.vue"
import { lookupService } from "@/app/services/lookupService"
import { itemService, type EditLotPayload } from "@/app/services/lotService"
import { mediaService } from "@/app/services/mediaService"
import { useAuthStore } from "@/stores/authStore"
import { CONDITION_OPTIONS, getOptionLabel, SIZE_OPTIONS } from "@/features/lots/create-lot/options"
import {
  DEFAULT_CREATE_LOT_FORM,
  type CreateLotFormState,
  type UploadedPhoto,
} from "@/types/createLot"
import type { CategoryNode, LotDepartment, LotFormLookups, LotMaterialInput, LotMeasurementInput, UploadedProofDocument } from "@/types/lotCatalog"
import type { Lot, LotMedia, LotProofDocument } from "@/types/lot"
import { useLocalePath } from "@/composables/useLocalePath"

const MIN_PHOTOS = 5
const MAX_PHOTOS = 15
const MIN_DESCRIPTION_LENGTH = 100
const VINTAGE_MIN_AGE_YEARS = 15
const MIN_PRODUCTION_YEAR = 1800
const MIN_ACQUISITION_YEAR = 1900

const EDIT_STEPS = ["category", "details", "measurements", "photos", "pricing", "review"] as const
type EditStepKey = typeof EDIT_STEPS[number]

type ApiError = {
  response?: {
    data?: {
      message?: string
      errors?: Record<string, string | string[]>
    }
  }
}

const route = useRoute()
const router = useRouter()
const localePath = useLocalePath()
const { t, te } = useI18n()

const authStore = useAuthStore()
const { user } = storeToRefs(authStore)

const lot = ref<Lot | null>(null)
const lookups = ref<LotFormLookups | null>(null)
const photos = ref<UploadedPhoto[]>([])
const isLoading = ref(true)
const isSubmitting = ref(false)
const isUploadingPhotos = ref(false)
const isUploadingProofDocuments = ref(false)
const errorMessage = ref("")
const successMessage = ref("")
const currentStepIndex = ref(0)
const initialSnapshot = ref("")

const form = reactive<CreateLotFormState>({
  ...DEFAULT_CREATE_LOT_FORM,
  materials: [...DEFAULT_CREATE_LOT_FORM.materials],
  measurements: [...DEFAULT_CREATE_LOT_FORM.measurements],
  proofDocuments: [...DEFAULT_CREATE_LOT_FORM.proofDocuments],
  photoKeys: [...DEFAULT_CREATE_LOT_FORM.photoKeys],
  coverPhotoKey: DEFAULT_CREATE_LOT_FORM.coverPhotoKey,
})

const currentStep = computed<EditStepKey>(() => EDIT_STEPS[currentStepIndex.value] ?? "category")
const isFirstStep = computed(() => currentStepIndex.value === 0)
const isLastStep = computed(() => currentStepIndex.value === EDIT_STEPS.length - 1)
const visibleStepNumber = computed(() => currentStepIndex.value + 1)
const progressPercent = computed(() => Math.round(((currentStepIndex.value + 1) / EDIT_STEPS.length) * 100))

const normalizeStatus = computed(() => {
  const raw = String(lot.value?.statusName ?? lot.value?.status ?? "").toLowerCase()
  if (raw.includes("draft")) return "draft"
  if (raw.includes("reject")) return "rejected"
  if (raw.includes("pend")) return "pending"
  if (raw.includes("active")) return "active"
  if (raw.includes("approve") || raw.includes("publish")) return "approved"
  if (raw.includes("sold")) return "sold"
  if (raw.includes("expir")) return "expired"
  return "unknown"
})

const canEdit = computed(() => ["draft", "rejected", "pending"].includes(normalizeStatus.value))
const localizedStatus = computed(() => {
  const status = lot.value?.statusName ?? normalizeStatus.value
  const key = `catalog.status.${status}`
  return te(key) ? t(key) : status
})

const rootCategories = computed(() =>
  (lookups.value?.categories ?? [])
    .filter((category) => category.isActive)
    .filter((category) => categoryAllowsDepartment(category, form.department)),
)
const selectedRootCategory = computed(() =>
  rootCategories.value.find((category) => category.id === form.rootCategoryId) ?? null,
)
const subcategories = computed(() =>
  (selectedRootCategory.value?.children ?? [])
    .filter((category) => category.isActive)
    .filter((category) => categoryAllowsDepartment(category, form.department)),
)
const allCategories = computed(() => flattenCategories(lookups.value?.categories ?? []))
const selectedCategory = computed(() =>
  allCategories.value.find((category) => category.id === form.categoryId) ?? null,
)
const measurementDefinitions = computed(() => {
  const profile = selectedCategory.value?.measurementProfile

  if (!profile) {
    return []
  }

  return (lookups.value?.measurementDefinitions ?? [])
    .filter((definition) => definition.profile === profile)
    .sort((first, second) => first.sortOrder - second.sortOrder)
})

const uploadedPhotoKeys = computed(() =>
  photos.value
    .filter((photo) => photo.uploadStatus !== "failed")
    .map((photo) => photo.key)
    .filter((key): key is string => Boolean(key)),
)

const reviewForm = computed(() => ({
  ...form,
  category: selectedCategory.value ? localizedOptionLabel(selectedCategory.value.nameKey) : "",
  gender: form.department ? localizedOptionLabel(`departments.${form.department}`) : "",
  size: localizedOptionLabel(getOptionLabel(form.size, SIZE_OPTIONS)),
  condition: localizedOptionLabel(getOptionLabel(form.condition, CONDITION_OPTIONS)),
  proofCount: form.proofDocuments.length,
  measurementsCount: form.measurements.length,
  materials: form.materials
    .map((material) => lookups.value?.materials.find((option) => option.id === material.materialId)?.nameKey)
    .filter((labelKey): labelKey is string => Boolean(labelKey))
    .map((labelKey) => localizedOptionLabel(labelKey)),
  primaryColor: localizedOptionLabel(
    lookups.value?.colors.find((color) => color.id === form.primaryColorId)?.nameKey ?? "",
  ),
}))

const isDirty = computed(() => Boolean(initialSnapshot.value) && createSnapshot() !== initialSnapshot.value)

function createClientId() {
  if (
    typeof window !== "undefined" &&
    window.crypto &&
    typeof window.crypto.randomUUID === "function"
  ) {
    return window.crypto.randomUUID()
  }

  return `${Date.now()}-${Math.random().toString(36).slice(2)}`
}

function localizedOptionLabel(labelKey: string | null | undefined) {
  return labelKey && te(labelKey) ? t(labelKey) : labelKey || ""
}

function flattenCategories(categories: CategoryNode[]): CategoryNode[] {
  return categories.flatMap((category) => [
    category,
    ...flattenCategories(category.children ?? []),
  ])
}

function categoryAllowsDepartment(category: CategoryNode, department: LotDepartment | null): boolean {
  return !department ||
    category.allowedDepartments.length === 0 ||
    category.allowedDepartments.includes(department)
}

function departmentToLegacyGender(department: LotDepartment | number | null): number | null {
  if (department === 1 || department === 2 || department === 3) return department
  if (department === "Women") return 1
  if (department === "Men") return 2
  if (department === "Unisex") return 3
  return null
}

function normalizeDepartment(value?: string | number | null): LotDepartment {
  if (value === 1) return "Women"
  if (value === 2) return "Men"
  if (value === 3) return "Unisex"

  if (value === "Women" || value === "Men" || value === "Unisex") {
    return value
  }

  return "Unisex"
}

function mapRootSlugToLegacyCategory(slug?: string | null) {
  switch (slug) {
    case "shoes":
      return 3
    case "accessories":
      return 4
    case "bags":
      return 6
    case "jewellery":
    case "watches":
      return 7
    default:
      return 8
  }
}

function extractApiErrorMessage(error: unknown): string | null {
  const typedError = error as ApiError
  const errors = typedError.response?.data?.errors

  if (errors && typeof errors === "object") {
    const firstError = Object.values(errors)
      .flatMap((value) => Array.isArray(value) ? value : [value])
      .find((value) => typeof value === "string" && value.trim())

    if (typeof firstError === "string") {
      return firstError
    }
  }

  const message = typedError.response?.data?.message
  return typeof message === "string" && message.trim() ? message : null
}

function mediaFileName(media: LotMedia) {
  return media.key.split("/").pop() || media.key
}

function mapExistingPhoto(media: LotMedia): UploadedPhoto {
  return {
    id: media.id ?? media.key,
    key: media.key,
    previewUrl: media.url,
    fileName: mediaFileName(media),
    uploadStatus: "uploaded",
    isCover: Boolean(media.isCover),
    isExisting: true,
  }
}

function mapExistingProofDocument(document: LotProofDocument): UploadedProofDocument {
  return {
    id: document.id,
    uploadId: "",
    documentTypeId: document.documentTypeId,
    originalFileName: document.originalFileName,
    mimeType: document.mimeType,
    sizeBytes: document.sizeBytes,
    createdAtUtc: document.createdAtUtc,
    isExisting: true,
  }
}

function syncPhotoState() {
  const uploadedPhotos = photos.value.filter((photo) => photo.key && photo.uploadStatus !== "failed")
  const coverPhoto = uploadedPhotos.find((photo) => photo.isCover) ?? uploadedPhotos[0] ?? null

  photos.value.forEach((photo) => {
    photo.isCover = Boolean(coverPhoto && photo.id === coverPhoto.id)
  })

  form.photoKeys = uploadedPhotos.map((photo) => photo.key)
  form.coverPhotoKey = coverPhoto?.key ?? ""
}

function createSnapshot() {
  return JSON.stringify({
    form: {
      ...form,
      materials: form.materials.map((material) => ({ ...material })),
      measurements: form.measurements.map((measurement) => ({ ...measurement })),
      proofDocuments: form.proofDocuments.map((document) => ({
        id: document.id,
        uploadId: document.uploadId,
        isExisting: Boolean(document.isExisting),
      })),
    },
    photos: photos.value.map((photo) => ({
      id: photo.id,
      key: photo.key,
      isCover: Boolean(photo.isCover),
      isExisting: Boolean(photo.isExisting),
    })),
  })
}

function captureSnapshot() {
  syncPhotoState()
  initialSnapshot.value = createSnapshot()
}

function hydrateForm(data: Lot) {
  const sortedMedia = [...(data.media ?? [])]
    .filter((media) => media.type === "photo")
    .sort((first, second) => (first.sortOrder ?? 0) - (second.sortOrder ?? 0))

  lot.value = data
  form.title = data.title
  form.description = data.description
  form.amount = data.price
  form.currency = data.currency
  form.category = data.category
  form.gender = data.gender
  form.size = data.size
  form.brand = data.brand
  form.condition = data.condition
  form.color = data.color
  form.country = data.country
  form.city = data.city
  form.age = data.age
  form.style = data.style
  form.rootCategoryId = data.rootCategoryId ?? ""
  form.categoryId = data.categoryId ?? ""
  form.department = normalizeDepartment(data.department ?? data.genderName)
  form.primaryColorId = data.primaryColorId ?? ""
  form.modelName = data.modelName ?? ""
  form.acquisitionYear = data.acquisitionYear ?? null
  form.productionYear = data.productionYear ?? null
  form.isVintage = Boolean(data.isVintage)
  form.vintageNotes = data.vintageNotes ?? ""
  form.materials = (data.materials ?? []).slice(0, 3).map((material) => ({
    materialId: material.materialId,
    percentage: material.percentage ?? null,
    otherName: material.otherName ?? null,
  }))
  form.measurements = (data.measurements ?? []).map((measurement) => ({
    key: measurement.key,
    value: measurement.value,
    unit: measurement.unit,
  }))
  form.proofDocuments = (data.proofDocuments ?? []).map(mapExistingProofDocument)
  form.shippingPrice = Number(data.shippingPrice ?? 0)
  form.shippingCurrency = data.shippingCurrency || data.currency || "EUR"
  form.shippingOriginCountry = data.shippingOriginCountry || data.country
  form.shipsToCountries = data.shipsToCountries || data.country
  form.shippingHandlingDays = Number(data.shippingHandlingDays ?? 3)
  photos.value = sortedMedia.map(mapExistingPhoto)

  if (photos.value.length > 0 && !photos.value.some((photo) => photo.isCover)) {
    const firstPhoto = photos.value[0]
    if (firstPhoto) {
      firstPhoto.isCover = true
    }
  }

  syncPhotoState()
  captureSnapshot()
}

async function loadLot(showLoading = true) {
  try {
    if (showLoading) {
      isLoading.value = true
    }

    const lotId = String(route.params.id)
    const [data, lookupData] = await Promise.all([
      itemService.getLot(lotId),
      lookupService.getLotFormLookups(),
    ])

    lookups.value = lookupData
    hydrateForm(data)
    errorMessage.value = ""
  } catch (error) {
    console.error(error)
    errorMessage.value = t("editLot.loadFailed")
  } finally {
    isLoading.value = false
  }
}

async function uploadPhotoForEntry(photoId: string, file: File) {
  const photo = photos.value.find((item) => item.id === photoId)
  if (!photo) {
    return
  }

  photo.uploadStatus = "uploading"
  photo.errorMessage = undefined

  try {
    const response = await mediaService.uploadPhoto(file)
    photo.key = response.key
    photo.uploadStatus = "uploaded"
    photo.isExisting = false
  } catch (error) {
    console.error("Failed to upload photo", error)
    photo.uploadStatus = "failed"
    photo.errorMessage = t("sell.uploadFailed", { fileName: file.name })
    errorMessage.value = photo.errorMessage
  } finally {
    syncPhotoState()
  }
}

async function handleFilesSelected(files: File[]) {
  errorMessage.value = ""
  const availableSlots = MAX_PHOTOS - photos.value.length

  if (availableSlots <= 0) {
    errorMessage.value = t("sell.maxPhotosError", { count: MAX_PHOTOS })
    return
  }

  const selectedFiles = Array.from(files).slice(0, availableSlots)
  if (files.length > availableSlots) {
    errorMessage.value = t("sell.maxPhotosError", { count: MAX_PHOTOS })
  }

  isUploadingPhotos.value = true

  try {
    for (const file of selectedFiles) {
      const photoId = createClientId()
      photos.value.push({
        id: photoId,
        key: "",
        previewUrl: URL.createObjectURL(file),
        fileName: file.name,
        file,
        uploadStatus: "uploading",
        isCover: photos.value.length === 0,
        isExisting: false,
      })

      await uploadPhotoForEntry(photoId, file)
    }
  } finally {
    isUploadingPhotos.value = false
  }
}

function removePhoto(photoId: string) {
  const photo = photos.value.find((item) => item.id === photoId)
  if (photo?.previewUrl.startsWith("blob:")) {
    URL.revokeObjectURL(photo.previewUrl)
  }

  if (photo?.key && !photo.isExisting) {
    void mediaService.deletePhoto(photo.key)
  }

  photos.value = photos.value.filter((item) => item.id !== photoId)
  syncPhotoState()
}

async function replacePhoto(photoId: string, file: File) {
  const photo = photos.value.find((item) => item.id === photoId)
  if (!photo) {
    return
  }

  const previousKey = photo.key
  const previousPreviewUrl = photo.previewUrl
  const wasExisting = Boolean(photo.isExisting)
  const nextPreviewUrl = URL.createObjectURL(file)

  isUploadingPhotos.value = true

  try {
    photo.previewUrl = nextPreviewUrl
    photo.fileName = file.name
    photo.file = file
    photo.key = ""
    photo.uploadStatus = "uploading"

    const response = await mediaService.uploadPhoto(file)
    photo.key = response.key
    photo.uploadStatus = "uploaded"
    photo.errorMessage = undefined
    photo.isExisting = false

    if (previousPreviewUrl.startsWith("blob:")) {
      URL.revokeObjectURL(previousPreviewUrl)
    }

    if (previousKey && !wasExisting) {
      void mediaService.deletePhoto(previousKey)
    }
  } catch (error) {
    console.error("Failed to replace photo", error)
    URL.revokeObjectURL(nextPreviewUrl)
    photo.previewUrl = previousPreviewUrl
    photo.key = previousKey
    photo.uploadStatus = previousKey ? "uploaded" : "failed"
    photo.isExisting = wasExisting
    photo.errorMessage = t("sell.uploadFailed", { fileName: file.name })
    errorMessage.value = photo.errorMessage
  } finally {
    syncPhotoState()
    isUploadingPhotos.value = false
  }
}

async function retryPhoto(photoId: string) {
  const photo = photos.value.find((item) => item.id === photoId)

  if (!photo?.file) {
    return
  }

  isUploadingPhotos.value = true

  try {
    await uploadPhotoForEntry(photoId, photo.file)
  } finally {
    isUploadingPhotos.value = false
  }
}

function movePhoto(photoId: string, direction: "left" | "right") {
  const index = photos.value.findIndex((photo) => photo.id === photoId)
  const nextIndex = direction === "left" ? index - 1 : index + 1

  if (index < 0 || nextIndex < 0 || nextIndex >= photos.value.length) {
    return
  }

  const photo = photos.value[index]
  if (!photo) {
    return
  }

  photos.value.splice(index, 1)
  photos.value.splice(nextIndex, 0, photo)
  syncPhotoState()
}

function makeCoverPhoto(photoId: string) {
  photos.value.forEach((photo) => {
    photo.isCover = photo.id === photoId
  })

  syncPhotoState()
}

async function handleProofDocumentsSelected(documentTypeId: string, files: File[]) {
  errorMessage.value = ""

  if (!documentTypeId) {
    errorMessage.value = t("sell.proofTypeRequired")
    return
  }

  if (form.proofDocuments.length + files.length > 5) {
    errorMessage.value = t("sell.proofMaxDocuments")
    return
  }

  isUploadingProofDocuments.value = true

  try {
    for (const file of files) {
      const response = await mediaService.uploadProofDocument(file, documentTypeId)
      form.proofDocuments.push({
        id: createClientId(),
        uploadId: response.uploadId,
        documentTypeId: response.documentTypeId,
        originalFileName: response.originalFileName,
        mimeType: response.mimeType,
        sizeBytes: response.sizeBytes,
        createdAtUtc: response.createdAtUtc,
        isExisting: false,
      })
    }
  } catch (error) {
    console.error("Failed to upload proof document", error)
    errorMessage.value = t("sell.proofUploadFailed")
  } finally {
    isUploadingProofDocuments.value = false
  }
}

async function removeProofDocument(id: string) {
  const document = form.proofDocuments.find((item) => item.id === id)
  if (!document) {
    return
  }

  if (document.isExisting && lot.value) {
    try {
      await mediaService.deleteProofDocument(lot.value.id, document.id)
    } catch (error) {
      console.error("Failed to delete proof document", error)
      errorMessage.value = t("sell.proofDeleteFailed")
      return
    }
  }

  form.proofDocuments = form.proofDocuments.filter((item) => item.id !== id)
}

function validateVintage(): boolean {
  if (!form.isVintage) {
    return true
  }

  const currentYear = new Date().getFullYear()

  if (form.productionYear && currentYear - form.productionYear < VINTAGE_MIN_AGE_YEARS) {
    errorMessage.value = t("sell.vintageTooYoung", { count: VINTAGE_MIN_AGE_YEARS })
    return false
  }

  if (!form.productionYear && !form.vintageNotes.trim()) {
    errorMessage.value = t("sell.vintageNotesRequired")
    return false
  }

  return true
}

function validateYears(): boolean {
  const currentYear = new Date().getFullYear()
  const productionYear = form.productionYear
  const acquisitionYear = form.acquisitionYear

  if (
    typeof productionYear === "number" &&
    (!Number.isInteger(productionYear) || productionYear < MIN_PRODUCTION_YEAR || productionYear > currentYear)
  ) {
    errorMessage.value = t("sell.productionYearRange", {
      min: MIN_PRODUCTION_YEAR,
      max: currentYear,
    })
    return false
  }

  if (
    typeof acquisitionYear === "number" &&
    (!Number.isInteger(acquisitionYear) || acquisitionYear < MIN_ACQUISITION_YEAR || acquisitionYear > currentYear)
  ) {
    errorMessage.value = t("sell.acquisitionYearRange", {
      min: MIN_ACQUISITION_YEAR,
      max: currentYear,
    })
    return false
  }

  if (
    typeof productionYear === "number" &&
    typeof acquisitionYear === "number" &&
    acquisitionYear < productionYear
  ) {
    errorMessage.value = t("sell.acquisitionBeforeProduction")
    return false
  }

  return true
}

function validateCurrentStep(): boolean {
  errorMessage.value = ""

  switch (currentStep.value) {
    case "category":
      if (!form.department) {
        errorMessage.value = t("sell.departmentRequired")
        return false
      }

      if (!form.rootCategoryId) {
        errorMessage.value = t("sell.rootCategoryRequired")
        return false
      }

      if (!form.categoryId) {
        errorMessage.value = t("sell.subcategoryRequired")
        return false
      }

      return true

    case "details": {
      if (!form.title.trim()) {
        errorMessage.value = t("sell.titleRequired")
        return false
      }

      if (!form.brand.trim()) {
        errorMessage.value = t("sell.brandRequired")
        return false
      }

      if (form.condition === null) {
        errorMessage.value = t("sell.conditionRequired")
        return false
      }

      if (form.size === null) {
        errorMessage.value = t("sell.sizeRequired")
        return false
      }

      if (!form.primaryColorId) {
        errorMessage.value = t("sell.colorRequired")
        return false
      }

      const selectedMaterialIds = form.materials.map((material) => material.materialId).filter(Boolean)
      if (selectedMaterialIds.length > 3) {
        errorMessage.value = t("sell.materialsLimit")
        return false
      }

      if (new Set(selectedMaterialIds).size !== selectedMaterialIds.length) {
        errorMessage.value = t("sell.materialDuplicate")
        return false
      }

      if (!form.description.trim()) {
        errorMessage.value = t("sell.descriptionRequired")
        return false
      }

      if (form.description.trim().length < MIN_DESCRIPTION_LENGTH) {
        errorMessage.value = t("sell.descriptionTooShort", { count: MIN_DESCRIPTION_LENGTH })
        return false
      }

      if (!form.country.trim()) {
        errorMessage.value = t("sell.countryRequired")
        return false
      }

      if (!validateYears()) {
        return false
      }

      return validateVintage()
    }

    case "measurements":
      if (form.measurements.some((measurement) => measurement.value <= 0)) {
        errorMessage.value = t("sell.measurementsPositive")
        return false
      }

      return true

    case "photos":
      syncPhotoState()

      if (uploadedPhotoKeys.value.length < MIN_PHOTOS) {
        errorMessage.value = t("sell.uploadMinimumPhotos", { count: MIN_PHOTOS })
        return false
      }

      if (uploadedPhotoKeys.value.length > MAX_PHOTOS) {
        errorMessage.value = t("sell.maxPhotosError", { count: MAX_PHOTOS })
        return false
      }

      if (isUploadingPhotos.value || isUploadingProofDocuments.value) {
        errorMessage.value = t("sell.waitUploads")
        return false
      }

      if (photos.value.some((photo) => !photo.key || photo.uploadStatus === "failed")) {
        errorMessage.value = t("sell.photosPending")
        return false
      }

      return true

    case "pricing":
      if (form.amount === null || Number.isNaN(form.amount) || form.amount <= 0) {
        errorMessage.value = t("sell.amountPositive")
        return false
      }

      if (!form.currency.trim()) {
        errorMessage.value = t("sell.currencyRequired")
        return false
      }

      if (form.shippingPrice === null || Number.isNaN(form.shippingPrice) || form.shippingPrice < 0) {
        errorMessage.value = t("sell.shippingPricePositive")
        return false
      }

      if (!form.shippingCurrency.trim()) {
        errorMessage.value = t("sell.shippingCurrencyRequired")
        return false
      }

      if (!form.shippingOriginCountry.trim()) {
        errorMessage.value = t("sell.shippingOriginRequired")
        return false
      }

      if (!form.shipsToCountries.trim()) {
        errorMessage.value = t("sell.shippingDestinationRequired")
        return false
      }

      if (form.shippingHandlingDays < 1 || form.shippingHandlingDays > 30) {
        errorMessage.value = t("sell.handlingRange")
        return false
      }

      return true

    default:
      return true
  }
}

function validateReadyToSubmit() {
  const previousStepIndex = currentStepIndex.value
  const isValid = EDIT_STEPS
    .filter((step) => step !== "review")
    .every((step) => {
      currentStepIndex.value = EDIT_STEPS.indexOf(step)
      return validateCurrentStep()
    })

  if (isValid) {
    currentStepIndex.value = previousStepIndex
  }

  return isValid
}

function goToPreviousStep() {
  errorMessage.value = ""

  if (currentStepIndex.value > 0) {
    currentStepIndex.value -= 1
  }
}

function goToNextStep() {
  successMessage.value = ""

  if (!validateCurrentStep()) {
    return
  }

  if (currentStepIndex.value < EDIT_STEPS.length - 1) {
    currentStepIndex.value += 1
  }
}

function goToStep(index: number) {
  if (index <= currentStepIndex.value) {
    currentStepIndex.value = index
    return
  }

  if (validateCurrentStep()) {
    currentStepIndex.value = index
  }
}

function buildPayload(): EditLotPayload {
  const root = selectedRootCategory.value
  const selectedColor = lookups.value?.colors.find((color) => color.id === form.primaryColorId)
  const gender = departmentToLegacyGender(form.department)

  if (
    !form.department ||
    !gender ||
    !form.categoryId ||
    !form.primaryColorId ||
    form.amount === null ||
    form.size === null ||
    form.condition === null ||
    form.shippingPrice === null
  ) {
    throw new Error("Listing is missing required fields.")
  }

  return {
    title: form.title.trim(),
    description: form.description.trim(),
    amount: form.amount,
    currency: form.currency.trim(),
    category: mapRootSlugToLegacyCategory(root?.slug),
    gender,
    size: form.size,
    brand: form.brand.trim(),
    condition: form.condition,
    color: selectedColor?.code ?? form.color.trim(),
    country: form.country.trim(),
    city: form.city.trim(),
    categoryId: form.categoryId,
    department: gender,
    primaryColorId: form.primaryColorId,
    modelName: form.modelName.trim() || null,
    acquisitionYear: form.acquisitionYear,
    productionYear: form.productionYear,
    isVintage: form.isVintage,
    vintageNotes: form.vintageNotes.trim() || null,
    age: form.age.trim() || String(form.productionYear ?? form.acquisitionYear ?? "Unknown"),
    style: form.style.trim() || root?.slug || "marketplace",
    shippingPrice: form.shippingPrice,
    shippingCurrency: form.shippingCurrency.trim(),
    shippingOriginCountry: form.shippingOriginCountry.trim(),
    shipsToCountries: form.shipsToCountries.trim(),
    shippingHandlingDays: form.shippingHandlingDays,
    photoKeys: uploadedPhotoKeys.value,
    coverPhotoKey: form.coverPhotoKey || uploadedPhotoKeys.value[0] || null,
    materials: form.materials
      .filter((material): material is LotMaterialInput => Boolean(material.materialId))
      .map((material) => ({
        materialId: material.materialId,
        percentage: material.percentage,
        otherName: material.otherName?.trim() || null,
      })),
    measurements: form.measurements
      .filter((measurement): measurement is LotMeasurementInput =>
        Boolean(measurement.key && measurement.value > 0),
      )
      .map((measurement) => ({
        key: measurement.key,
        value: measurement.value,
        unit: measurement.unit || "cm",
      })),
    proofDocuments: form.proofDocuments
      .filter((document) => !document.isExisting && document.uploadId)
      .map((document) => ({
        uploadId: document.uploadId,
      })),
  }
}

async function submit() {
  if (!lot.value) return

  errorMessage.value = ""
  successMessage.value = ""

  if (!canEdit.value) {
    errorMessage.value = t("editLot.cannotEdit")
    return
  }

  if (!user.value?.userId) {
    errorMessage.value = t("editLot.signInRequired")
    return
  }

  if (!validateReadyToSubmit()) {
    return
  }

  try {
    isSubmitting.value = true
    await itemService.updateLot(lot.value.id, buildPayload())
    successMessage.value = t("editLot.updated")
    toast.success(t("editLot.updateToast"), { position: "bottom-right" })
    await loadLot(false)
  } catch (error: unknown) {
    errorMessage.value = extractApiErrorMessage(error) ?? t("editLot.updateFailed")
    toast.error(errorMessage.value)
  } finally {
    isSubmitting.value = false
  }
}

async function cleanupPendingPhotoUploads() {
  const pendingKeys = photos.value
    .filter((photo) => !photo.isExisting && photo.key)
    .map((photo) => photo.key)

  await Promise.allSettled(pendingKeys.map((key) => mediaService.deletePhoto(key)))
}

async function discardChanges() {
  await cleanupPendingPhotoUploads()
  await loadLot(false)
  successMessage.value = t("editLot.discarded")
}

function cancelEdit() {
  router.push(localePath("/listings"))
}

function handleBeforeUnload(event: BeforeUnloadEvent) {
  if (!isDirty.value) {
    return
  }

  event.preventDefault()
  event.returnValue = ""
}

watch(
  () => form.department,
  () => {
    form.gender = departmentToLegacyGender(form.department)
  },
)

watch(
  () => form.rootCategoryId,
  () => {
    if (form.categoryId && !subcategories.value.some((category) => category.id === form.categoryId)) {
      form.categoryId = ""
    }
  },
)

watch(
  measurementDefinitions,
  (definitions) => {
    const allowedKeys = new Set(definitions.map((definition) => definition.key))
    form.measurements = form.measurements.filter((measurement) => allowedKeys.has(measurement.key))
  },
)

onMounted(() => {
  void loadLot()

  if (typeof window !== "undefined") {
    window.addEventListener("beforeunload", handleBeforeUnload)
  }
})

onBeforeUnmount(() => {
  if (typeof window !== "undefined") {
    window.removeEventListener("beforeunload", handleBeforeUnload)
  }
})
</script>

<template>
  <div class="min-h-screen bg-background text-foreground">
    <div class="mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="mb-8 flex flex-wrap items-start justify-between gap-4">
        <div>
          <p class="text-xs uppercase tracking-[0.22em] text-foreground/50">
            {{ $t("editLot.sellerDashboard") }}
          </p>

          <h1 class="mt-2 text-3xl font-semibold tracking-tight sm:text-4xl">
            {{ $t("editLot.title") }}
          </h1>

          <p class="mt-3 max-w-2xl text-sm leading-6 text-foreground/70 sm:text-base">
            {{ $t("editLot.description") }}
          </p>
        </div>

        <div class="min-w-[220px] rounded-[24px] border bg-background p-4">
          <p class="text-xs uppercase tracking-[0.16em] text-foreground/50">
            {{ $t("sell.progress") }}
          </p>

          <p class="mt-2 text-sm font-medium">
            {{ $t("sell.stepOf", { step: visibleStepNumber, total: EDIT_STEPS.length }) }}
          </p>

          <div class="mt-3 h-2 overflow-hidden rounded-full bg-foreground/10">
            <div
              class="h-full rounded-full bg-foreground transition-all duration-300"
              :style="{ width: `${progressPercent}%` }"
            />
          </div>
        </div>
      </div>

      <div
        v-if="isLoading"
        class="rounded-[28px] border bg-background p-8 text-sm text-foreground/60"
      >
        {{ $t("lot.loadingListing") }}
      </div>

      <div
        v-else
        class="space-y-6"
      >
        <div
          v-if="errorMessage"
          class="rounded-[20px] border border-red-500/20 bg-red-500/10 px-4 py-3 text-sm text-red-300"
        >
          {{ errorMessage }}
        </div>

        <div
          v-if="successMessage"
          class="rounded-[20px] border border-emerald-500/20 bg-emerald-500/10 px-4 py-3 text-sm text-emerald-300"
        >
          {{ successMessage }}
        </div>

        <div
          v-if="!canEdit"
          class="rounded-[24px] border bg-foreground/[0.03] p-5 text-sm text-foreground/70"
        >
          {{ $t("editLot.statusCannotEdit", { status: localizedStatus }) }}
        </div>

        <nav class="flex gap-2 overflow-x-auto pb-2">
          <button
            v-for="(step, index) in EDIT_STEPS"
            :key="step"
            type="button"
            class="whitespace-nowrap rounded-full border px-4 py-2 text-xs font-medium transition"
            :class="index === currentStepIndex ? 'bg-foreground text-background' : index < currentStepIndex ? 'bg-foreground/5' : 'text-foreground/55'"
            @click="goToStep(index)"
          >
            {{ $t(`editLot.steps.${step}`) }}
          </button>
        </nav>

        <fieldset
          :disabled="!canEdit || isSubmitting"
          class="min-w-0"
        >
          <SellCategoryStep
            v-if="currentStep === 'category'"
            :form="form"
            :departments="lookups?.departments ?? []"
            :root-categories="rootCategories"
            :subcategories="subcategories"
            :is-loading="isLoading"
          />

          <SellDetailsStep
            v-else-if="currentStep === 'details'"
            :form="form"
            :condition-options="CONDITION_OPTIONS"
            :size-options="SIZE_OPTIONS"
            :colors="lookups?.colors ?? []"
            :materials="lookups?.materials ?? []"
          />

          <SellMeasurementsStep
            v-else-if="currentStep === 'measurements'"
            :form="form"
            :measurement-definitions="measurementDefinitions"
          />

          <SellPhotosStep
            v-else-if="currentStep === 'photos'"
            :photos="photos"
            :proof-documents="form.proofDocuments"
            :proof-document-types="lookups?.proofDocumentTypes ?? []"
            :is-uploading="isUploadingPhotos"
            :is-uploading-proof-documents="isUploadingProofDocuments"
            @files-selected="handleFilesSelected"
            @proof-documents-selected="handleProofDocumentsSelected"
            @remove-proof-document="removeProofDocument"
            @remove-photo="removePhoto"
            @replace-photo="replacePhoto"
            @retry-photo="retryPhoto"
            @move-photo="movePhoto"
            @make-cover="makeCoverPhoto"
          />

          <SellPricingStep
            v-else-if="currentStep === 'pricing'"
            :form="form"
            :currency-options="['EUR']"
          />

          <SellReviewStep
            v-else-if="currentStep === 'review'"
            :form="reviewForm"
            :photos="photos"
          />
        </fieldset>

        <div class="sticky bottom-0 z-10 -mx-4 border-t bg-background/95 px-4 py-4 backdrop-blur sm:static sm:mx-0 sm:border-0 sm:bg-transparent sm:px-0 sm:py-0">
          <div class="flex flex-wrap items-center justify-between gap-3">
            <div class="flex flex-wrap gap-3">
              <button
                type="button"
                class="inline-flex items-center justify-center rounded-2xl border px-5 py-3 text-sm font-medium transition hover:bg-foreground hover:text-background disabled:cursor-not-allowed disabled:opacity-50"
                :disabled="isFirstStep || isSubmitting || isUploadingPhotos || isUploadingProofDocuments"
                @click="goToPreviousStep"
              >
                {{ $t("sell.back") }}
              </button>

              <button
                type="button"
                class="inline-flex items-center justify-center rounded-2xl border px-5 py-3 text-sm font-medium transition hover:bg-foreground hover:text-background disabled:cursor-not-allowed disabled:opacity-50"
                :disabled="isSubmitting || isUploadingPhotos || isUploadingProofDocuments"
                @click="cancelEdit"
              >
                {{ $t("editLot.cancel") }}
              </button>

              <button
                type="button"
                class="inline-flex items-center justify-center rounded-2xl border px-5 py-3 text-sm font-medium transition hover:bg-foreground hover:text-background disabled:cursor-not-allowed disabled:opacity-50"
                :disabled="!isDirty || isSubmitting || isUploadingPhotos || isUploadingProofDocuments"
                @click="discardChanges"
              >
                {{ $t("editLot.discardChanges") }}
              </button>
            </div>

            <button
              v-if="!isLastStep"
              type="button"
              class="inline-flex items-center justify-center rounded-2xl bg-foreground px-5 py-3 text-sm font-medium text-background transition hover:opacity-90 disabled:cursor-not-allowed disabled:opacity-50"
              :disabled="isSubmitting || isUploadingPhotos || isUploadingProofDocuments"
              @click="goToNextStep"
            >
              {{ isUploadingPhotos || isUploadingProofDocuments ? $t("sell.uploading") : $t("sell.continue") }}
            </button>

            <button
              v-else
              type="button"
              class="inline-flex items-center justify-center rounded-2xl bg-foreground px-5 py-3 text-sm font-medium text-background transition hover:opacity-90 disabled:cursor-not-allowed disabled:opacity-50"
              :disabled="!canEdit || isSubmitting || isUploadingPhotos || isUploadingProofDocuments"
              @click="submit"
            >
              {{ isSubmitting ? $t("editLot.savingChanges") : $t("editLot.saveChanges") }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
