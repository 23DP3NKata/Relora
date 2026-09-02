<script setup lang="ts">
import { computed, ref } from 'vue';
import { DESIGNERS } from '@/constants/designers';
import Input from '@/components/ui/input/Input.vue';
import { useLocalePath } from '@/composables/useLocalePath'

const search = ref('');
const localePath = useLocalePath()

const alphabet = '#ABCDEFGHIJKLMNOPQRSTUVWXYZ'.split('');

const filteredDesigners = computed(() => {
  return DESIGNERS      
    .filter(designer =>
      designer.toLowerCase().includes(search.value.toLowerCase())
    )
    .sort((a, b) => a.localeCompare(b));
});

const groupedDesigners = computed(() => {
  const groups: Record<string, string[]> = {};

  for (const designer of filteredDesigners.value) {
    const firstChar = designer.charAt(0).toUpperCase();
    const key = /^[A-Z]$/.test(firstChar) ? firstChar : '#';

    if (!groups[key]) groups[key] = [];
    groups[key].push(designer);
  }

  return groups;
});

const scrollToLetter = (letter: string) => {
  document.getElementById(`letter-${letter}`)?.scrollIntoView({
    behavior: 'smooth',
    block: 'center',
  });
};

function slugify(value: string): string {
  return value
    .trim()
    .toLowerCase()
    .replace(/&/g, 'and')
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '')
}
</script>

<template>
  <main class="min-h-screen bg-background text-foreground">
    <section class="mx-auto max-w-7xl px-6 py-12">
      <div class="mb-10 text-center">
        <h1 class="text-4xl font-semibold md:text-6xl">
          {{ $t('designers.title') }}
        </h1>

        <p class="mx-auto mt-4 max-w-2xl text-foreground/70">
          {{ $t('designers.description') }}
        </p>
      </div>

      <div class="mx-auto mb-8 max-w-xl">
        <Input
          v-model="search"
          type="text"
          :placeholder="$t('search.designerPlaceholder')"
          class="w-full rounded-full border border-input text-foreground placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-0 bg-muted-background px-5 py-3 text-sm outline-none transition focus:border-neutral-900"
        />
      </div>

    <div class="sticky top-0 z-30 mb-12 border-y bg-background/90 py-4 backdrop-blur">
        <div class="flex flex-wrap justify-center gap-4">
            <button
            v-for="letter in alphabet"
            :key="letter"
            type="button"
            class="text-lg font-semibold underline-offset-4 hover:cursor-pointer hover:underline"
            @click="scrollToLetter(letter)"
            >
            {{ letter }}
            </button>
        </div>
    </div>

      <div v-if="filteredDesigners.length === 0" class="py-20 text-center text-neutral-500">
        {{ $t('designers.noDesigners') }}
      </div>

      <div v-else class="space-y-14">
        <section
          v-for="letter in alphabet"
          :key="letter"
          :id="`letter-${letter}`"
          v-show="groupedDesigners[letter]?.length"
          class="grid gap-8 border-t border-neutral-200 pt-8 md:grid-cols-[120px_1fr]"
        >
          <div class="text-7xl">
            {{ letter }}
          </div>

          <div class="grid gap-x-10 gap-y-3 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
            <RouterLink
              v-for="designer in groupedDesigners[letter]"
              :key="designer"
              :to="localePath({
                name: 'designer-catalog',
                params: {
                  brandSlug: slugify(designer),
                },
              })"
              class="text-sm uppercase tracking-wide text-foreground hover:underline"
            >
              {{ designer }}
            </RouterLink>
          </div>
        </section>
      </div>
    </section>
  </main>
</template>
