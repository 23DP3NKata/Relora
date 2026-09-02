<script setup lang="ts">
import { computed } from "vue"
import { storeToRefs } from "pinia"
import { useI18n } from "vue-i18n"
import { useRoute, useRouter } from "vue-router"

import SellIntroStep from "@/components/sell/SellIntroStep.vue"
import SellPhotosStep from "@/components/sell/SellPhotosStep.vue"
import SellCategoryStep from "@/components/sell/SellCategoryStep.vue"
import SellMeasurementsStep from "@/components/sell/SellMeasurementsStep.vue"
import SellPricingStep from "@/components/sell/SellPricingStep.vue"
import SellDetailsStep from "@/components/sell/SellDetailsStep.vue"
import SellReviewStep from "@/components/sell/SellReviewStep.vue"
import {
  CONDITION_OPTIONS,
  getOptionLabel,
  SIZE_OPTIONS,
} from "@/features/lots/create-lot/options"
import { useCreateLotFlow } from "@/features/lots/create-lot/useCreateLotFlow"
import { useLocalePath } from "@/composables/useLocalePath"
import { useAuthStore } from "@/stores/authStore"

const { t, te } = useI18n()
const route = useRoute()
const router = useRouter()
const localePath = useLocalePath()
const authStore = useAuthStore()
const { isAuthenticated } = storeToRefs(authStore)

const {
  form,
  photos,
  currentStep,
  isFirstStep,
  isLastStep,
  visibleStepNumber,
  progressPercent,
  lookups,
  rootCategories,
  subcategories,
  selectedCategory,
  measurementDefinitions,
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
} = useCreateLotFlow()

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

function localizedOptionLabel(labelKey: string) {
  return labelKey && te(labelKey) ? t(labelKey) : labelKey
}

function startSelling() {
  if (!isAuthenticated.value) {
    void router.push(
      localePath({
        path: "/login",
        query: { redirect: route.fullPath },
      }),
    )
    return
  }

  handleStart()
}

function manageListings() {
  void router.push(localePath("/listings"))
}
</script>

<template>
  <div class="min-h-screen bg-background text-foreground">
    <div class="mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8">
      <div v-if="currentStep !== 'intro'" class="mb-8 flex flex-wrap items-start justify-between gap-4">
        <div>
          <p class="text-xs uppercase tracking-[0.22em] text-foreground/50">
            {{ $t("sell.sellOnRelora") }}
          </p>

          <h1 class="mt-2 text-3xl font-semibold tracking-tight sm:text-4xl">
            {{ $t("sell.createListing") }}
          </h1>

          <p class="mt-3 max-w-2xl text-sm leading-6 text-foreground/70 sm:text-base">
            {{ $t("sell.createListingDescription") }}
          </p>
        </div>

        <div
          class="min-w-[220px] rounded-[24px] border bg-background p-4"
        >
          <p class="text-xs uppercase tracking-[0.16em] text-foreground/50">
            {{ $t("sell.progress") }}
          </p>

          <p class="mt-2 text-sm font-medium">
            {{ $t("sell.stepOf", { step: visibleStepNumber, total: 6 }) }}
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
        v-if="currentStep !== 'intro' && errorMessage"
        class="mb-6 rounded-[20px] border border-red-500/20 bg-red-500/10 px-4 py-3 text-sm text-red-300"
      >
        {{ errorMessage }}
      </div>

      <div
        v-if="currentStep !== 'intro' && successMessage"
        class="mb-6 rounded-[20px] border border-emerald-500/20 bg-emerald-500/10 px-4 py-3 text-sm text-emerald-300"
      >
        {{ successMessage }}
      </div>

      <SellIntroStep
        v-if="currentStep === 'intro'"
        :is-authenticated="isAuthenticated"
        @start="startSelling"
        @manage="manageListings"
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

      <SellCategoryStep
        v-else-if="currentStep === 'category'"
        :form="form"
        :departments="lookups?.departments ?? []"
        :root-categories="rootCategories"
        :subcategories="subcategories"
        :is-loading="isLoadingLookups"
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

      <div
        v-if="currentStep !== 'intro'"
        class="mt-6 flex flex-wrap items-center justify-between gap-3"
      >
        <button
          type="button"
          class="inline-flex items-center justify-center rounded-2xl border px-5 py-3 text-sm font-medium transition hover:bg-foreground hover:text-background disabled:cursor-not-allowed disabled:opacity-50"
          :disabled="isFirstStep || isSubmitting || isUploadingPhotos || isUploadingProofDocuments"
          @click="goToPreviousStep"
        >
          {{ $t("sell.back") }}
        </button>

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
          :disabled="isSubmitting || isUploadingPhotos || isUploadingProofDocuments"
          @click="handleSubmit"
        >
          {{ isSubmitting ? $t("sell.submitting") : $t("sell.submitForReview") }}
        </button>
      </div>
    </div>
  </div>
</template>
