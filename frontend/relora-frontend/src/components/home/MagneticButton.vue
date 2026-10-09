<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from 'vue'
import { RouterLink, type RouteLocationRaw } from 'vue-router'
import { animate, press } from 'motion'

// magnetic link button (Motion spring), hover fill is css
withDefaults(
  defineProps<{
    to: RouteLocationRaw
    variant?: 'dark' | 'light'
    // full width on mobile
    wide?: boolean
  }>(),
  {
    variant: 'dark',
    wide: false,
  },
)

const button = ref<HTMLElement | null>(null)
const label = ref<HTMLElement | null>(null)

const spring = { type: 'spring' as const, stiffness: 180, damping: 14, mass: 0.3 }
let stopPress: (() => void) | undefined

function onMove(event: PointerEvent) {
  if (!button.value || !label.value) return

  const rect = button.value.getBoundingClientRect()
  const x = event.clientX - (rect.left + rect.width / 2)
  const y = event.clientY - (rect.top + rect.height / 2)

  animate(button.value, { x: x * 0.3, y: y * 0.4 }, spring)
  animate(label.value, { x: x * 0.12, y: y * 0.15 }, spring)
}

function onLeave() {
  if (!button.value || !label.value) return

  animate(button.value, { x: 0, y: 0 }, spring)
  animate(label.value, { x: 0, y: 0 }, spring)
}

onMounted(() => {
  if (!button.value) return

  const canHover = window.matchMedia('(hover: hover) and (pointer: fine)').matches
  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches

  if (canHover && !reduceMotion) {
    button.value.addEventListener('pointermove', onMove)
    button.value.addEventListener('pointerleave', onLeave)
  }

  if (!reduceMotion) {
    // press effect, works on mobile too
    stopPress = press(button.value, (element) => {
      animate(element, { scale: 0.96 }, { duration: 0.12 })
      return () => animate(element, { scale: 1 }, spring)
    })
  }
})

onBeforeUnmount(() => {
  button.value?.removeEventListener('pointermove', onMove)
  button.value?.removeEventListener('pointerleave', onLeave)
  stopPress?.()
})
</script>

<template>
  <RouterLink v-slot="{ href, navigate }" :to="to" custom>
    <a
      ref="button"
      :href="href"
      class="magnetic-button group relative inline-flex h-14 items-center justify-center overflow-hidden rounded-full px-8 text-[13px] font-medium uppercase tracking-[0.16em] focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-[var(--auction-accent)]"
      :class="[
        variant === 'dark'
          ? 'bg-[#111111] text-white dark:bg-white dark:text-[#111111]'
          : 'border border-[#111111]/15 bg-white text-[#111111] dark:border-white/20 dark:bg-transparent dark:text-white',
        wide ? 'w-full sm:w-auto' : '',
      ]"
      @click="navigate"
    >
      <span class="magnetic-fill" aria-hidden="true" />
      <span ref="label" class="relative z-10 inline-flex items-center gap-3">
        <slot />
        <span class="magnetic-arrow" aria-hidden="true">→</span>
      </span>
    </a>
  </RouterLink>
</template>

<style scoped>
.magnetic-fill {
  position: absolute;
  inset: 0;
  border-radius: inherit;
  background: var(--auction-accent);
  transform: translateY(101%);
  transition: transform 0.5s cubic-bezier(0.22, 1, 0.36, 1);
}

.magnetic-button:hover .magnetic-fill,
.magnetic-button:focus-visible .magnetic-fill {
  transform: translateY(0);
}

.magnetic-button:hover,
.magnetic-button:focus-visible {
  color: #ffffff;
}

.magnetic-arrow {
  display: inline-block;
  transition: transform 0.4s cubic-bezier(0.22, 1, 0.36, 1);
}

.magnetic-button:hover .magnetic-arrow {
  transform: translateX(4px);
}

@media (prefers-reduced-motion: reduce) {
  .magnetic-fill,
  .magnetic-arrow {
    transition: none;
  }
}
</style>
