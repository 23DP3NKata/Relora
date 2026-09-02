import api from '@/api'
import type { LotPreview } from '@/types/lot'

const normalizeStatus = (lot: LotPreview): string => {
  return String(lot.statusName ?? lot.status ?? '').toLowerCase()
}

export const adminLotService = {
  async getPendingLots(): Promise<LotPreview[]> {
    try {
      const response = await api.get<LotPreview[]>('/api/items', {
        params: {
          Status: 'Pending',
        },
      })

      return response.data.filter((lot) => {
        const status = normalizeStatus(lot)
        return status.includes('pending') || status.includes('draft')
      })
    } catch {
      const fallbackResponse = await api.get<LotPreview[]>('/api/items')

      return fallbackResponse.data.filter((lot) => {
        const status = normalizeStatus(lot)
        return status.includes('pending') || status.includes('draft')
      })
    }
  },
}
