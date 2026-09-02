<script setup lang="ts">
import type { NumericOption } from "@/features/lots/create-lot/options"

type ConditionForm = {
  condition: number | null
  color: string
}

defineProps<{
  form: ConditionForm
  conditionOptions: NumericOption[]
}>()
</script>

<template>
  <section class="rounded-[28px] border bg-background p-6 sm:p-8">
    <div>
      <p class="text-xs uppercase tracking-[0.22em] text-foreground/50">
        {{ $t("sell.stepOf", { step: 3, total: 6 }) }}
      </p>

      <h2 class="mt-3 text-2xl font-semibold tracking-tight sm:text-3xl">
        {{ $t("sell.conditionAppearance") }}
      </h2>

      <p class="mt-3 text-sm leading-6 text-foreground/70 sm:text-base">
        {{ $t("sell.conditionAppearanceDescription") }}
      </p>
    </div>

    <div class="mt-8 grid grid-cols-1 gap-6 md:grid-cols-2">
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
        <label class="block text-sm font-medium">{{ $t("sell.color") }}</label>
        <input
          v-model="form.color"
          type="text"
          :placeholder="$t('sell.colorPlaceholder')"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition placeholder:text-foreground/40 focus:ring-2 focus:ring-foreground/20"
        />
      </div>
    </div>
  </section>
</template>
