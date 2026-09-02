import { defineStore } from 'pinia'
import { ref } from 'vue'
import api from '@/api'

export interface User {
  userId: string
  email: string
  username: string
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const isAuthenticated = ref(false)
  const isLoading = ref(false)

  function setUser(userData: User): void {
    user.value = userData
    isAuthenticated.value = true
  }

  function clearAuth(): void {
    user.value = null
    isAuthenticated.value = false
  }

  async function login(email: string, password: string): Promise<void> {
    await api.post('/api/auth/login', {
      email,
      password,
    })

    await checkAuth()
  }

  async function register(username: string, email: string, password: string, confirmPassword: string): Promise<void> {
    await api.post('/api/auth/register', {
      username,
      email,
      password,
      confirmPassword,
    })

    await checkAuth()
  }

  async function logout(): Promise<void> {
    try 
    {
      await api.post('/api/auth/logout')
      clearAuth()
    } 
    catch (error) 
    {
      console.error('Logout failed:', error)
    }
    finally
    {
      
    }
  }

  async function checkAuth(): Promise<boolean> {
    isLoading.value = true
    
    try 
    {
      const response = await api.get<User>('/api/auth/me')
      setUser(response.data)
      return true
    } 
    catch (error) 
    {
      clearAuth()
      return false
    } 
    finally 
    {
      isLoading.value = false
    }
  }

  return {
    user,
    isAuthenticated,
    isLoading,
    setUser,
    clearAuth,
    login,
    register,
    logout,
    checkAuth,
  }
})