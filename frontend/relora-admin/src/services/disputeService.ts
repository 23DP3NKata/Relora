import api from '@/api'
import type { DisputeDecision, OrderDispute } from '@/types/dispute'

export const disputeService = {
  async getDisputes(): Promise<OrderDispute[]> {
    const response = await api.get<OrderDispute[]>('/api/admin/disputes')
    return response.data
  },

  async resolveDispute(disputeId: string, decision: DisputeDecision, reason: string): Promise<void> {
    await api.post(`/api/admin/disputes/${disputeId}/resolve`, { decision, reason })
  },
}
