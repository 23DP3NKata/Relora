<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'

import { orderService } from '@/services/orderService'
import type { OverdueShipment } from '@/types/order'

const { locale, t } = useI18n()
const isLoading = ref(true)
const errorMessage = ref('')
const orders = ref<OverdueShipment[]>([])

const formatDate = (value: string | null) => {
  if (!value) return t('common.unavailable')
  return new Date(value).toLocaleString(locale.value, { dateStyle: 'medium', timeStyle: 'short' })
}

const formatPrice = (order: OverdueShipment) =>
  new Intl.NumberFormat(locale.value, { style: 'currency', currency: order.currency || 'EUR' }).format(order.totalPrice)

const shortId = (id: string) => id.slice(0, 8)

const loadOrders = async () => {
  try {
    isLoading.value = true
    errorMessage.value = ''
    orders.value = await orderService.getOverdueShipments()
  } catch (error) {
    console.error(error)
    errorMessage.value = t('admin.overdue.loadFailed')
  } finally {
    isLoading.value = false
  }
}

onMounted(loadOrders)
</script>

<template>
  <section class="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
    <div class="mb-8 flex flex-wrap items-end justify-between gap-4">
      <div>
        <p class="eyebrow">{{ $t('admin.badge') }}</p>
        <h1 class="mt-2 text-3xl font-semibold tracking-tight">
          {{ $t('admin.overdue.title') }}
          <span v-if="!isLoading" class="text-neutral-400">{{ orders.length }}</span>
        </h1>
        <p class="mt-2 text-sm text-neutral-500">{{ $t('admin.overdue.description') }}</p>
      </div>
      <button type="button" class="btn" :disabled="isLoading" @click="loadOrders">
        {{ $t('common.refresh') }}
      </button>
    </div>

    <p v-if="errorMessage" class="card mb-4 px-4 py-3 text-sm text-red-600">{{ errorMessage }}</p>

    <div v-if="isLoading" class="card h-64 animate-pulse bg-neutral-50" />

    <div v-else-if="orders.length === 0 && !errorMessage" class="card px-6 py-12 text-center">
      <p class="text-lg font-semibold">{{ $t('admin.overdue.empty') }}</p>
      <p class="mt-2 text-sm text-neutral-500">{{ $t('admin.overdue.emptyHint') }}</p>
    </div>

    <!-- telefonā tabula neietilpst, tāpēc rāda kartītes -->
    <div v-if="!isLoading && orders.length > 0" class="space-y-3 md:hidden">
      <article v-for="order in orders" :key="order.orderId" class="card p-4">
        <div class="flex items-start justify-between gap-3">
          <p class="eyebrow truncate">{{ order.lotBrand || $t('admin.pending.unknownBrand') }}</p>
          <span class="tag shrink-0">{{ $t(`admin.overdue.statuses.${order.statusName}`) }}</span>
        </div>
        <h3 class="mt-1 text-base font-semibold">{{ order.lotTitle || $t('admin.pending.untitledLot') }}</h3>
        <p class="mt-1 text-lg font-semibold tabular-nums">{{ formatPrice(order) }}</p>

        <dl class="mt-3 grid grid-cols-2 gap-x-4 gap-y-2 border-t border-neutral-200 pt-3 text-sm">
          <div>
            <dt class="eyebrow">{{ $t('admin.overdue.order') }}</dt>
            <dd class="font-mono text-neutral-500">{{ shortId(order.orderId) }}</dd>
          </div>
          <div>
            <dt class="eyebrow">{{ $t('admin.overdue.problemSince') }}</dt>
            <dd>{{ formatDate(order.shipByUtc) }}</dd>
            <dd class="text-xs text-neutral-400">{{ $t('admin.overdue.daysOverdue', { days: order.daysOverdue }) }}</dd>
          </div>
          <div>
            <dt class="eyebrow">{{ $t('admin.disputes.buyer') }}</dt>
            <dd class="font-mono text-neutral-500">{{ shortId(order.buyerId) }}</dd>
          </div>
          <div>
            <dt class="eyebrow">{{ $t('admin.pending.seller') }}</dt>
            <dd class="font-mono text-neutral-500">{{ shortId(order.sellerId) }}</dd>
          </div>
        </dl>
      </article>
    </div>

    <div v-if="!isLoading && orders.length > 0" class="card hidden overflow-x-auto md:block">
      <table class="min-w-full text-left text-sm">
        <thead class="border-b border-neutral-200">
          <tr>
            <th class="eyebrow px-4 py-3">{{ $t('admin.overdue.order') }}</th>
            <th class="eyebrow px-4 py-3">{{ $t('admin.overdue.lot') }}</th>
            <th class="eyebrow px-4 py-3">{{ $t('admin.disputes.buyer') }}</th>
            <th class="eyebrow px-4 py-3">{{ $t('admin.pending.seller') }}</th>
            <th class="eyebrow px-4 py-3">{{ $t('admin.overdue.total') }}</th>
            <th class="eyebrow px-4 py-3">{{ $t('admin.overdue.status') }}</th>
            <th class="eyebrow px-4 py-3">{{ $t('admin.overdue.problemSince') }}</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-neutral-200">
          <tr v-for="order in orders" :key="order.orderId" class="align-top">
            <td class="px-4 py-3 font-mono text-neutral-500" :title="order.orderId">{{ shortId(order.orderId) }}</td>
            <td class="px-4 py-3">
              <p class="font-medium">{{ order.lotTitle || $t('admin.pending.untitledLot') }}</p>
              <p class="text-xs text-neutral-400">{{ order.lotBrand || $t('admin.pending.unknownBrand') }}</p>
            </td>
            <td class="px-4 py-3 font-mono text-neutral-500" :title="order.buyerId">{{ shortId(order.buyerId) }}</td>
            <td class="px-4 py-3 font-mono text-neutral-500" :title="order.sellerId">{{ shortId(order.sellerId) }}</td>
            <td class="whitespace-nowrap px-4 py-3 font-medium tabular-nums">{{ formatPrice(order) }}</td>
            <td class="whitespace-nowrap px-4 py-3 text-neutral-500">{{ $t(`admin.overdue.statuses.${order.statusName}`) }}</td>
            <td class="whitespace-nowrap px-4 py-3">
              <p>{{ formatDate(order.shipByUtc) }}</p>
              <p class="text-xs text-neutral-400">{{ $t('admin.overdue.daysOverdue', { days: order.daysOverdue }) }}</p>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>
