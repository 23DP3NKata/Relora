<script setup lang="ts">
const emit = defineEmits<{
  select: [value: 'men' | 'women']
}>()

function selectDefault() {
  emit('select', 'women')
}
</script>

<template>
  <div
    class="fixed inset-0 z-50 flex items-center justify-center bg-black/50"
    @click="selectDefault"
  >
    <div
      class="w-[450px] rounded-2xl bg-background p-8 shadow-xl"
      @click.stop
    >
      <h2 class="text-center text-2xl font-semibold">
        Welcome to Relora
      </h2>

      <p class="mt-3 text-center text-foreground">
        Select your preferred catalog
      </p>

      <div class="mt-8 grid grid-cols-2 gap-4">
        <button
          class="rounded-xl border p-4 hover:bg-foreground/10"
          @click="emit('select', 'men')"
        >
          Men
        </button>

        <button
          class="rounded-xl border p-4 hover:bg-foreground/10"
          @click="emit('select', 'women')"
        >
          Women
        </button>
      </div>
    </div>
  </div>
</template>