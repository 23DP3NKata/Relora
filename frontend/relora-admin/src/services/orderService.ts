import api from '@/api'
import type { OverdueShipment } from '@/types/order'

export const orderService = {
  async getOverdueShipments(): Promise<OverdueShipment[]> {
    const response = await api.get<OverdueShipment[]>('/api/admin/orders/overdue-shipments')
    return response.data
  },
}
