export type DisputeEvidence = {
  id: string
  key: string
  createdAtUtc: string
}

export type OrderDispute = {
  disputeId: string
  orderId: string
  auctionId: string
  lotId: string | null
  lotTitle: string | null
  lotBrand: string | null
  lotPhotoKey: string | null
  sellerId: string
  buyerId: string
  totalPrice: number
  currency: string
  orderStatusName: string
  reasonName: string
  description: string
  disputeStatusName: string
  openedAtUtc: string
  resolvedAtUtc: string | null
  resolvedByAdminId: string | null
  decisionName: string | null
  decisionReason: string | null
  payoutBlocked: boolean
  shippedAtUtc: string | null
  carrierName: string | null
  trackingNumber: string | null
  evidence: DisputeEvidence[]
}

// CancelOrder - pircējam taisnība, CompleteOrder - pārdevējam
export type DisputeDecision = 'CancelOrder' | 'CompleteOrder'
