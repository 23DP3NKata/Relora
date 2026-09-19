import { nextTick, reactive, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import axios from 'axios'

export function useAuthValidation(values: Record<string, string>) {
  const { t } = useI18n()
  const errors = reactive<Record<string, string>>({})
  const touched = reactive<Record<string, boolean>>({})
  const errorMessage = ref('')

  function validate(field: string) {
    const value = values[field] ?? ''
    let key = ''
    if (field === 'email') {
      if (!value.trim()) key = 'emailRequired'
      else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value.trim())) key = 'emailInvalid'
    } else if (field === 'username') {
      if (!value.trim()) key = 'usernameRequired'
      else if (value.trim().length < 3 || value.trim().length > 20) key = 'usernameLength'
    } else if (field === 'password') {
      if (!value.trim()) key = 'passwordRequired'
      else if (value.length < 6) key = 'passwordLength'
    } else if (field === 'confirmPassword') {
      if (!value) key = 'confirmRequired'
      else if (value !== values.password) key = 'passwordMismatch'
    }
    errors[field] = key ? t(`auth.${key}`) : ''
  }

  async function edit(field: string) {
    errorMessage.value = ''
    await nextTick()
    if (touched[field]) validate(field)
    if (field === 'password' && touched.confirmPassword) validate('confirmPassword')
  }

  async function focusSummary(form: HTMLFormElement | null) {
    await nextTick()
    form?.querySelector<HTMLElement>('[data-error-summary]')?.focus()
  }

  async function submit(form: HTMLFormElement | null) {
    errorMessage.value = ''
    values.email = values.email?.trim() ?? ''
    if ('username' in values) values.username = values.username?.trim() ?? ''
    for (const field of Object.keys(values)) {
      touched[field] = true
      validate(field)
    }
    if (Object.values(errors).some(Boolean)) {
      errorMessage.value = t('auth.checkFields')
      await focusSummary(form)
      return false
    }
    return true
  }

  async function showError(error: unknown, form: HTMLFormElement | null) {
    errorMessage.value = t('auth.unexpectedError')
    if (axios.isAxiosError(error)) {
      const status = error.response?.status
      const data = error.response?.data
      if (!error.response) errorMessage.value = t('auth.networkError')
      else if (status === 401) errorMessage.value = t('auth.invalidCredentials')
      else if (status === 429) errorMessage.value = t('auth.tooManyAttempts')
      else if (status && status >= 500) errorMessage.value = t('auth.serviceUnavailable')
      else {
        const message = data?.message ?? data?.detail
        if (typeof message === 'string' && message.trim()) errorMessage.value = message
        if (status === 409 && message === 'A user with this email already exists.') {
          errors.email = t('auth.emailTaken')
          errorMessage.value = errors.email
        } else if (status === 409 && message === 'This username is already taken.') {
          errors.username = t('auth.usernameTaken')
          errorMessage.value = errors.username
        }
        if (data?.errors && typeof data.errors === 'object') {
          for (const [name, messages] of Object.entries(data.errors)) {
            const field = Object.keys(values).find(key => key.toLowerCase() === name.toLowerCase())
            if (field && Array.isArray(messages)) {
              errors[field] = messages.filter(message => typeof message === 'string').join(' ')
            }
          }
          if (Object.values(errors).some(Boolean)) errorMessage.value = t('auth.checkFields')
        }
      }
    }
    await focusSummary(form)
  }

  return { errors, touched, errorMessage, validate, edit, submit, showError }
}
