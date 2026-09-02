export type OrderStatusNormalized =
  | 'unpaid'
  | 'payment_expired'
  | 'paid'
  | 'awaiting_shipment'
  | 'shipped'
  | 'buyer_protection'
  | 'completed'
  | 'cancelled'
  | 'disputed'
  | 'failed'
  | 'unknown'

export type PaymentStatusNormalized = 'pending' | 'paid' | 'failed' | 'unknown'

export type ShippingStatusNormalized = 'awaiting_shipment' | 'shipped' | 'buyer_protection' | 'completed' | 'unknown'

export type OrderUserRole = 'buyer' | 'seller' | 'unknown'

export type ShippingAddress = {
  fullName?: string | null
  countryCode?: string | null
  country?: string | null
  city?: string | null
  postalCode?: string | null
  addressLine1?: string | null
  addressLine2?: string | null
  phone?: string | null
}

export type ShippingAddressPayload = {
  fullName: string
  countryCode: string
  country: string
  city: string
  postalCode: string
  addressLine1: string
  addressLine2?: string | null
  phone?: string | null
}

export type MarkOrderShippedPayload = {
  carrierName: string
  trackingNumber: string
}

export type ReportOrderNotDeliveredPayload = {
  reason?: string | null
}

export type OrderDisputeReason =
  | 'ItemNotShipped'
  | 'ItemNotReceived'
  | 'WrongItem'
  | 'SignificantlyNotAsDescribed'
  | 'Damaged'
  | 'SuspectedCounterfeit'

export type OrderDisputeDecision = 'Refund' | 'CompleteOrder'

export type OrderDisputeEvidence = {
  id?: string
  key?: string | null
  createdAtUtc?: string | null
}

export type OrderDispute = {
  id?: string
  orderId?: string
  reason?: OrderDisputeReason | string | number | null
  reasonName?: string | null
  description?: string | null
  status?: string | number | null
  statusName?: string | null
  openedAtUtc?: string | null
  resolvedAtUtc?: string | null
  resolvedByAdminId?: string | null
  decision?: OrderDisputeDecision | string | number | null
  decisionName?: string | null
  decisionReason?: string | null
  evidence?: OrderDisputeEvidence[] | null
}

export type OpenOrderDisputePayload = {
  reason: OrderDisputeReason
  description: string
  evidenceKeys?: string[]
}

export type OrderItem = {
  id?: string
  orderId?: string
  auctionId?: string
  sellerId?: string
  buyerId?: string
  status?: string | number | null
  orderStatus?: string | number | null
  paymentStatus?: string | number | null
  shippingStatus?: string | number | null
  price?: number
  amount?: number
  shippingPrice?: number
  totalPrice?: number
  currency?: string
  shippingCurrency?: string
  shippingOriginCountry?: string
  shipsToCountries?: string
  shippingHandlingDays?: number
  shipByUtc?: string | null
  shippedAtUtc?: string | null
  deliveredAtUtc?: string | null
  completedAtUtc?: string | null
  deliveryIssueAvailableFromUtc?: string | null
  deliveryIssueReportedAtUtc?: string | null
  deliveryIssueReason?: string | null
  carrierName?: string | null
  trackingNumber?: string | null
  canViewShippingAddress?: boolean
  shippingAddress?: ShippingAddress | null
  createdAt?: string | null
  createdDate?: string | null
  paymentDeadlineUtc?: string | null
  paidAtUtc?: string | null
  dispute?: OrderDispute | null
  lotTitle?: string | null
  title?: string | null
  imageUrl?: string | null
  photos?: string[] | null
  media?: Array<{ url?: string | null }>
  [key: string]: unknown
}

export type OrderDetails = OrderItem
