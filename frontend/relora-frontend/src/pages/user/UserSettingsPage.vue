<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, onMounted, reactive, ref } from 'vue'
import { onBeforeRouteLeave, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { isAxiosError } from 'axios'
import api from '@/api'
import { useAuthStore, type User } from '@/stores/authStore'
import { useLocalePath } from '@/composables/useLocalePath'
import Container from '@/components/ui/Container.vue'
import LanguageSwitcher from '@/components/layout/LanguageSwitcher.vue'
import ModeToggle from '@/components/theme/ModeToggle.vue'

interface Settings extends User { name: string }
type Section = 'profile' | 'password'
const { t } = useI18n()
const auth = useAuthStore()
const router = useRouter()
const localePath = useLocalePath()
const saved = ref<Settings | null>(null)
const profile = reactive({ name: '', username: '' })
const password = reactive({ currentPassword: '', newPassword: '', confirmPassword: '' })
const errors = reactive<Record<Section, Record<string, string>>>({ profile: {}, password: {} })
const message = reactive({ profile: '', password: '' })
const loading = ref(true)
const loadFailed = ref(false)
const busy = ref<Section | null>(null)
const showPasswords = ref(false)
const passwordChanged = ref(false)
const profileDirty = computed(() => !!saved.value && (profile.name !== saved.value.name || profile.username !== saved.value.username))
const passwordDirty = computed(() => Object.values(password).some(Boolean))
const dirty = computed(() => profileDirty.value || passwordDirty.value)
const fields = ['currentPassword', 'newPassword', 'confirmPassword'] as const

async function load() {
  loading.value = true
  loadFailed.value = false
  try {
    saved.value = (await api.get<Settings>('/api/settings')).data
    resetProfile()
  } catch {
    loadFailed.value = true
  } finally {
    loading.value = false
  }
}

function resetProfile() {
  if (!saved.value) return
  profile.name = saved.value.name
  profile.username = saved.value.username
  errors.profile = {}
  message.profile = ''
}

function edit(section: Section, field: string) {
  delete errors[section][field]
  delete errors[section].general
  message[section] = ''
}

async function focusErrors(section: Section) {
  await nextTick()
  document.getElementById(`${section}-errors`)?.focus()
}

async function save(section: Section) {
  if (busy.value) return
  errors[section] = {}
  message[section] = ''
  if (section === 'profile') {
    profile.name = profile.name.trim()
    profile.username = profile.username.trim()
    if (!profile.name || profile.name.length > 100 || /[\u0000-\u001f\u007f-\u009f]/.test(profile.name)) errors.profile.name = 'invalidName'
    if (profile.username.length < 3 || profile.username.length > 20 || /[\s\u0000-\u001f\u007f-\u009f/?#%\\]/.test(profile.username)) errors.profile.username = 'invalidUsername'
  } else {
    if (!password.currentPassword || password.currentPassword.length > 256) errors.password.currentPassword = 'requiredPassword'
    if (password.newPassword.length < 8 || !password.newPassword.trim() || new TextEncoder().encode(password.newPassword).length > 72) errors.password.newPassword = 'invalidPassword'
    else if (password.newPassword === password.currentPassword) errors.password.newPassword = 'samePassword'
    if (password.newPassword !== password.confirmPassword || !password.confirmPassword) errors.password.confirmPassword = 'passwordMismatch'
  }
  if (Object.keys(errors[section]).length) return focusErrors(section)
  busy.value = section
  try {
    if (section === 'profile') {
      saved.value = (await api.put<Settings>('/api/settings/profile', profile)).data
      auth.setUser(saved.value)
      resetProfile()
      message.profile = 'saved'
    } else {
      await api.put('/api/settings/password', password)
      password.currentPassword = password.newPassword = password.confirmPassword = ''
      showPasswords.value = false
      auth.clearAuth()
      passwordChanged.value = true
      await nextTick()
      document.getElementById('password-changed')?.focus()
    }
  } catch (error) {
    const data = isAxiosError(error) ? error.response?.data : null
    const known = ['invalidName', 'invalidUsername', 'usernameTaken', 'invalidPassword', 'passwordMismatch', 'samePassword', 'incorrectPassword']
    for (const [field, values] of Object.entries(data?.errors ?? {})) {
      const key = field.charAt(0).toLowerCase() + field.slice(1)
      const value = Array.isArray(values) ? values[0] : null
      if (key in (section === 'profile' ? profile : password) && known.includes(value)) errors[section][key] = value
    }
    if (!Object.keys(errors[section]).length) {
      errors[section].general = isAxiosError(error) && error.response?.status === 429 ? 'tooManyRequests' : 'saveFailed'
    }
    await focusErrors(section)
  } finally {
    busy.value = null
  }
}

function beforeUnload(event: BeforeUnloadEvent) {
  if (!dirty.value && !busy.value) return
  event.preventDefault()
  event.returnValue = ''
}
onBeforeRouteLeave(() => !busy.value && (!dirty.value || window.confirm(t('settings.leaveConfirm'))))
onMounted(() => { load(); window.addEventListener('beforeunload', beforeUnload) })
onBeforeUnmount(() => window.removeEventListener('beforeunload', beforeUnload))
</script>

<template>
  <Container class="py-8 md:py-12">
    <div class="mx-auto max-w-4xl space-y-8">
      <header>
        <p class="text-sm text-muted-foreground">{{ t('navigation.myAccount') }}</p>
        <h1 class="mt-2 text-3xl font-semibold tracking-tight md:text-4xl">{{ t('settings.title') }}</h1>
        <p class="mt-3 max-w-xl text-muted-foreground">{{ t('settings.description') }}</p>
      </header>
      <p v-if="loading" role="status">{{ t('settings.loading') }}</p>
      <div v-else-if="loadFailed" role="alert" class="settings-card space-y-4">
        <p>{{ t('settings.loadFailed') }}</p>
        <button class="settings-button" @click="load">{{ t('settings.retry') }}</button>
      </div>
      <section v-else-if="passwordChanged" id="password-changed" tabindex="-1" class="settings-card space-y-4" role="status">
        <h2 class="text-xl font-semibold">{{ t('settings.passwordChanged') }}</h2>
        <p class="text-muted-foreground">{{ t('settings.signInAgain') }}</p>
        <button class="settings-button primary" @click="router.push(localePath('/login'))">{{ t('navigation.signIn') }}</button>
      </section>
      <template v-else-if="saved">
        <form class="settings-card" novalidate @submit.prevent="save('profile')">
          <h2 class="text-xl font-semibold">{{ t('settings.profile') }}</h2>
          <p class="mt-2 text-sm text-muted-foreground">{{ t('settings.profileHint') }}</p>
          <div v-if="Object.keys(errors.profile).length" id="profile-errors" tabindex="-1" role="alert" class="error-summary">
            <p class="font-medium">{{ t('settings.checkFields') }}</p>
            <p v-for="(error, field) in errors.profile" :key="field">
              <a v-if="field !== 'general'" :href="`#${field}`" class="underline">{{ t(`settings.${error}`) }}</a>
              <span v-else>{{ t(`settings.${error}`) }}</span>
            </p>
          </div>
          <fieldset :disabled="!!busy" class="mt-6 space-y-5">
            <div class="grid gap-5 sm:grid-cols-2">
              <div v-for="field in (['name', 'username'] as const)" :key="field" class="space-y-2">
                <label :for="field" class="block text-sm font-medium">{{ t(`settings.${field}`) }}</label>
                <input :id="field" v-model="profile[field]" class="settings-input" :maxlength="field === 'name' ? 100 : 20" :autocomplete="field === 'name' ? 'name' : 'username'" required :aria-invalid="!!errors.profile[field]" :aria-describedby="`${field}-hint ${field}-error`" @input="edit('profile', field)" />
                <p :id="`${field}-hint`" class="text-sm text-muted-foreground">{{ t(`settings.${field}Hint`) }}</p>
                <p :id="`${field}-error`" class="text-sm text-destructive">{{ errors.profile[field] ? t(`settings.${errors.profile[field]}`) : '' }}</p>
              </div>
            </div>
            <div class="space-y-2">
              <label for="email" class="block text-sm font-medium">{{ t('settings.email') }}</label>
              <input id="email" :value="saved.email" readonly class="settings-input bg-muted" aria-describedby="email-hint" />
              <p id="email-hint" class="text-sm text-muted-foreground">{{ t('settings.emailHint') }}</p>
            </div>
            <div class="flex flex-wrap items-center gap-3 border-t border-border pt-5">
              <button class="settings-button primary" :disabled="!profileDirty">{{ t(busy === 'profile' ? 'settings.saving' : 'settings.save') }}</button>
              <button type="button" class="settings-button" :disabled="!profileDirty" @click="resetProfile">{{ t('settings.cancel') }}</button>
              <p role="status" class="text-sm">{{ message.profile ? t(`settings.${message.profile}`) : profileDirty ? t('settings.unsaved') : '' }}</p>
            </div>
          </fieldset>
        </form>
        <form class="settings-card" novalidate @submit.prevent="save('password')">
          <h2 class="text-xl font-semibold">{{ t('settings.security') }}</h2>
          <p class="mt-2 text-sm text-muted-foreground">{{ t('settings.securityHint') }}</p>
          <div v-if="Object.keys(errors.password).length" id="password-errors" tabindex="-1" role="alert" class="error-summary">
            <p class="font-medium">{{ t('settings.checkFields') }}</p>
            <p v-for="(error, field) in errors.password" :key="field">
              <a v-if="field !== 'general'" :href="`#${field}`" class="underline">{{ t(`settings.${error}`) }}</a>
              <span v-else>{{ t(`settings.${error}`) }}</span>
            </p>
          </div>
          <fieldset :disabled="!!busy" class="mt-6 space-y-5">
            <div v-for="field in fields" :key="field" class="max-w-lg space-y-2">
              <label :for="field" class="block text-sm font-medium">{{ t(`settings.${field}`) }}</label>
              <input :id="field" v-model="password[field]" :type="showPasswords ? 'text' : 'password'" :autocomplete="field === 'currentPassword' ? 'current-password' : 'new-password'" :maxlength="field === 'currentPassword' ? 256 : 72" class="settings-input" required :aria-invalid="!!errors.password[field]" :aria-describedby="`${field}-error${field === 'newPassword' ? ' password-hint' : ''}`" @input="edit('password', field)" />
              <p v-if="field === 'newPassword'" id="password-hint" class="text-sm text-muted-foreground">{{ t('settings.passwordHint') }}</p>
              <p :id="`${field}-error`" class="text-sm text-destructive">{{ errors.password[field] ? t(`settings.${errors.password[field]}`) : '' }}</p>
            </div>
            <label class="flex min-h-11 w-fit cursor-pointer items-center gap-3 text-sm"><input v-model="showPasswords" type="checkbox" class="h-4 w-4" />{{ t('settings.showPasswords') }}</label>
            <p v-if="profileDirty" class="text-sm text-muted-foreground">{{ t('settings.saveProfileFirst') }}</p>
            <button class="settings-button primary" :disabled="!passwordDirty || profileDirty">{{ t(busy === 'password' ? 'settings.saving' : 'settings.changePassword') }}</button>
          </fieldset>
        </form>
        <section class="settings-card">
          <h2 class="text-xl font-semibold">{{ t('settings.preferences') }}</h2>
          <p class="mt-2 text-sm text-muted-foreground">{{ t('settings.preferencesHint') }}</p>
          <div class="mt-6 flex flex-wrap gap-8">
            <div class="space-y-2"><p class="text-sm font-medium">{{ t('settings.language') }}</p><LanguageSwitcher /></div>
            <div class="space-y-2"><p class="text-sm font-medium">{{ t('settings.appearance') }}</p><ModeToggle /></div>
          </div>
        </section>
      </template>
    </div>
  </Container>
</template>

<style scoped>
@reference "../../style.css";
.settings-card { @apply rounded-2xl border border-border bg-background p-5 sm:p-8; }
.settings-input { @apply min-h-11 w-full rounded-lg border border-input px-3 py-2 text-base outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:opacity-60; }
.settings-input[aria-invalid="true"] { @apply border-destructive; }
.settings-button { @apply inline-flex min-h-11 items-center justify-center rounded-full border border-border px-5 py-2 text-sm font-medium hover:bg-accent focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-ring disabled:cursor-not-allowed disabled:opacity-50; }
.primary { @apply bg-primary text-primary-foreground hover:bg-primary/90; }
.error-summary { @apply mt-5 space-y-2 rounded-lg border border-destructive p-4 text-sm text-destructive; }
</style>
