import { computed, onBeforeUnmount, onMounted, reactive, ref, watch } from "vue"
import { useRouter } from "vue-router"
import { useI18n } from "vue-i18n"

import { lookupService } from "@/app/services/lookupService"
import { mediaService } from "@/app/services/mediaService"
import { itemService } from "@/app/services/lotService"
import { useLocalePath } from "@/composables/useLocalePath"
import {
  DEFAULT_CREATE_LOT_FORM,
  SELL_STEPS,
  type CreateLotFormState,
  type CreateLotPayload,
  type SellStepKey,
  type UploadedPhoto,
} from "@/types/createLot"
import type {
  CategoryNode,
  LotDepartment,
  LotFormLookups,
  LotMaterialInput,
  LotMeasurementInput,
  UploadedProofDocument,
} from "@/types/lotCatalog"

const MIN_PHOTOS = 5
const MAX_PHOTOS = 15
const MIN_DESCRIPTION_LENGTH = 100
const VINTAGE_MIN_AGE_YEARS = 15
const MIN_PRODUCTION_YEAR = 1800
const MIN_ACQUISITION_YEAR = 1900

type ApiError = {
  response?: {
    data?: {
      message?: string
      errors?: Record<string, string | string[]>
    }
  }
}

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

function departmentToLegacyGender(department: LotDepartment | number | null): number | null {
  if (department === 1 || department === 2 || department === 3) return department
  if (department === "Women") return 1
  if (department === "Men") return 2
  if (department === "Unisex") return 3
  return null
}

function rootSlugToLegacyCategory(slug?: string): number {
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

export const useCreateLotFlow = () => {
  const router = useRouter()
  const localePath = useLocalePath()
  const { t } = useI18n()
  const currentStepIndex = ref(0)
  const form = reactive<CreateLotFormState>({
    ...DEFAULT_CREATE_LOT_FORM,
    materials: [...DEFAULT_CREATE_LOT_FORM.materials],
    measurements: [...DEFAULT_CREATE_LOT_FORM.measurements],
    proofDocuments: [...DEFAULT_CREATE_LOT_FORM.proofDocuments],
    photoKeys: [...DEFAULT_CREATE_LOT_FORM.photoKeys],
    coverPhotoKey: DEFAULT_CREATE_LOT_FORM.coverPhotoKey,
  })
  const photos = ref<UploadedPhoto[]>([])
  const lookups = ref<LotFormLookups | null>(null)
  const isLoadingLookups = ref(false)
  const isSubmitting = ref(false)
  const isUploadingPhotos = ref(false)
  const isUploadingProofDocuments = ref(false)
  const errorMessage = ref("")
  const successMessage = ref("")
  let lotWasCreated = false

  const currentStep = computed<SellStepKey>(() => SELL_STEPS[currentStepIndex.value] ?? "intro")
  const isFirstStep = computed(() => currentStepIndex.value === 0)
  const isLastStep = computed(() => currentStepIndex.value === SELL_STEPS.length - 1)
  const visibleStepNumber = computed(() => (currentStep.value === "intro" ? 0 : currentStepIndex.value))
  const progressPercent = computed(() => {
    if (currentStep.value === "intro") {
      return 0
    }

    return Math.round((currentStepIndex.value / (SELL_STEPS.length - 1)) * 100)
  })
  const uploadedPhotoKeys = computed(() =>
    photos.value
      .filter((photo) => photo.uploadStatus !== "failed")
      .map((photo) => photo.key)
      .filter((key): key is string => Boolean(key)),
  )
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
    if (!profile || profile === "none") {
      return []
    }

    return (lookups.value?.measurementDefinitions ?? [])
      .filter((definition) => definition.profile === profile)
      .sort((left, right) => left.sortOrder - right.sortOrder)
  })

  const clearMessages = () => {
    errorMessage.value = ""
    successMessage.value = ""
  }

  const loadLookups = async () => {
    isLoadingLookups.value = true

    try {
      lookups.value = await lookupService.getLotFormLookups()
    } catch (error) {
      console.error("Failed to load lot form lookups", error)
      errorMessage.value = t("sell.lookupsFailed")
    } finally {
      isLoadingLookups.value = false
    }
  }

  const handleStart = () => {
    clearMessages()
    currentStepIndex.value = 1
  }

  const syncPhotoState = () => {
    const uploadedPhotos = photos.value.filter((photo) => photo.key && photo.uploadStatus !== "failed")
    const coverPhoto = uploadedPhotos.find((photo) => photo.isCover) ?? uploadedPhotos[0] ?? null

    photos.value.forEach((photo) => {
      photo.isCover = Boolean(coverPhoto && photo.id === coverPhoto.id)
    })

    form.photoKeys = uploadedPhotos.map((photo) => photo.key)
    form.coverPhotoKey = coverPhoto?.key ?? ""
  }

  const uploadPhotoForEntry = async (photoId: string, file: File) => {
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
    } catch (error) {
      console.error("Failed to upload photo", error)
      photo.uploadStatus = "failed"
      photo.errorMessage = t("sell.uploadFailed", { fileName: file.name })
      errorMessage.value = photo.errorMessage
    } finally {
      syncPhotoState()
    }
  }

  const handleFilesSelected = async (files: File[]) => {
    clearMessages()

    const availableSlots = MAX_PHOTOS - photos.value.length

    if (availableSlots <= 0) {
      errorMessage.value = t("sell.maxPhotosError", { count: MAX_PHOTOS })
      return
    }

    const selectedFiles = Array.from(files).slice(0, availableSlots)

    if (files.length > availableSlots) {
      errorMessage.value = t("sell.maxPhotosError", { count: MAX_PHOTOS })
    }

    if (selectedFiles.length === 0) {
      return
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
        })

        await uploadPhotoForEntry(photoId, file)
      }
    } finally {
      isUploadingPhotos.value = false
    }
  }

  const handleProofDocumentsSelected = async (documentTypeId: string, files: File[]) => {
    clearMessages()

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
        })
      }
    } catch (error) {
      console.error("Failed to upload proof document", error)
      errorMessage.value = t("sell.proofUploadFailed")
    } finally {
      isUploadingProofDocuments.value = false
    }
  }

  const removeProofDocument = (id: string) => {
    form.proofDocuments = form.proofDocuments.filter((document) => document.id !== id)
  }

  const removePhoto = (photoId: string) => {
    clearMessages()

    const photoToRemove = photos.value.find((photo) => photo.id === photoId)

    if (photoToRemove) {
      if (photoToRemove.previewUrl.startsWith("blob:")) {
        URL.revokeObjectURL(photoToRemove.previewUrl)
      }

      if (photoToRemove.key && !photoToRemove.isExisting) {
        void mediaService.deletePhoto(photoToRemove.key)
      }
    }

    photos.value = photos.value.filter((photo) => photo.id !== photoId)
    syncPhotoState()
  }

  const replacePhoto = async (photoId: string, file: File) => {
    clearMessages()

    const photo = photos.value.find((item) => item.id === photoId)
    if (!photo) {
      return
    }

    const previousKey = photo.key
    const previousPreviewUrl = photo.previewUrl
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

      if (previousKey) {
        void mediaService.deletePhoto(previousKey)
      }
    } catch (error) {
      console.error("Failed to replace photo", error)
      URL.revokeObjectURL(nextPreviewUrl)
      photo.previewUrl = previousPreviewUrl
      photo.key = previousKey
      photo.uploadStatus = previousKey ? "uploaded" : "failed"
      photo.errorMessage = t("sell.uploadFailed", { fileName: file.name })
      errorMessage.value = photo.errorMessage
    } finally {
      syncPhotoState()
      isUploadingPhotos.value = false
    }
  }

  const retryPhoto = async (photoId: string) => {
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

  const movePhoto = (photoId: string, direction: "left" | "right") => {
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

  const makeCoverPhoto = (photoId: string) => {
    photos.value.forEach((photo) => {
      photo.isCover = photo.id === photoId
    })

    syncPhotoState()
  }

  const validateVintage = (): boolean => {
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

  const validateYears = (): boolean => {
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

  const validateCurrentStep = (): boolean => {
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

      case "details":
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

        const selectedMaterialIds = form.materials
          .map((material) => material.materialId)
          .filter(Boolean)

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

        if (!validateVintage()) {
          return false
        }

        return true

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
        if (form.amount === null || Number.isNaN(form.amount)) {
          errorMessage.value = t("sell.amountRequired")
          return false
        }

        if (form.amount <= 0) {
          errorMessage.value = t("sell.amountPositive")
          return false
        }

        if (!form.currency.trim()) {
          errorMessage.value = t("sell.currencyRequired")
          return false
        }

        if (form.shippingPrice === null || Number.isNaN(form.shippingPrice)) {
          errorMessage.value = t("sell.shippingPriceRequired")
          return false
        }

        if (form.shippingPrice < 0) {
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

  const goToNextStep = () => {
    successMessage.value = ""

    if (!validateCurrentStep()) {
      return
    }

    if (currentStepIndex.value < SELL_STEPS.length - 1) {
      currentStepIndex.value += 1
    }
  }

  const goToPreviousStep = () => {
    clearMessages()

    if (currentStepIndex.value > 0) {
      currentStepIndex.value -= 1
    }
  }

  const validateReadyToSubmit = (): boolean => {
    return SELL_STEPS
      .filter((step) => step !== "intro" && step !== "review")
      .every((step) => {
        currentStepIndex.value = SELL_STEPS.indexOf(step)
        return validateCurrentStep()
      })
  }

  const extractApiErrorMessage = (error: unknown): string | null => {
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

    return typeof message === "string" && message.trim()
      ? message
      : null
  }

  const buildPayload = (): CreateLotPayload => {
    const root = selectedRootCategory.value
    const primaryColor = lookups.value?.colors.find((color) => color.id === form.primaryColorId)
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
      category: rootSlugToLegacyCategory(root?.slug),
      gender,
      size: form.size,
      brand: form.brand.trim(),
      condition: form.condition,
      color: primaryColor?.code ?? form.color.trim(),
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
      proofDocuments: form.proofDocuments.map((document: UploadedProofDocument) => ({
        uploadId: document.uploadId,
      })),
    }
  }

  const handleSubmit = async () => {
    clearMessages()

    const previousStepIndex = currentStepIndex.value

    if (!validateReadyToSubmit()) {
      return
    }

    currentStepIndex.value = previousStepIndex
    isSubmitting.value = true

    try {
      const payload = buildPayload()
      const lotId = await itemService.createLot(payload)
      lotWasCreated = true
      successMessage.value = t("sell.createSuccess", { lotId })
      router.push(localePath("/listings"))
    } catch (error: unknown) {
      console.error("Failed to create listing", error)
      errorMessage.value = extractApiErrorMessage(error) ?? t("sell.submitFailed")
    } finally {
      isSubmitting.value = false
    }
  }

  watch(
    () => form.department,
    () => {
      form.gender = departmentToLegacyGender(form.department)

      if (form.rootCategoryId && !rootCategories.value.some((category) => category.id === form.rootCategoryId)) {
        form.rootCategoryId = ""
        form.categoryId = ""
      }
    },
  )

  watch(
    () => form.rootCategoryId,
    () => {
      if (!subcategories.value.some((category) => category.id === form.categoryId)) {
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

  onMounted(loadLookups)

  onBeforeUnmount(() => {
    photos.value.forEach((photo) => {
      if (photo.previewUrl.startsWith("blob:")) {
        URL.revokeObjectURL(photo.previewUrl)
      }
    })

    if (!lotWasCreated) {
      void Promise.allSettled(
        uploadedPhotoKeys.value.map((key) => mediaService.deletePhoto(key)),
      )
    }
  })

  return {
    form,
    photos,
    lookups,
    rootCategories,
    subcategories,
    selectedCategory,
    measurementDefinitions,
    currentStep,
    isFirstStep,
    isLastStep,
    visibleStepNumber,
    progressPercent,
    isLoadingLookups,
    isSubmitting,
    isUploadingPhotos,
    isUploadingProofDocuments,
    errorMessage,
    successMessage,
    handleStart,
    handleFilesSelected,
    handleProofDocumentsSelected,
    removeProofDocument,
    removePhoto,
    replacePhoto,
    retryPhoto,
    movePhoto,
    makeCoverPhoto,
    goToNextStep,
    goToPreviousStep,
    handleSubmit,
  }
}
