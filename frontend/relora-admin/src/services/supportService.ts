import api from '@/api'
import type { SupportRequest, SupportRequestStatus } from '@/types/support'

export const supportService = {
  async getSupportRequests(): Promise<SupportRequest[]> {
    const response = await api.get<SupportRequest[]>('/api/admin/support-requests')
    return response.data
  },

  async changeStatus(requestId: string, status: SupportRequestStatus): Promise<void> {
    await api.post(`/api/admin/support-requests/${requestId}/status`, { status })
  },
}
