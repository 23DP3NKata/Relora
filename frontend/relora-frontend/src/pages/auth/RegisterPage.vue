<script setup lang="ts">
import Button from '@/components/ui/button/Button.vue'
import Input from '@/components/ui/input/Input.vue'

import { ref } from 'vue'
import { RouterLink , useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/authStore'
import axios from 'axios'
import { toast } from "vue-sonner"
import { useLocalePath } from '@/composables/useLocalePath'

const router = useRouter()
const localePath = useLocalePath()
const { t } = useI18n()
const authStore = useAuthStore()

const username = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const errorMessage = ref('')

const handleRegister = async (): Promise<void> => {
  toast.dismiss()
  errorMessage.value = ''

  if (password.value !== confirmPassword.value) {
    errorMessage.value = t('auth.passwordMismatch')
    return
  }

  try 
  {
    await authStore.register(username.value, email.value, password.value, confirmPassword.value)
    router.push(localePath('/home'))
    toast.success(t('auth.registerSuccess'), { position: "bottom-right" })

  } 
  catch (error) 
  {
    if (axios.isAxiosError(error)) 
    {
      errorMessage.value = error.response?.data?.message ?? t('auth.registrationFailed')
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
                {{ $t('auth.registerEyebrow') }}
              </p>

              <h1 class="mt-3 text-3xl font-semibold tracking-tight text-black sm:text-4xl">
                {{ $t('auth.registerTitle') }}
              </h1>

              <p class="mt-3 text-sm leading-6 text-black/60 sm:text-base">
                {{ $t('auth.registerDescription') }}
              </p>
            </div>

            <form @submit.prevent="handleRegister" class="mt-8 space-y-5">

              <div>
                <label for="text" class="block text-sm font-medium text-black">
                  {{ $t('auth.username') }}
                </label>
                <div class="mt-2">
                  <Input
                    v-model="username"
                    id="username"
                    type="text"
                    name="username"
                    required
                    autocomplete="username"
                    :placeholder="$t('auth.usernamePlaceholder')"
                    class="block h-12 w-full rounded-2xl border border-black/10 bg-neutral-100 px-4 text-black placeholder:text-black/35 focus:border-black focus:bg-white focus:outline-none"
                  />
                </div>
              </div>

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
                    :placeholder="$t('auth.emailPlaceholder')"
                    class="block h-12 w-full rounded-2xl border border-black/10 bg-neutral-100 px-4 text-black placeholder:text-black/35 focus:border-black focus:bg-white focus:outline-none"
                  />
                </div>
              </div>

              <div>
                <label for="password" class="block text-sm font-medium text-black">
                  {{ $t('auth.password') }}
                </label>
                <div class="mt-2">
                  <Input
                    v-model="password"
                    id="password"
                    type="password"
                    name="password"
                    required
                    autocomplete="new-password"
                    :placeholder="$t('auth.createPasswordPlaceholder')"
                    class="block h-12 w-full rounded-2xl border border-black/10 bg-neutral-100 px-4 text-black placeholder:text-black/35 focus:border-black focus:bg-white focus:outline-none"
                  />
                </div>
              </div>

              <div>
                <label for="confirmPassword" class="block text-sm font-medium text-black">
                  {{ $t('auth.confirmPassword') }}
                </label>
                <div class="mt-2">
                  <Input
                    v-model="confirmPassword"
                    id="confirmPassword"
                    type="password"
                    name="confirmPassword"
                    required
                    autocomplete="new-password"
                    :placeholder="$t('auth.repeatPasswordPlaceholder')"
                    class="block h-12 w-full rounded-2xl border border-black/10 bg-neutral-100 px-4 text-black placeholder:text-black/35 focus:border-black focus:bg-white focus:outline-none"
                  />
                </div>
              </div>

              <div class="pt-2">
                <Button
                  type="submit"
                  class="flex h-12 w-full items-center justify-center rounded-full bg-black px-5 text-sm font-medium text-white transition hover:bg-black/90"
                >
                  {{ $t('auth.createAccount') }}
                </Button>
              </div>

              <p v-if="errorMessage" class="text-red-500">
                {{ errorMessage }}
              </p>
            </form>

            <div class="mt-6 text-sm text-black/55">
              {{ $t('auth.alreadyHaveAccount') }}
              <RouterLink
                :to="localePath('/login')"
                class="font-medium text-black transition hover:text-black/70"
              >
                {{ $t('navigation.signIn') }}
              </RouterLink>
            </div>
          </div>
        </section>

        <section class="flex w-full items-center bg-neutral-100 px-6 py-10 sm:px-10 lg:w-[52%] lg:px-12 xl:px-16">
          <div class="max-w-xl">
            <p class="text-[11px] uppercase tracking-[0.24em] text-black/40">
              {{ $t('auth.joinEyebrow') }}
            </p>

            <h2 class="mt-4 text-4xl font-semibold tracking-tight text-black sm:text-5xl xl:text-6xl">
              {{ $t('auth.joinTitleLine1') }}
              <br />
              {{ $t('auth.joinTitleLine2') }}
            </h2>

            <p class="mt-5 max-w-lg text-sm leading-7 text-black/65 sm:text-base">
              {{ $t('auth.joinDescription') }}
            </p>
          </div>
        </section>
      </div>
    </div>
  </div>
</template>
