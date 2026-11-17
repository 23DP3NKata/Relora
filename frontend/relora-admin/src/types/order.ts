export type OverdueShipment = {
  orderId: string
  auctionId: string
  lotTitle: string | null
  lotBrand: string | null
  statusName: string
  sellerId: string
  buyerId: string
  totalPrice: number
  currency: string
  paidAtUtc: string | null
  shipByUtc: string | null
  daysOverdue: number
}
