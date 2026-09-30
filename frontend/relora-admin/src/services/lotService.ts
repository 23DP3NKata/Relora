import api from '@/api'
import type { LotPreview } from '@/types/lot'

export const adminLotService = {
  async getPendingLots(): Promise<LotPreview[]> {
    const response = await api.get<LotPreview[]>('/api/admin/lots/pending')
    return response.data
  },
}
