<script lang="ts" setup>
import type { ToasterProps } from "vue-sonner"
import { reactiveOmit } from "@vueuse/core"
import { CircleCheckIcon, InfoIcon, Loader2Icon, OctagonXIcon, TriangleAlertIcon, XIcon } from "lucide-vue-next"
import { Toaster as Sonner } from "vue-sonner"

const props = defineProps<ToasterProps>()
const delegatedProps = reactiveOmit(props, "toastOptions")
</script>

<template>
  <Sonner
    class="toaster group"
    :toast-options="{
    classes: {
      toast:
        'group toast rounded-2xl border border-black/10 bg-white px-4 py-3 text-zinc-950 shadow-2xl',
      title:
        'text-sm font-semibold text-zinc-950',
      description:
        'text-sm text-zinc-500',
      actionButton:
        'rounded-xl bg-zinc-950 px-3 py-2 text-sm font-medium text-white',
      cancelButton:
        'rounded-xl bg-zinc-100 px-3 py-2 text-sm font-medium text-zinc-700',
    },
  }"
    v-bind="delegatedProps"
  >
    <template #success-icon>
      <CircleCheckIcon class="size-4" />
    </template>
    <template #info-icon>
      <InfoIcon class="size-4" />
    </template>
    <template #warning-icon>
      <TriangleAlertIcon class="size-4" />
    </template>
    <template #error-icon>
      <OctagonXIcon class="size-4" />
    </template>
    <template #loading-icon>
      <div>
        <Loader2Icon class="size-4 animate-spin" />
      </div>
    </template>
    <template #close-icon>
      <XIcon class="size-4" />
    </template>
  </Sonner>
</template>
