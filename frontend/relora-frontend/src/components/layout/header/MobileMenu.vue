<script setup lang="ts">
import Input from '@/components/ui/input/Input.vue'
import ModeToggle from '@/components/theme/ModeToggle.vue'
import LanguageSwitcher from '@/components/layout/LanguageSwitcher.vue'
import MobileCategoryGrid from './MobileCategoryGrid.vue'
import MobileAccountCard from './MobileAccountCard.vue'
import { useLocalePath } from '@/composables/useLocalePath'

const localePath = useLocalePath()

defineProps<{
  isOpen: boolean
  searchQuery: string
  isAuthenticated: boolean
  user: any
}>()

const emit = defineEmits<{
  close: []
  search: []
  logout: []
  'update:searchQuery': [value: string]
}>()
</script>

<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition-opacity duration-200 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-200 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="isOpen"
        class="fixed inset-0 z-[70] md:hidden"
      >
        <div
          class="absolute inset-0 z-0 bg-black/45 backdrop-blur-[2px]"
          @click="emit('close')"
        />

        <Transition
          enter-active-class="transition-transform duration-300 ease-out"
          enter-from-class="-translate-x-full"
          enter-to-class="translate-x-0"
          leave-active-class="transition-transform duration-300 ease-in"
          leave-from-class="translate-x-0"
          leave-to-class="-translate-x-full"
        >
          <aside
            v-if="isOpen"
            class="absolute inset-y-0 left-0 z-10 flex h-full w-full flex-col overflow-hidden bg-background text-foreground shadow-2xl"
            @click.stop
          >
            <div class="flex items-center justify-between px-5 pb-4 pt-5">
              <RouterLink
                :to="localePath('/home')"
                class="text-2xl font-semibold tracking-tight"
                @click="emit('close')"
              >
                Relora
              </RouterLink>

              <button
                type="button"
                class="inline-flex h-11 w-11 items-center justify-center rounded-full border border-border bg-background text-foreground transition hover:bg-accent hover:text-accent-foreground"
                :aria-label="$t('common.closeMenu')"
                @click="emit('close')"
              >
                ✕
              </button>
            </div>

            <div class="min-h-0 flex-1 overflow-y-auto px-5 pb-8 pt-1 overscroll-contain">
              <div class="pb-5">
                <div class="relative">
                  <Input
                    :model-value="searchQuery"
                    @update:model-value="emit('update:searchQuery', String($event))"
                    @keydown.enter="emit('search')"
                    :placeholder="$t('search.mobilePlaceholder')"
                    class="h-12 rounded-full border border-input bg-muted/80 pl-11 pr-24 text-sm text-foreground placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-0"
                  />

                  <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-4">
                    <svg class="h-4 w-4 text-muted-foreground" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                    </svg>
                  </div>

                  <button
                    type="button"
                    class="absolute inset-y-1 right-1 inline-flex items-center rounded-full bg-primary px-4 text-xs font-medium text-primary-foreground transition hover:opacity-90"
                    @click="emit('search')"
                  >
                    {{ $t('common.search') }}
                  </button>
                </div>
              </div>

              <MobileCategoryGrid @close="emit('close')" />

              <MobileAccountCard
                :is-authenticated="isAuthenticated"
                :user="user"
                @close="emit('close')"
                @logout="emit('logout')"
              />

              <div class="pb-8">
                <div class="flex items-center justify-between rounded-[28px] border border-border bg-background px-4 py-4">
                  <div>
                    <p class="text-sm font-medium text-foreground">
                      {{ $t('theme.label') }}
                    </p>

                    <p class="text-xs text-muted-foreground">
                      {{ $t('theme.description') }}
                    </p>
                  </div>

                  <ModeToggle />
                </div>

                <div class="mt-3 flex items-center justify-between rounded-[28px] border border-border bg-background px-4 py-4">
                  <p class="text-sm font-medium text-foreground">
                    {{ $t('common.language') }}
                  </p>

                  <LanguageSwitcher />
                </div>
              </div>
            </div>
          </aside>
        </Transition>
      </div>
    </Transition>
  </Teleport>
</template>
