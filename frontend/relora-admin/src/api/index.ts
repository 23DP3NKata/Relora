import axios, { type AxiosError, type InternalAxiosRequestConfig } from 'axios'

interface RetryableRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean
}

const api = axios.create({
  baseURL: 'https://localhost:7001',
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

    if (isUnauthorized && !originalRequest._retry && !isRefreshRequest) {
      originalRequest._retry = true

      try {
        await api.post('/api/auth/refresh')
        return api(originalRequest)
      } catch (refreshError) {
        return Promise.reject(refreshError)
      }
    }

    return Promise.reject(error)
  },
)

export default api
