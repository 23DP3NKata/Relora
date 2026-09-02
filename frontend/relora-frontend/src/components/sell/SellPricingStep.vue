<script setup lang="ts">
type PricingForm = {
  amount: number | null
  currency: string
  shippingPrice: number | null
  shippingCurrency: string
  shippingOriginCountry: string
  shipsToCountries: string
  shippingHandlingDays: number
}

defineProps<{
  form: PricingForm
  currencyOptions?: string[]
}>()
</script>

<template>
  <section class="rounded-[28px] border bg-background p-6 sm:p-8">
    <div>
      <p class="text-xs uppercase tracking-[0.22em] text-foreground/50">
        {{ $t("sell.stepOf", { step: 4, total: 6 }) }}
      </p>

      <h2 class="mt-3 text-2xl font-semibold tracking-tight sm:text-3xl">
        {{ $t("sell.pricing") }}
      </h2>

      <p class="mt-3 text-sm leading-6 text-foreground/70 sm:text-base">
        {{ $t("sell.pricingDescription") }}
      </p>
    </div>

    <div class="mt-8 grid grid-cols-1 gap-6 md:grid-cols-2">
      <div>
        <label class="block text-sm font-medium">{{ $t("sell.amount") }}</label>
        <input
          v-model.number="form.amount"
          type="number"
          min="0"
          step="1"
          max="1000000"
          :placeholder="$t('sell.amountPlaceholder')"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition placeholder:text-foreground/40 focus:ring-2 focus:ring-foreground/20"
        />
      </div>

      <div>
        <label class="block text-sm font-medium">{{ $t("sell.currency") }}</label>
        <select
          v-model="form.currency"
          class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
        >
          <option
            v-for="currency in currencyOptions ?? ['EUR']"
            :key="currency"
            :value="currency"
          >
            {{ currency }}
          </option>
        </select>
      </div>
    </div>

    <div class="mt-8 border-t pt-8">
      <h3 class="text-base font-semibold">{{ $t("common.shipping") }}</h3>
      <p class="mt-2 text-sm leading-6 text-foreground/65">
        {{ $t("sell.shippingDescription") }}
      </p>

      <div class="mt-5 grid grid-cols-1 gap-6 md:grid-cols-2">
        <div>
          <label class="block text-sm font-medium">{{ $t("sell.shippingPrice") }}</label>
          <input
            v-model.number="form.shippingPrice"
            type="number"
            min="0"
            step="0.01"
            :placeholder="$t('sell.shippingPricePlaceholder')"
            class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition placeholder:text-foreground/40 focus:ring-2 focus:ring-foreground/20"
          />
        </div>

        <div>
          <label class="block text-sm font-medium">{{ $t("sell.shippingCurrency") }}</label>
          <select
            v-model="form.shippingCurrency"
            class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
          >
            <option
              v-for="currency in currencyOptions ?? ['EUR']"
              :key="currency"
              :value="currency"
            >
              {{ currency }}
            </option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium">{{ $t("common.shipsFrom") }}</label>
          <input
            v-model="form.shippingOriginCountry"
            :placeholder="$t('catalog.options.Latvia')"
            class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition placeholder:text-foreground/40 focus:ring-2 focus:ring-foreground/20"
          />
        </div>

        <div>
          <label class="block text-sm font-medium">{{ $t("sell.handlingTime") }}</label>
          <input
            v-model.number="form.shippingHandlingDays"
            type="number"
            min="1"
            max="30"
            step="1"
            class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition focus:ring-2 focus:ring-foreground/20"
          />
        </div>

        <div class="md:col-span-2">
          <label class="block text-sm font-medium">{{ $t("common.shipsTo") }}</label>
          <input
            v-model="form.shipsToCountries"
            :placeholder="$t('sell.shipsToPlaceholder')"
            class="mt-2 w-full rounded-2xl border bg-background px-4 py-3 text-sm outline-none transition placeholder:text-foreground/40 focus:ring-2 focus:ring-foreground/20"
          />
        </div>
      </div>
    </div>
  </section>
</template>
