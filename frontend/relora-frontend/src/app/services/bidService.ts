import api from '@/api'

export type Bid = {
  bidId: string
  userId: string
  amount: number
  currency: string
  status: string
  placedAt: string
}

export type PlaceBidPayload = {
  amount: number
  currency: string
}

export const bidService = {
  async getBidsByAuction(auctionId: string): Promise<Bid[]> {
    const response = await api.get<Bid[]>(`/api/bids/${auctionId}`)
    return response.data
  },

  async placeBid(auctionId: string, payload: PlaceBidPayload): Promise<void> {
    await api.post(`/api/bids/place/${auctionId}`, payload)
  },
}
