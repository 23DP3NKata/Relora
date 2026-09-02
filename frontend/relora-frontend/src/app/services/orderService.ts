import api from '@/api'
import type {
  MarkOrderShippedPayload,
  OpenOrderDisputePayload,
  OrderDetails,
  OrderItem,
  ReportOrderNotDeliveredPayload,
  ShippingAddressPayload,
} from '@/types/order'

export const orderService = {
  async getMyOrders(): Promise<OrderItem[]> {
    const response = await api.get<OrderItem[]>('/api/orders/my')
    return response.data ?? []
  },

  async getOrderDetails(orderId: string): Promise<OrderDetails> {
    const response = await api.get<OrderDetails>(`/api/orders/details/${orderId}`)
    return response.data
  },

  async createCheckout(orderId: string, shippingAddress: ShippingAddressPayload): Promise<string> {
    const response = await api.post<{ url: string }>(
      `/orders/${orderId}/checkout`,
      shippingAddress,
    )

    return response.data.url
  },

  async markOrderShipped(orderId: string, payload: MarkOrderShippedPayload): Promise<void> {
    await api.post(`/api/orders/${orderId}/shipment`, payload)
  },

  async confirmOrderReceived(orderId: string): Promise<void> {
    await api.post(`/api/orders/${orderId}/received`)
  },

  async reportNotDelivered(orderId: string, payload: ReportOrderNotDeliveredPayload): Promise<void> {
    await api.post(`/api/orders/${orderId}/not-delivered`, payload)
  },

  async openDispute(orderId: string, payload: OpenOrderDisputePayload): Promise<string> {
    const response = await api.post<{ disputeId: string }>(
      `/api/orders/${orderId}/problem`,
      payload,
    )

    return response.data.disputeId
  },
}
