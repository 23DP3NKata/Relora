import api from '@/api'
import type { LotPreview, PendingLotDetails } from '@/types/lot'

export const adminLotService = {
  async getPendingLots(): Promise<LotPreview[]> {
    const response = await api.get<LotPreview[]>('/api/admin/lots/pending')
    return response.data
  },

  async getPendingLotDetails(lotId: string): Promise<PendingLotDetails> {
    const response = await api.get<PendingLotDetails>(`/api/admin/lots/pending/${lotId}`)
    return response.data
  },
}
