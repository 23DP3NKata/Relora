<script setup lang="ts">
import { useLocalePath } from '@/composables/useLocalePath'

defineProps<{
  isAuthenticated: boolean
  user: any
}>()

defineEmits<{
  close: []
  logout: []
}>()

const localePath = useLocalePath()
</script>

<template>
  <div class="pb-6">
    <div class="rounded-[28px] border border-border bg-muted/40 p-4">
      <p class="text-xs uppercase tracking-[0.24em] text-muted-foreground">
        {{ $t('navigation.myAccount') }}
      </p>

      <div v-if="!isAuthenticated" class="mt-4 space-y-3">
        <RouterLink
          :to="localePath('/login')"
          class="flex h-12 items-center justify-center rounded-full bg-primary text-sm font-medium text-primary-foreground transition hover:opacity-90"
          @click="$emit('close')"
        >
          {{ $t('navigation.signIn') }}
        </RouterLink>

        <RouterLink
          :to="localePath('/register')"
          class="flex h-12 items-center justify-center rounded-full border border-border bg-background text-sm font-medium text-foreground transition hover:bg-accent hover:text-accent-foreground"
          @click="$emit('close')"
        >
          {{ $t('navigation.register') }}
        </RouterLink>
      </div>

      <div v-else class="mt-4 rounded-[24px] border border-border bg-background p-4">
        <div class="flex items-start gap-3">
          <div class="flex h-12 w-12 items-center justify-center rounded-full bg-primary text-sm font-semibold text-primary-foreground">
            {{ user?.username?.slice(0, 1)?.toUpperCase() ?? 'U' }}
          </div>

          <div class="min-w-0 flex-1">
            <p class="text-sm font-medium text-foreground">
              {{ $t('navigation.myProfile') }}
            </p>

            <p class="mt-1 truncate text-sm text-muted-foreground">
              {{ user?.username ? `@${user.username}` : $t('navigation.signedInAccount') }}
            </p>
          </div>
        </div>

        <div class="mt-4 space-y-2">
          <RouterLink
            :to="localePath(user?.username ? `/profile/${user.username}` : '/login')"
            class="flex h-11 items-center justify-center rounded-full border border-border bg-muted text-sm font-medium text-foreground transition hover:bg-accent hover:text-accent-foreground"
            @click="$emit('close')"
          >
            {{ $t('navigation.myProfile') }}
          </RouterLink>

          <RouterLink
            :to="localePath('/orders')"
            class="flex h-11 items-center justify-center rounded-full border border-border bg-muted text-sm font-medium text-foreground transition hover:bg-accent hover:text-accent-foreground"
            @click="$emit('close')"
          >
            {{ $t('navigation.yourOrders') }}
          </RouterLink>

          <RouterLink
            :to="localePath('/listings')"
            class="flex h-11 items-center justify-center rounded-full border border-border bg-muted text-sm font-medium text-foreground transition hover:bg-accent hover:text-accent-foreground"
            @click="$emit('close')"
          >
            {{ $t('navigation.myListings') }}
          </RouterLink>

          <RouterLink
            :to="localePath('/')"
            class="flex h-11 items-center justify-center rounded-full border border-border bg-muted text-sm font-medium text-foreground transition hover:bg-accent hover:text-accent-foreground"
            @click="$emit('close')"
          >
            {{ $t('navigation.settings') }}
          </RouterLink>

          <button
            type="button"
            class="flex h-11 w-full items-center justify-center rounded-full border border-border bg-primary text-sm font-medium text-primary-foreground transition hover:opacity-90"
            @click="$emit('logout')"
          >
            {{ $t('navigation.signOut') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
