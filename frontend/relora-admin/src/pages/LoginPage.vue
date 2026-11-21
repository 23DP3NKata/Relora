<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'

import LanguageSwitcher from '@/components/LanguageSwitcher.vue'
import { authService } from '@/services/authService'

const route = useRoute()
const router = useRouter()
const { t } = useI18n()

const email = ref('')
const password = ref('')
const isLoading = ref(false)
const errorMessage = ref('')

const submit = async () => {
  if (!email.value.trim() || !password.value) {
    errorMessage.value = t('login.fillFields')
    return
  }

  try {
    isLoading.value = true
    errorMessage.value = ''

    await authService.login(email.value.trim(), password.value)
    const user = await authService.loadCurrentUser()

    // parasts lietotājs var ielogoties, bet admin panelī viņu nelaižam
    if (!user || !user.isAdmin) {
      await authService.logout()
      errorMessage.value = t('login.notAdmin')
      return
    }

    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : '/admin/dashboard'
    router.push(redirect)
  } catch (error) {
    console.error(error)
    errorMessage.value = t('login.failed')
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <section class="flex min-h-screen items-center justify-center px-4 py-10">
    <div class="w-full max-w-sm">
      <div class="mb-8 flex items-center justify-between">
        <div class="flex items-baseline gap-2">
          <span class="text-2xl font-semibold tracking-tight">Relora</span>
          <span class="eyebrow">{{ $t('admin.badge') }}</span>
        </div>
        <LanguageSwitcher />
      </div>

      <form class="card p-6" @submit.prevent="submit">
        <p class="eyebrow">{{ $t('login.eyebrow') }}</p>
        <h1 class="mt-2 text-2xl font-semibold tracking-tight">{{ $t('login.title') }}</h1>
        <p class="mt-2 text-sm text-neutral-500">{{ $t('login.description') }}</p>

        <label for="login-email" class="mt-6 block text-sm font-medium">{{ $t('login.email') }}</label>
        <input
          id="login-email"
          v-model="email"
          type="email"
          autocomplete="username"
          class="field mt-2"
          :disabled="isLoading"
        />

        <label for="login-password" class="mt-4 block text-sm font-medium">{{ $t('login.password') }}</label>
        <input
          id="login-password"
          v-model="password"
          type="password"
          autocomplete="current-password"
          class="field mt-2"
          :disabled="isLoading"
        />

        <p v-if="errorMessage" class="mt-4 text-sm text-red-600">{{ errorMessage }}</p>

        <button type="submit" class="btn-primary mt-6 w-full" :disabled="isLoading">
          {{ isLoading ? $t('login.signingIn') : $t('login.signIn') }}
        </button>
      </form>
    </div>
  </section>
</template>
