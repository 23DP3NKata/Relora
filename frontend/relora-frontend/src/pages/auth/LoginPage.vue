<script setup lang="ts">
import Button from '@/components/ui/button/Button.vue'
import Input from '@/components/ui/input/Input.vue'

import { ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/authStore'
import axios from 'axios'
import { toast } from "vue-sonner"
import { useLocalePath } from '@/composables/useLocalePath'

const router = useRouter()
const localePath = useLocalePath()
const { t } = useI18n()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')
const errorMessage = ref('')

const handleLogin = async (): Promise<void>  => {
    errorMessage.value = ''

    try
    {
      await authStore.login(email.value, password.value)
      router.push(localePath('/home'))
      toast.success(t('auth.loginSuccess'), { position: "bottom-right" })
    }
    catch (error) 
    {
      if (axios.isAxiosError(error)) {
        errorMessage.value = error.response?.data?.message ?? t('auth.loginFailed')
        toast.error(errorMessage.value, { position: "bottom-right" })
        return
    }
    
    errorMessage.value = t('auth.unexpectedError')
  }
}
</script>

<template>
  <div class="min-h-screen bg-white px-4 py-6 sm:px-6 lg:px-8">
    <div class="mx-auto flex min-h-[calc(100vh-3rem)] max-w-5xl overflow-hidden rounded-[32px] border border-black/10 bg-white">
      <div class="flex w-full flex-col lg:flex-row">
        <section class="flex w-full items-center justify-center px-6 py-10 sm:px-10 lg:w-[48%] lg:px-12 xl:px-16">
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
                {{ $t('auth.loginEyebrow') }}
              </p>

              <h1 class="mt-3 text-3xl font-semibold tracking-tight text-black sm:text-4xl">
                {{ $t('auth.loginTitle') }}
              </h1>

              <p class="mt-3 text-sm leading-6 text-black/60 sm:text-base">
                {{ $t('auth.loginDescription') }}
              </p>
            </div>

            <form @submit.prevent="handleLogin" class="mt-8 space-y-5">
              <div>
                <label for="email" class="block text-sm font-medium text-black">
                  {{ $t('auth.emailAddress') }}
                </label>
                <div class="mt-2">
                  <Input
                    v-model="email"
                    id="email"
                    type="email"
                    name="email"
                    required
                    autocomplete="email"
                    class="block h-12 w-full rounded-2xl border border-black/10 bg-neutral-100 px-4 text-black placeholder:text-black/35 focus:border-black focus:bg-white focus:outline-none"
                    :placeholder="$t('auth.emailPlaceholder')"
                  />
                </div>
              </div>

              <div>
                <div class="flex items-center justify-between gap-3">
                  <label for="password" class="block text-sm font-medium text-black">
                    {{ $t('auth.password') }}
                  </label>

                  <RouterLink
                    :to="localePath('/forgot-password')"
                    class="text-sm font-medium text-black/55 transition hover:text-black"
                  >
                    {{ $t('auth.forgotPassword') }}
                  </RouterLink>
                </div>

                <div class="mt-2">
                  <Input
                    v-model="password"
                    id="password"
                    type="password"
                    name="password"
                    required
                    autocomplete="current-password"
                    class="block h-12 w-full rounded-2xl border border-black/10 bg-neutral-100 px-4 text-black placeholder:text-black/35 focus:border-black focus:bg-white focus:outline-none"
                    :placeholder="$t('auth.passwordPlaceholder')"
                  />
                </div>
              </div>

              <div class="pt-2">
                <Button
                  type="submit"
                  class="cursor-pointer flex h-12 w-full items-center justify-center rounded-full bg-black px-5 text-sm font-medium text-white transition hover:bg-black/90"
                >
                  {{ $t('navigation.signIn') }}
                </Button>
              </div>
              <p v-if="errorMessage" class="text-red-500">
                {{ errorMessage }}
              </p>
            </form>

            <div class="mt-6 text-sm text-black/55">
              {{ $t('auth.noAccount') }}
              <RouterLink
                :to="localePath('/register')"
                class="font-medium text-black transition hover:text-black/70"
              >
                {{ $t('auth.createOne') }}
              </RouterLink>
            </div>
          </div>
        </section>

        <section class="flex w-full items-center bg-neutral-100 px-6 py-10 sm:px-10 lg:w-[52%] lg:px-12 xl:px-16">
          <div class="max-w-xl">
            <p class="text-[11px] uppercase tracking-[0.24em] text-black/40">
              {{ $t('auth.marketplaceEyebrow') }}
            </p>

            <h2 class="mt-4 text-4xl font-semibold tracking-tight text-black sm:text-5xl xl:text-6xl">
              {{ $t('auth.marketplaceTitle') }}
            </h2>

            <p class="mt-5 max-w-lg text-sm leading-7 text-black/65 sm:text-base">
              {{ $t('auth.marketplaceDescription') }}
            </p>
          </div>
        </section>
      </div>
    </div>
  </div>
</template>
