<script setup lang="ts">
import type { NumericOption } from "@/features/lots/create-lot/options"

type BasicInfoForm = {
  title: string
  brand: string
  category: number | null
  gender: number | null
  size: number | null
}

const props = defineProps<{
  form: BasicInfoForm
  categoryOptions: NumericOption[]
  genderOptions: NumericOption[]
  sizeOptions: NumericOption[]
}>()
</script>

<template>
  <section class="rounded-[28px] border bg-background p-6 sm:p-8">
    <div>
      <p class="text-xs uppercase tracking-[0.22em] text-foreground/50">
        {{ $t("sell.stepOf", { step: 2, total: 6 }) }}
      </p>

      <h2 class="mt-3 text-2xl font-semibold tracking-tight sm:text-3xl">
        {{ $t("sell.basicInfo") }}
      </h2>

      <p class="mt-3 text-sm leading-6 text-foreground/70 sm:text-base">
        {{ $t("sell.basicInfoDescription") }}
      </p>
    </div>

    <div class="mt-8 grid grid-cols-1 gap-6 md:grid-cols-2">
      <div class="md:col-span-2">
        <label class="block text-sm font-medium">{{ $t("sell.title") }}</label>
        <input
          v-model="props.form.title"
          type="text"
          :placeholder="$t('sell.titlePlaceholder')"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition placeholder:text-foreground/40 focus:ring-2 focus:ring-foreground/20"
        />
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("sell.brand") }}</label>
        <input
          v-model="props.form.brand"
          type="text"
          :placeholder="$t('sell.brandPlaceholder')"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition placeholder:text-foreground/40 focus:ring-2 focus:ring-foreground/20"
        />
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("catalog.category") }}</label>
        <select
          v-model.number="props.form.category"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
        >
          <option :value="null" disabled>
            {{ $t("sell.selectCategory") }}
          </option>
          <option
            v-for="category in categoryOptions"
            :key="category.value"
            :value="category.value"
          >
            {{ $t(category.labelKey) }}
          </option>
        </select>
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("catalog.gender") }}</label>
        <select
          v-model.number="props.form.gender"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
        >
          <option :value="null" disabled>
            {{ $t("sell.selectGender") }}
          </option>
          <option
            v-for="gender in genderOptions"
            :key="gender.value"
            :value="gender.value"
          >
            {{ $t(gender.labelKey) }}
          </option>
        </select>
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("catalog.size") }}</label>
        <select
          v-model.number="props.form.size"
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
    </div>
  </section>
</template>
