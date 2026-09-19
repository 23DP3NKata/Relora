<script setup lang="ts">
import Button from '@/components/ui/button/Button.vue'
import Input from '@/components/ui/input/Input.vue'
import { reactive, ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/authStore'
import { toast } from 'vue-sonner'
import { useLocalePath } from '@/composables/useLocalePath'
import { useAuthValidation } from '@/composables/useAuthValidation'

const router = useRouter()
const localePath = useLocalePath()
const { t } = useI18n()
const authStore = useAuthStore()
const form = ref<HTMLFormElement | null>(null)
const values = reactive({ email: '', password: '' })
const { errors, touched, errorMessage, validate, edit, submit, showError } = useAuthValidation(values)

const handleLogin = async (): Promise<void> => {
  if (!await submit(form.value)) return

  try {
    await authStore.login(values.email, values.password)
    await router.push(localePath('/home'))
    toast.success(t('auth.loginSuccess'), { position: 'bottom-right' })
  } catch (error) {
    await showError(error, form.value)
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

            <form ref="form" novalidate @submit.prevent="handleLogin" class="mt-8 space-y-5">
              <div v-if="errorMessage" tabindex="-1" data-error-summary role="alert" class="rounded-2xl border border-red-200 bg-red-50 p-4 text-sm text-red-800 focus-visible:outline-2 focus-visible:outline-red-600">
                <p class="font-medium">{{ errorMessage }}</p>
                <ul v-if="Object.values(errors).some(Boolean)" class="mt-2 space-y-1">
                  <template v-for="(message, field) in errors" :key="field">
                    <li v-if="message">
                      <a :href="`#${field}`" class="underline underline-offset-2" @click.prevent="form?.querySelector<HTMLInputElement>(`#${field}`)?.focus()">{{ message }}</a>
                    </li>
                  </template>
                </ul>
              </div>
              <div>
                <label for="email" class="block text-sm font-medium text-black">
                  {{ $t('auth.emailAddress') }}
                </label>
                <div class="mt-2">
                  <Input
                    v-model="values.email"
                    @blur="touched.email = true; validate('email')"
                    @update:model-value="edit('email')"
                    :aria-invalid="Boolean(errors.email)"
                    :aria-describedby="errors.email ? 'email-error' : undefined"
                    :class="errors.email ? 'border-red-600 bg-red-50 focus:border-red-600 focus-visible:ring-red-600' : ''"
                    id="email"
                    type="email"
                    name="email"
                    required
                    autocomplete="email"
                    class="block h-12 w-full rounded-2xl border border-black/10 bg-neutral-100 px-4 text-black placeholder:text-black/35 focus:border-black focus:bg-white focus:outline-none"
                    :placeholder="$t('auth.emailPlaceholder')"
                  />
                  <p v-if="errors.email" id="email-error" class="mt-2 text-sm text-red-700" aria-live="polite">{{ errors.email }}</p>
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
                    v-model="values.password"
                    @blur="touched.password = true; validate('password')"
                    @update:model-value="edit('password')"
                    :aria-invalid="Boolean(errors.password)"
                    :aria-describedby="errors.password ? 'password-error' : undefined"
                    :class="errors.password ? 'border-red-600 bg-red-50 focus:border-red-600 focus-visible:ring-red-600' : ''"
                    id="password"
                    type="password"
                    name="password"
                    required
                    autocomplete="current-password"
                    class="block h-12 w-full rounded-2xl border border-black/10 bg-neutral-100 px-4 text-black placeholder:text-black/35 focus:border-black focus:bg-white focus:outline-none"
                    :placeholder="$t('auth.passwordPlaceholder')"
                  />
                  <p v-if="errors.password" id="password-error" class="mt-2 text-sm text-red-700" aria-live="polite">{{ errors.password }}</p>
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
