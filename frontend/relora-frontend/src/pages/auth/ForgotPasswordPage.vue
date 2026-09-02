<script setup lang="ts">
import { RouterLink } from 'vue-router'
import Button from '@/components/ui/button/Button.vue'
import Input from '@/components/ui/input/Input.vue'
import { useLocalePath } from '@/composables/useLocalePath'

const localePath = useLocalePath()
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-white px-4 sm:px-6 lg:px-8">
    <div class="w-full max-w-2xl rounded-[32px] border border-black/10 bg-white">
      <section class="flex w-full items-center justify-center px-6 py-10 sm:px-10 lg:px-12 xl:px-16">
        <div class="w-full max-w-md">
          <div class="flex justify-center">
            <RouterLink
              :to="localePath('/home')"
              class="inline-flex text-4xl font-semibold tracking-tight text-black transition hover:opacity-80"
            >
              Relora
            </RouterLink>
          </div>

          <div class="mt-8">
            <p class="text-[11px] uppercase tracking-[0.24em] text-black/40">
              {{ $t('auth.passwordReset') }}
            </p>

            <h1 class="mt-3 text-3xl font-semibold tracking-tight text-black sm:text-4xl">
              {{ $t('auth.forgotTitle') }}
            </h1>

            <p class="mt-3 text-sm leading-6 text-black/60 sm:text-base">
              {{ $t('auth.forgotDescription') }}
            </p>
          </div>

          <form action="#" method="POST" class="mt-8 space-y-5">
            <div>
              <label for="email" class="block text-sm font-medium text-black">
                {{ $t('auth.emailAddress') }}
              </label>
              <div class="mt-2">
                <Input
                  id="email"
                  type="email"
                  name="email"
                  required
                  autocomplete="email"
                  :placeholder="$t('auth.emailPlaceholder')"
                  class="block h-12 w-full rounded-2xl border border-black/10 bg-neutral-100 px-4 text-black placeholder:text-black/35 focus:border-black focus:bg-white focus:outline-none"
                />
              </div>
            </div>

            <div class="pt-2">
              <Button
                type="submit"
                class="flex h-12 w-full items-center justify-center rounded-full bg-black px-5 text-sm font-medium text-white transition hover:bg-black/90"
              >
                {{ $t('auth.sendResetLink') }}
              </Button>
            </div>
          </form>

          <div class="mt-6 text-sm text-black/55">
            {{ $t('auth.rememberedPassword') }}
            <RouterLink
              :to="localePath('/login')"
              class="font-medium text-black transition hover:text-black/70"
            >
              {{ $t('auth.backToSignIn') }}
            </RouterLink>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>
