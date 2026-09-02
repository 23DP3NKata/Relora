<script setup lang="ts">
import type { CreateLotFormState } from "@/types/createLot"
import type { CategoryNode, LotDepartment } from "@/types/lotCatalog"

defineProps<{
  form: CreateLotFormState
  departments: LotDepartment[]
  rootCategories: CategoryNode[]
  subcategories: CategoryNode[]
  isLoading?: boolean
}>()
</script>

<template>
  <section class="rounded-[28px] border bg-background p-6 sm:p-8">
    <div>
      <p class="text-xs uppercase tracking-[0.22em] text-foreground/50">
        {{ $t("sell.stepOf", { step: 1, total: 6 }) }}
      </p>

      <h2 class="mt-3 text-2xl font-semibold tracking-tight sm:text-3xl">
        {{ $t("sell.categoryStepTitle") }}
      </h2>

      <p class="mt-3 text-sm leading-6 text-foreground/70 sm:text-base">
        {{ $t("sell.categoryStepDescription") }}
      </p>
    </div>

    <div v-if="isLoading" class="mt-8 rounded-2xl border p-5 text-sm text-foreground/60">
      {{ $t("sell.loadingLookups") }}
    </div>

    <div v-else class="mt-8 grid grid-cols-1 gap-6 md:grid-cols-3">
      <div>
        <label class="block text-sm font-medium" for="department">
          {{ $t("catalog.department") }}
        </label>
        <select
          id="department"
          v-model="form.department"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
        >
          <option :value="null" disabled>
            {{ $t("sell.selectDepartment") }}
          </option>
          <option
            v-for="department in departments"
            :key="department"
            :value="department"
          >
            {{ $t(`departments.${department}`) }}
          </option>
        </select>
      </div>

      <div>
        <label class="block text-sm font-medium" for="root-category">
          {{ $t("catalog.rootCategory") }}
        </label>
        <select
          id="root-category"
          v-model="form.rootCategoryId"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
        >
          <option value="" disabled>
            {{ $t("sell.selectRootCategory") }}
          </option>
          <option
            v-for="category in rootCategories"
            :key="category.id"
            :value="category.id"
          >
            {{ $t(category.nameKey) }}
          </option>
        </select>
      </div>

      <div>
        <label class="block text-sm font-medium" for="subcategory">
          {{ $t("catalog.subcategory") }}
        </label>
        <select
          id="subcategory"
          v-model="form.categoryId"
          :disabled="!form.rootCategoryId"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20 disabled:cursor-not-allowed disabled:opacity-50"
        >
          <option value="" disabled>
            {{ $t("sell.selectSubcategory") }}
          </option>
          <option
            v-for="category in subcategories"
            :key="category.id"
            :value="category.id"
          >
            {{ $t(category.nameKey) }}
          </option>
        </select>
      </div>
    </div>
  </section>
</template>
