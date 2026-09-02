import type {
  OrderItem,
  OrderStatusNormalized,
  OrderUserRole,
  PaymentStatusNormalized,
  ShippingStatusNormalized,
} from '@/types/order'

const statusMapByNumber: Record<number, OrderStatusNormalized> = {
  0: 'unpaid',
  1: 'payment_expired',
  2: 'paid',
  3: 'awaiting_shipment',
  4: 'shipped',
  5: 'shipped',
  6: 'buyer_protection',
  7: 'paid',
  8: 'completed',
  9: 'cancelled',
  10: 'disputed',
  11: 'cancelled',
  12: 'failed',
}

export function normalizeOrderStatus(status: string | number | null | undefined): OrderStatusNormalized {
  if (typeof status === 'number') {
    return statusMapByNumber[status] ?? 'unknown'
  }

  const value = String(status ?? '').toLowerCase().trim()
  if (!value) return 'unknown'
  if (value.includes('buyerprotection') || value.includes('buyer protection')) return 'buyer_protection'
  if (value.includes('disputed')) return 'disputed'
  if (value.includes('paymentexpired') || value.includes('payment expired')) return 'payment_expired'
  if (value.includes('pending') || value.includes('unpaid')) return 'unpaid'
  if (value.includes('awaitingshipment') || value.includes('awaiting shipment')) return 'awaiting_shipment'
  if (value.includes('paid')) return 'paid'
  if (value.includes('ship')) return 'shipped'
  if (value.includes('complete')) return 'completed'
  if (value.includes('cancel') || value.includes('refund')) return 'cancelled'
  if (value.includes('failed') || value.includes('expired')) return 'failed'
  return 'unknown'
}

export function normalizePaymentStatus(status: string | number | null | undefined): PaymentStatusNormalized {
  if (typeof status === 'number') {
    if (status === 1) return 'paid'
    if (status === 2 || status === 3 || status === 4 || status === 5) return 'failed'
    if (status === 0) return 'pending'
  }

  const value = String(status ?? '').toLowerCase().trim()
  if (!value) return 'unknown'
  if (value.includes('paid')) return 'paid'
  if (value.includes('pending') || value.includes('unpaid')) return 'pending'
  if (value.includes('failed') || value.includes('cancel') || value.includes('expired')) return 'failed'
  return 'unknown'
}

export function normalizeShippingStatus(status: string | number | null | undefined): ShippingStatusNormalized {
  if (typeof status === 'number') {
    if (status >= 3) return 'completed'
    if (status === 2) return 'shipped'
    return 'awaiting_shipment'
  }

  const value = String(status ?? '').toLowerCase().trim()
  if (!value) return 'unknown'
  if (value.includes('complete')) return 'completed'
  if (value.includes('buyerprotection') || value.includes('buyer protection') || value.includes('disputed')) return 'buyer_protection'
  if (value.includes('ship')) return 'shipped'
  if (value.includes('paid') || value.includes('pending') || value.includes('await')) return 'awaiting_shipment'
  return 'unknown'
}

export function getUserRoleInOrder(order: OrderItem, userId?: string | null): OrderUserRole {
  if (!userId) return 'unknown'

  const buyerId = String(order.buyerId ?? '')
  const sellerId = String(order.sellerId ?? '')

  if (buyerId && buyerId === userId) return 'buyer'
  if (sellerId && sellerId === userId) return 'seller'
  return 'unknown'
}

export function getOrderActionHint(order: OrderItem, role: OrderUserRole): string {
  const orderStatus = normalizeOrderStatus(order.status ?? order.orderStatus)
  const paymentStatus = normalizePaymentStatus(order.paymentStatus ?? order.status ?? order.orderStatus)
  const shippingStatus = normalizeShippingStatus(order.shippingStatus ?? order.status ?? order.orderStatus)

  if (role === 'buyer') {
    if (orderStatus === 'unpaid' || paymentStatus === 'pending') return 'Action needed: complete payment'
    if (orderStatus === 'payment_expired') return 'Payment deadline expired'
    if (orderStatus === 'buyer_protection' || orderStatus === 'disputed') return 'Dispute under review'
    if (orderStatus === 'paid' || shippingStatus === 'awaiting_shipment') return 'Waiting for seller to ship'
    if (orderStatus === 'shipped' || shippingStatus === 'shipped') return 'Item shipped'
    if (orderStatus === 'completed' || shippingStatus === 'completed') return 'Order completed'
    if (orderStatus === 'cancelled') return 'Order cancelled'
    if (orderStatus === 'failed') return 'Order failed'
  }

  if (role === 'seller') {
    if (orderStatus === 'unpaid' || paymentStatus === 'pending') return 'Waiting for buyer payment'
    if (orderStatus === 'payment_expired') return 'Payment deadline expired'
    if (orderStatus === 'buyer_protection' || orderStatus === 'disputed') return 'Dispute under review'
    if (orderStatus === 'paid' || shippingStatus === 'awaiting_shipment') return 'Action needed: ship item'
    if (orderStatus === 'shipped' || shippingStatus === 'shipped') return 'Item shipped'
    if (orderStatus === 'completed' || shippingStatus === 'completed') return 'Order completed'
    if (orderStatus === 'cancelled') return 'Order cancelled'
    if (orderStatus === 'failed') return 'Order failed'
  }

  return 'No action available'
}

export function getOrderBadgeClass(order: OrderItem): string {
  const orderStatus = normalizeOrderStatus(order.status ?? order.orderStatus)
  const paymentStatus = normalizePaymentStatus(order.paymentStatus ?? order.status ?? order.orderStatus)
  const shippingStatus = normalizeShippingStatus(order.shippingStatus ?? order.status ?? order.orderStatus)

  if (orderStatus === 'buyer_protection' || orderStatus === 'disputed') {
    return 'bg-orange-100 text-orange-800 border-orange-300'
  }

  if (orderStatus === 'cancelled' || orderStatus === 'failed' || orderStatus === 'payment_expired' || paymentStatus === 'failed') {
    return 'bg-red-100 text-red-700 border-red-300'
  }

  if (orderStatus === 'completed' || shippingStatus === 'completed') {
    return 'bg-emerald-100 text-emerald-700 border-emerald-300'
  }

  if (orderStatus === 'shipped' || shippingStatus === 'shipped') {
    return 'bg-blue-100 text-blue-700 border-blue-300'
  }

  if (orderStatus === 'paid' || paymentStatus === 'paid') {
    return 'bg-emerald-100 text-emerald-700 border-emerald-300'
  }

  if (orderStatus === 'unpaid' || paymentStatus === 'pending' || shippingStatus === 'awaiting_shipment') {
    return 'bg-amber-100 text-amber-800 border-amber-300'
  }

  return 'bg-gray-100 text-gray-700 border-gray-300'
}

export function getOrderStatusLabel(order: OrderItem): string {
  const orderStatus = normalizeOrderStatus(order.status ?? order.orderStatus)
  const paymentStatus = normalizePaymentStatus(order.paymentStatus ?? order.status ?? order.orderStatus)
  const shippingStatus = normalizeShippingStatus(order.shippingStatus ?? order.status ?? order.orderStatus)

  if (orderStatus === 'cancelled') return 'Cancelled'
  if (orderStatus === 'payment_expired') return 'Payment expired'
  if (orderStatus === 'buyer_protection' || orderStatus === 'disputed') return 'Disputed'
  if (orderStatus === 'failed' || paymentStatus === 'failed') return 'Failed'
  if (orderStatus === 'completed' || shippingStatus === 'completed') return 'Completed'
  if (orderStatus === 'shipped' || shippingStatus === 'shipped') return 'Shipped'
  if (orderStatus === 'paid' || paymentStatus === 'paid') return 'Paid'
  if (shippingStatus === 'awaiting_shipment') return 'Awaiting shipment'
  if (orderStatus === 'unpaid' || paymentStatus === 'pending') return 'Pending payment'
  return 'Pending'
}
