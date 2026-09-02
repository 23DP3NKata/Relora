<script setup lang="ts">
import { computed } from "vue"
import CharacterCounter from "@/components/inputs/CharacterCouter.vue"
import type { NumericOption } from "@/features/lots/create-lot/options"
import type { CreateLotFormState } from "@/types/createLot"
import type { LookupOption } from "@/types/lotCatalog"

const MAX_MATERIALS = 3

const props = defineProps<{
  form: CreateLotFormState
  conditionOptions: NumericOption[]
  sizeOptions: NumericOption[]
  colors: LookupOption[]
  materials: LookupOption[]
}>()

const canAddMaterial = computed(() => props.form.materials.length < MAX_MATERIALS)

function selectedMaterialIdsExcept(index: number) {
  return new Set(
    props.form.materials
      .filter((_, materialIndex) => materialIndex !== index)
      .map((material) => material.materialId)
      .filter(Boolean),
  )
}

function addMaterial() {
  if (!canAddMaterial.value) {
    return
  }

  props.form.materials.push({
    materialId: "",
    percentage: null,
    otherName: null,
  })
}

function removeMaterial(index: number) {
  props.form.materials.splice(index, 1)
}
</script>

<template>
  <section class="rounded-[28px] border bg-background p-6 sm:p-8">
    <div>
      <p class="text-xs uppercase tracking-[0.22em] text-foreground/50">
        {{ $t("sell.stepOf", { step: 2, total: 6 }) }}
      </p>

      <h2 class="mt-3 text-2xl font-semibold tracking-tight sm:text-3xl">
        {{ $t("sell.itemDetailsTitle") }}
      </h2>

      <p class="mt-3 text-sm leading-6 text-foreground/70 sm:text-base">
        {{ $t("sell.itemDetailsDescription") }}
      </p>
    </div>

    <div class="mt-8 grid grid-cols-1 gap-6 md:grid-cols-2">
      <div class="md:col-span-2">
        <label class="block text-sm font-medium">{{ $t("sell.title") }}</label>
        <input
          v-model="form.title"
          type="text"
          :placeholder="$t('sell.titlePlaceholder')"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition placeholder:text-foreground/40 focus:ring-2 focus:ring-foreground/20"
        />
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("sell.brand") }}</label>
        <input
          v-model="form.brand"
          type="text"
          :placeholder="$t('sell.brandPlaceholder')"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition placeholder:text-foreground/40 focus:ring-2 focus:ring-foreground/20"
        />
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("sell.modelName") }}</label>
        <input
          v-model="form.modelName"
          type="text"
          :placeholder="$t('sell.modelNamePlaceholder')"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition placeholder:text-foreground/40 focus:ring-2 focus:ring-foreground/20"
        />
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("catalog.condition") }}</label>
        <select
          v-model.number="form.condition"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
        >
          <option :value="null" disabled>
            {{ $t("sell.selectCondition") }}
          </option>
          <option
            v-for="option in conditionOptions"
            :key="option.value"
            :value="option.value"
          >
            {{ $t(option.labelKey) }}
          </option>
        </select>
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("catalog.size") }}</label>
        <select
          v-model.number="form.size"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
        >
          <option :value="null" disabled>
            {{ $t("sell.selectSize") }}
          </option>
          <option
            v-for="size in sizeOptions"
            :key="size.value"
            :value="size.value"
          >
            {{ $t(size.labelKey) }}
          </option>
        </select>
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("sell.primaryColor") }}</label>
        <select
          v-model="form.primaryColorId"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
        >
          <option value="" disabled>
            {{ $t("sell.selectColor") }}
          </option>
          <option
            v-for="color in colors"
            :key="color.id"
            :value="color.id"
          >
            {{ $t(color.nameKey) }}
          </option>
        </select>
      </div>

      <div class="md:col-span-2">
        <div class="flex items-center justify-between gap-4">
          <label class="block text-sm font-medium">{{ $t("sell.materials") }}</label>
          <button
            type="button"
            class="rounded-xl border px-3 py-2 text-xs font-medium transition hover:bg-foreground hover:text-background disabled:cursor-not-allowed disabled:opacity-50"
            :disabled="!canAddMaterial"
            @click="addMaterial"
          >
            {{ $t("sell.addMaterial") }}
          </button>
        </div>

        <p class="mt-2 text-xs text-foreground/55">
          {{ $t("sell.materialsLimit") }}
        </p>

        <div v-if="form.materials.length === 0" class="mt-2 rounded-2xl border border-dashed p-4 text-sm text-foreground/60">
          {{ $t("sell.materialsOptional") }}
        </div>

        <div v-else class="mt-3 space-y-3">
          <div
            v-for="(material, index) in form.materials"
            :key="index"
            class="grid gap-3 rounded-2xl border p-3 md:grid-cols-[1fr_120px_auto]"
          >
            <select
              v-model="material.materialId"
              class="h-11 rounded-xl border bg-background px-3 text-sm outline-none focus:ring-2 focus:ring-foreground/20"
            >
              <option value="" disabled>
                {{ $t("sell.selectMaterial") }}
              </option>
              <option
                v-for="option in materials"
                :key="option.id"
                :value="option.id"
                :disabled="selectedMaterialIdsExcept(index).has(option.id)"
              >
                {{ $t(option.nameKey) }}
              </option>
            </select>

            <input
              v-model.number="material.percentage"
              type="number"
              min="0"
              max="100"
              step="1"
              class="h-11 rounded-xl border bg-background px-3 text-sm outline-none focus:ring-2 focus:ring-foreground/20"
              :placeholder="$t('sell.percentPlaceholder')"
            />

            <button
              type="button"
              class="rounded-xl border px-3 py-2 text-xs font-medium transition hover:bg-foreground hover:text-background"
              @click="removeMaterial(index)"
            >
              {{ $t("sell.remove") }}
            </button>
          </div>
        </div>
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("sell.productionYear") }}</label>
        <input
          v-model.number="form.productionYear"
          type="number"
          min="1800"
          :max="new Date().getFullYear()"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
        />
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("sell.acquisitionYear") }}</label>
        <input
          v-model.number="form.acquisitionYear"
          type="number"
          min="1900"
          :max="new Date().getFullYear()"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
        />
      </div>

      <div class="md:col-span-2 rounded-2xl border p-4">
        <label class="inline-flex items-center gap-3 text-sm font-medium">
          <input
            v-model="form.isVintage"
            type="checkbox"
            class="h-4 w-4 rounded border"
          />
          {{ $t("sell.markVintage") }}
        </label>

        <p class="mt-2 text-sm leading-6 text-foreground/60">
          {{ $t("sell.vintageHelp") }}
        </p>

        <textarea
          v-if="form.isVintage && !form.productionYear"
          v-model="form.vintageNotes"
          rows="3"
          class="mt-3 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
          :placeholder="$t('sell.vintageNotesPlaceholder')"
        />
      </div>

      <div class="md:col-span-2">
        <label class="block text-sm font-medium">{{ $t("sell.description") }}</label>
        <textarea
          v-model="form.description"
          rows="8"
          maxlength="2000"
          :placeholder="$t('sell.descriptionPlaceholder')"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition placeholder:text-foreground/40 focus:ring-2 focus:ring-foreground/20"
        />
        <CharacterCounter
          :model-value="form.description"
          :max="2000"
        />
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("catalog.country") }}</label>
        <input
          v-model="form.country"
          class="mt-2 h-12 w-full rounded-2xl border bg-background px-4 text-sm outline-none transition focus:border-foreground"
          :placeholder="$t('catalog.options.Latvia')"
        />
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("sell.city") }}</label>
        <input
          v-model="form.city"
          class="mt-2 h-12 w-full rounded-2xl border bg-background px-4 text-sm outline-none transition focus:border-foreground"
          :placeholder="$t('sell.cityPlaceholder')"
        />
      </div>
    </div>
  </section>
</template>
