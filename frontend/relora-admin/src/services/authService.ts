import { ref } from 'vue'

import api from '@/api'

export type CurrentUser = {
  userId: string
  username: string
  email: string
  isAdmin: boolean
}

// kopīgs visai lietotnei, lai header zina, kas ir ielogojies
export const currentUser = ref<CurrentUser | null>(null)

export const authService = {
  async login(email: string, password: string): Promise<void> {
    await api.post('/api/auth/login', { email, password })
  },

  async logout(): Promise<void> {
    try {
      await api.post('/api/auth/logout')
    } finally {
      currentUser.value = null
    }
  },

  async loadCurrentUser(): Promise<CurrentUser | null> {
    try {
      const response = await api.get<CurrentUser>('/api/auth/me')
      currentUser.value = response.data
    } catch {
      currentUser.value = null
    }

    return currentUser.value
  },
}
