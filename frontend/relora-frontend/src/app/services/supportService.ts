import api from '@/api'

export interface CreateSupportRequestPayload {
  email: string
  category: string
  subject: string
  message: string
}

export const supportService = {
  async createRequest(payload: CreateSupportRequestPayload): Promise<void> {
    await api.post('/api/support/requests', payload)
  },
}
