<script setup lang="ts">
import type { UploadedPhoto } from "@/types/createLot"

type ReviewForm = {
  title: string
  description: string
  amount: number | null
  currency: string
  category: string
  gender: string
  size: string
  brand: string
  condition: string
  color: string
  primaryColor: string
  materials: string[]
  measurementsCount: number
  proofCount: number
  shippingPrice: number | null
  shippingCurrency: string
  shippingOriginCountry: string
  shipsToCountries: string
  shippingHandlingDays: number
}

const props = defineProps<{
  form: ReviewForm
  photos: UploadedPhoto[]
}>()
</script>

<template>
  <section class="rounded-[28px] border bg-background p-6 sm:p-8">
    <div>
      <p class="text-xs uppercase tracking-[0.22em] text-foreground/50">
        {{ $t("sell.stepOf", { step: 6, total: 6 }) }}
      </p>

      <h2 class="mt-3 text-2xl font-semibold tracking-tight sm:text-3xl">
        {{ $t("sell.reviewListing") }}
      </h2>

      <p class="mt-3 text-sm leading-6 text-foreground/70 sm:text-base">
        {{ $t("sell.reviewDescription") }}
      </p>
    </div>

    <div class="mt-8 grid grid-cols-1 gap-8 lg:grid-cols-[1.1fr_0.9fr]">
      <div>
        <div
          v-if="photos.length > 0"
          class="grid grid-cols-2 gap-4 sm:grid-cols-3"
        >
          <article
            v-for="photo in photos"
            :key="photo.id"
            class="overflow-hidden rounded-[24px] border"
          >
            <div class="relative aspect-[4/5] w-full overflow-hidden bg-foreground/5">
              <img
                :src="photo.previewUrl"
                :alt="photo.fileName"
                class="h-full w-full object-cover"
              />
              <span
                v-if="photo.isCover"
                class="absolute left-2 top-2 rounded-full bg-background/90 px-2.5 py-1 text-[11px] font-medium text-foreground shadow-sm"
              >
                {{ $t("sell.coverPhoto") }}
              </span>
            </div>
          </article>
        </div>

        <div
          v-else
          class="rounded-[24px] border border-dashed p-5 text-sm text-foreground/60"
        >
          {{ $t("sell.noPhotos") }}
        </div>
      </div>

      <div class="space-y-4">
        <div class="rounded-[24px] border p-5">
          <p class="text-xs uppercase tracking-[0.16em] text-foreground/50">
            {{ $t("sell.item") }}
          </p>
          <h3 class="mt-2 text-xl font-semibold">
            {{ form.title || $t("sell.untitledItem") }}
          </h3>
          <p class="mt-2 text-sm text-foreground/70">
            {{ form.brand || $t("sell.noBrandSelected") }}
          </p>
        </div>

        <div class="rounded-[24px] border p-5">
          <p class="text-sm font-medium">{{ $t("sell.listingDetails") }}</p>

          <div class="mt-4 space-y-3 text-sm text-foreground/70">
            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("catalog.category") }}</span>
              <span class="text-right text-foreground">{{ form.category || "-" }}</span>
            </div>

            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("catalog.department") }}</span>
              <span class="text-right text-foreground">{{ props.form.gender || "-" }}</span>
            </div>

            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("catalog.size") }}</span>
              <span class="text-right text-foreground">{{ form.size || "-" }}</span>
            </div>

            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("catalog.condition") }}</span>
              <span class="text-right text-foreground">{{ form.condition || "-" }}</span>
            </div>

            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("sell.primaryColor") }}</span>
              <span class="text-right text-foreground">{{ form.primaryColor || form.color || "-" }}</span>
            </div>

            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("sell.materials") }}</span>
              <span class="text-right text-foreground">
                {{ form.materials.length ? form.materials.join(", ") : "-" }}
              </span>
            </div>

            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("sell.measurementsTitle") }}</span>
              <span class="text-right text-foreground">
                {{ form.measurementsCount }}
              </span>
            </div>

            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("sell.proofTitle") }}</span>
              <span class="text-right text-foreground">
                {{ form.proofCount }}
              </span>
            </div>

            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("sell.price") }}</span>
              <span class="text-right text-foreground">
                {{ form.amount ? `${form.amount} ${form.currency}` : "-" }}
              </span>
            </div>

            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("common.shipping") }}</span>
              <span class="text-right text-foreground">
                {{
                  form.shippingPrice !== null
                    ? `${form.shippingPrice} ${form.shippingCurrency}`
                    : "-"
                }}
              </span>
            </div>

            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("common.shipsFrom") }}</span>
              <span class="text-right text-foreground">
                {{ form.shippingOriginCountry || "-" }}
              </span>
            </div>

            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("common.shipsTo") }}</span>
              <span class="text-right text-foreground">
                {{ form.shipsToCountries || "-" }}
              </span>
            </div>

            <div class="flex items-start justify-between gap-4">
              <span>{{ $t("sell.dispatchTime") }}</span>
              <span class="text-right text-foreground">
                {{ $t("sell.businessDays", { count: form.shippingHandlingDays }) }}
              </span>
            </div>
          </div>
        </div>

        <div class="rounded-[24px] border p-5">
          <p class="text-sm font-medium">{{ $t("sell.description") }}</p>
          <p class="mt-3 whitespace-pre-line text-sm leading-6 text-foreground/70">
            {{ form.description || $t("sell.noDescription") }}
          </p>
        </div>
      </div>
    </div>
  </section>
</template>
