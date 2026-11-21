import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios'

interface RetryableRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:7000'

const api = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
})

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as RetryableRequestConfig

    if (!originalRequest) {
      return Promise.reject(error)
    }

    const isUnauthorized = error.response?.status === 401
    const isRefreshRequest = originalRequest.url?.includes('/api/auth/refresh')
    // nepareiza parole arī dod 401, tad nav ko atjaunot
    const isLoginRequest = originalRequest.url?.includes('/api/auth/login')

    if (isUnauthorized && !originalRequest._retry && !isRefreshRequest && !isLoginRequest) {
      originalRequest._retry = true

      try {
        await api.post('/api/auth/refresh')
        return api(originalRequest)
      } catch (refreshError) {
        // sesija beigusies, sūtam atpakaļ uz login (bet ne tad, kad tikai pārbaudām /me)
        if (!originalRequest.url?.includes('/api/auth/me')) {
          const { default: router } = await import('@/router')
          if (router.currentRoute.value.path !== '/login') {
            router.push({ path: '/login', query: { redirect: router.currentRoute.value.fullPath } })
          }
        }
        return Promise.reject(refreshError)
      }
    }

    return Promise.reject(error)
  },
)

export default api
