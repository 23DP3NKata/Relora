<script setup lang="ts">
import type { CreateLotFormState } from "@/types/createLot"
import type { MeasurementDefinition } from "@/types/lotCatalog"

const props = defineProps<{
  form: CreateLotFormState
  measurementDefinitions: MeasurementDefinition[]
}>()

function getMeasurementValue(key: string): number | null {
  return props.form.measurements.find((measurement) => measurement.key === key)?.value ?? null
}

function setMeasurementValue(key: string, value: number | null, unit: string) {
  props.form.measurements = props.form.measurements.filter((measurement) => measurement.key !== key)

  if (value !== null && !Number.isNaN(value)) {
    props.form.measurements.push({
      key,
      value,
      unit,
    })
  }
}

function handleMeasurementInput(key: string, unit: string, event: Event) {
  const target = event.target as HTMLInputElement
  const value = target.value === "" ? null : Number(target.value)
  setMeasurementValue(key, value, unit)
}
</script>

<template>
  <section class="rounded-[28px] border bg-background p-6 sm:p-8">
    <div>
      <p class="text-xs uppercase tracking-[0.22em] text-foreground/50">
        {{ $t("sell.stepOf", { step: 3, total: 6 }) }}
      </p>

      <h2 class="mt-3 text-2xl font-semibold tracking-tight sm:text-3xl">
        {{ $t("sell.measurementsTitle") }}
      </h2>

      <p class="mt-3 text-sm leading-6 text-foreground/70 sm:text-base">
        {{ $t("sell.measurementsDescription") }}
      </p>
    </div>

    <div
      v-if="measurementDefinitions.length === 0"
      class="mt-8 rounded-[24px] border border-dashed p-5 text-sm text-foreground/60"
    >
      {{ $t("sell.noMeasurementsForCategory") }}
    </div>

    <div v-else class="mt-8 grid grid-cols-1 gap-6 md:grid-cols-2">
      <div
        v-for="definition in measurementDefinitions"
        :key="definition.key"
      >
        <label class="block text-sm font-medium" :for="`measurement-${definition.key}`">
          {{ $t(definition.labelKey) }}
          <span v-if="!definition.isRequired" class="text-foreground/45">
            {{ $t("common.optional") }}
          </span>
        </label>
        <div class="mt-2 flex rounded-2xl border bg-background focus-within:ring-2 focus-within:ring-foreground/20">
          <input
            :id="`measurement-${definition.key}`"
            :value="getMeasurementValue(definition.key)"
            type="number"
            min="0"
            step="0.1"
            class="min-w-0 flex-1 rounded-l-2xl bg-transparent px-4 py-3 text-sm outline-none"
            @input="handleMeasurementInput(definition.key, definition.unit, $event)"
          />
          <span class="inline-flex items-center rounded-r-2xl border-l px-4 text-sm text-foreground/60">
            {{ definition.unit }}
          </span>
        </div>
      </div>
    </div>
  </section>
</template>
