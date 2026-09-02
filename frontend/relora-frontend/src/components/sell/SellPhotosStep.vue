<script setup lang="ts">
import { computed, ref } from "vue"
import { useI18n } from "vue-i18n"
import { compressAndConvertToWebp } from "@/utils/compressImage"
import { toast } from "vue-sonner"
import type { LookupOption, UploadedProofDocument } from "@/types/lotCatalog"
import type { UploadedPhoto } from "@/types/createLot"

const MIN_PHOTOS = 5
const MAX_PHOTOS = 15

const props = defineProps<{
  photos: UploadedPhoto[]
  proofDocuments?: UploadedProofDocument[]
  proofDocumentTypes?: LookupOption[]
  isUploading?: boolean
  isUploadingProofDocuments?: boolean
}>()

const emit = defineEmits<{
  (e: "files-selected", files: File[]): void
  (e: "remove-photo", photoId: string): void
  (e: "replace-photo", photoId: string, file: File): void
  (e: "retry-photo", photoId: string): void
  (e: "move-photo", photoId: string, direction: "left" | "right"): void
  (e: "make-cover", photoId: string): void
  (e: "proof-documents-selected", documentTypeId: string, files: File[]): void
  (e: "remove-proof-document", id: string): void
}>()

const selectedProofTypeId = ref("")
const { t } = useI18n()

const hasEnoughPhotos = computed(() => props.photos.length >= MIN_PHOTOS)
const canAddMorePhotos = computed(() => props.photos.length < MAX_PHOTOS)

const handleFileChange = async (event: Event) => {
  const input = event.target as HTMLInputElement
  const files = input.files

  if (!files || files.length === 0) return

  if (files.length + props.photos.length > MAX_PHOTOS) {
    toast.error(t("sell.maxPhotosError", { count: MAX_PHOTOS }), { position: "bottom-right" })
    return
  }

  const processedFiles: File[] = []

  for (const file of Array.from(files)) {
    const processed = await compressAndConvertToWebp(file)
    processedFiles.push(processed)
  }

  emit("files-selected", processedFiles)

  input.value = ""
}

const handleProofFileChange = (event: Event) => {
  const input = event.target as HTMLInputElement
  const files = input.files

  if (!files || files.length === 0) return

  emit("proof-documents-selected", selectedProofTypeId.value, Array.from(files))
  input.value = ""
}

const handleReplacePhotoChange = async (photoId: string, event: Event) => {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]

  if (!file) return

  const processed = await compressAndConvertToWebp(file)
  emit("replace-photo", photoId, processed)
  input.value = ""
}
</script>

<template>
  <section class="rounded-[28px] border bg-background p-6 sm:p-8">
    <div>
      <p class="text-xs uppercase tracking-[0.22em] text-foreground/50">
        {{ $t("sell.stepOf", { step: 1, total: 6 }) }}
      </p>

      <h2 class="mt-3 text-2xl font-semibold tracking-tight sm:text-3xl">
        {{ $t("sell.uploadPhotos") }}
      </h2>

      <p class="mt-3 text-sm leading-6 text-foreground/70 sm:text-base">
        {{ $t("sell.photosDescription") }}
      </p>

      <p
        class="mt-4 rounded-2xl border px-4 py-3 text-sm text-foreground/70"
      >
        {{ $t("sell.photosCount", { minimum: MIN_PHOTOS, maximum: MAX_PHOTOS, current: photos.length }) }}
      </p>
    </div>

    <label
      class="mt-8 flex min-h-[220px] cursor-pointer flex-col items-center justify-center rounded-[24px] border border-dashed px-6 py-10 text-center transition hover:bg-foreground/5"
    >
      <span class="text-sm font-medium">
        {{ isUploading ? $t("sell.uploading") : $t("sell.clickUpload") }}
      </span>

      <span class="mt-2 text-sm text-foreground/60">
        {{ $t("sell.imageFormats") }}
      </span>

      <span class="mt-2 text-xs text-foreground/50">
        {{ $t("sell.multiImages") }}
      </span>

      <input
        type="file"
        multiple
        accept="image/png,image/jpeg,image/webp"
        :disabled="!canAddMorePhotos || isUploading"
        class="hidden"
        @change="handleFileChange"
      />
    </label>

    <div v-if="photos.length > 0" class="mt-8">
      <div class="mb-4">
        <h3 class="text-sm font-medium">{{ $t("sell.uploadedPhotos") }}</h3>
        <p class="mt-1 text-sm text-foreground/60">
          {{ $t("sell.photosAdded", photos.length) }}
        </p>
      </div>

      <div class="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
        <article
          v-for="(photo, index) in photos"
          :key="photo.id"
          class="group overflow-hidden rounded-[24px] border bg-background"
        >
          <div class="relative aspect-[4/5] w-full overflow-hidden bg-foreground/5">
            <img
              :src="photo.previewUrl"
              :alt="photo.fileName"
              class="h-full w-full object-cover transition duration-300 group-hover:scale-[1.02]"
            />

            <span
              v-if="photo.isCover"
              class="absolute left-2 top-2 rounded-full bg-background/90 px-2.5 py-1 text-[11px] font-medium text-foreground shadow-sm"
            >
              {{ $t("sell.coverPhoto") }}
            </span>

            <span
              v-if="photo.uploadStatus === 'uploading'"
              class="absolute inset-x-2 bottom-2 rounded-full bg-background/90 px-2.5 py-1 text-center text-[11px] font-medium text-foreground shadow-sm"
            >
              {{ $t("sell.uploading") }}
            </span>
          </div>

          <div class="p-3">
            <p class="truncate text-xs text-foreground/60">
              {{ photo.fileName }}
            </p>

            <p
              v-if="photo.errorMessage"
              class="mt-2 text-xs text-red-400"
            >
              {{ photo.errorMessage }}
            </p>

            <div class="mt-3 grid grid-cols-2 gap-2">
              <button
                type="button"
                class="rounded-xl border px-2 py-2 text-xs font-medium transition hover:bg-foreground hover:text-background disabled:cursor-not-allowed disabled:opacity-50"
                :disabled="index === 0 || photo.uploadStatus === 'uploading'"
                @click="emit('move-photo', photo.id, 'left')"
              >
                {{ $t("sell.moveLeft") }}
              </button>

              <button
                type="button"
                class="rounded-xl border px-2 py-2 text-xs font-medium transition hover:bg-foreground hover:text-background disabled:cursor-not-allowed disabled:opacity-50"
                :disabled="index === photos.length - 1 || photo.uploadStatus === 'uploading'"
                @click="emit('move-photo', photo.id, 'right')"
              >
                {{ $t("sell.moveRight") }}
              </button>
            </div>

            <div class="mt-2 grid grid-cols-2 gap-2">
              <button
                type="button"
                class="rounded-xl border px-2 py-2 text-xs font-medium transition hover:bg-foreground hover:text-background disabled:cursor-not-allowed disabled:opacity-50"
                :disabled="photo.isCover || photo.uploadStatus === 'failed' || photo.uploadStatus === 'uploading'"
                @click="emit('make-cover', photo.id)"
              >
                {{ $t("sell.makeCover") }}
              </button>

              <label
                class="inline-flex cursor-pointer items-center justify-center rounded-xl border px-2 py-2 text-xs font-medium transition hover:bg-foreground hover:text-background"
              >
                {{ $t("sell.replace") }}
                <input
                  type="file"
                  accept="image/png,image/jpeg,image/webp"
                  :disabled="isUploading || photo.uploadStatus === 'uploading'"
                  class="hidden"
                  @change="handleReplacePhotoChange(photo.id, $event)"
                />
              </label>
            </div>

            <button
              v-if="photo.uploadStatus !== 'failed'"
              type="button"
              :disabled="photo.uploadStatus === 'uploading'"
              class="mt-2 inline-flex w-full items-center justify-center rounded-xl border px-3 py-2 text-xs font-medium transition hover:bg-foreground hover:text-background"
              @click="emit('remove-photo', photo.id)"
            >
              {{ $t("sell.remove") }}
            </button>

            <div
              v-else
              class="mt-2 grid grid-cols-2 gap-2"
            >
              <button
                type="button"
                class="rounded-xl border px-3 py-2 text-xs font-medium transition hover:bg-foreground hover:text-background"
                @click="emit('retry-photo', photo.id)"
              >
                {{ $t("sell.retryUpload") }}
              </button>

              <button
                type="button"
                class="rounded-xl border px-3 py-2 text-xs font-medium transition hover:bg-foreground hover:text-background"
                @click="emit('remove-photo', photo.id)"
              >
                {{ $t("sell.remove") }}
              </button>
            </div>
          </div>
        </article>
      </div>
    </div>

    <div v-else class="mt-8 rounded-[24px] border border-dashed p-5 text-sm text-foreground/60">
      {{ $t("sell.noPhotosYet") }}
    </div>

    <div class="mt-10 border-t pt-8">
      <div>
        <p class="text-xs uppercase tracking-[0.16em] text-foreground/50">
          {{ $t("sell.proofPrivateLabel") }}
        </p>
        <h3 class="mt-2 text-lg font-semibold tracking-tight">
          {{ $t("sell.proofTitle") }}
        </h3>
        <p class="mt-2 text-sm leading-6 text-foreground/65">
          {{ $t("sell.proofDescription") }}
        </p>
        <p class="mt-1 text-sm leading-6 text-foreground/55">
          {{ $t("sell.proofHint") }}
        </p>
      </div>

      <div class="mt-5 grid gap-4 md:grid-cols-[1fr_1.3fr]">
        <div>
          <label class="block text-sm font-medium">
            {{ $t("sell.proofType") }}
          </label>
          <select
            v-model="selectedProofTypeId"
            class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
          >
            <option value="" disabled>
              {{ $t("sell.selectProofType") }}
            </option>
            <option
              v-for="type in proofDocumentTypes ?? []"
              :key="type.id"
              :value="type.id"
            >
              {{ $t(type.nameKey) }}
            </option>
          </select>
        </div>

        <label class="flex min-h-[96px] cursor-pointer flex-col items-center justify-center rounded-[24px] border border-dashed px-5 py-6 text-center transition hover:bg-foreground/5">
          <span class="text-sm font-medium">
            {{ isUploadingProofDocuments ? $t("sell.uploading") : $t("sell.addProofDocument") }}
          </span>
          <span class="mt-2 text-xs text-foreground/50">
            {{ $t("sell.proofFormats") }}
          </span>
          <input
            type="file"
            multiple
            accept="image/png,image/jpeg,image/webp,application/pdf"
            :disabled="!selectedProofTypeId || isUploadingProofDocuments"
            class="hidden"
            @change="handleProofFileChange"
          />
        </label>
      </div>

      <div
        v-if="proofDocuments?.length"
        class="mt-5 space-y-2"
      >
        <article
          v-for="document in proofDocuments"
          :key="document.id"
          class="flex flex-wrap items-center justify-between gap-3 rounded-2xl border px-4 py-3 text-sm"
        >
          <div>
            <p class="font-medium">{{ document.originalFileName }}</p>
            <p class="mt-1 text-xs text-foreground/55">
              {{ document.mimeType }} - {{ Math.round(document.sizeBytes / 1024) }} KB
            </p>
          </div>
          <button
            type="button"
            class="rounded-xl border px-3 py-2 text-xs font-medium transition hover:bg-foreground hover:text-background"
            @click="emit('remove-proof-document', document.id)"
          >
            {{ $t("sell.remove") }}
          </button>
        </article>
      </div>
    </div>
  </section>
</template>
