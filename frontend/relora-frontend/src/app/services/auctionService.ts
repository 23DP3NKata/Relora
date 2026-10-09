import api from '@/api'
import type {
  AuctionDetails,
  AuctionDurationOption,
  AuctionListItem,
  CreateAuctionPayload,
  StartAuctionDuration,
} from '@/types/auction'

const durationMap: Record<StartAuctionDuration, AuctionDurationOption> = {
  '3': 'Classic3Days',
  '5': 'Classic7Days',
  '7': 'Classic7Days',
}

export const auctionService = {
  async getAuctionDetails(auctionId: string): Promise<AuctionDetails> {
    const response = await api.get<AuctionDetails>(`/api/auctions/${auctionId}`)
    return response.data
  },

  async getAuctionByLotId(lotId: string): Promise<AuctionDetails> {
    const response = await api.get<AuctionDetails>(`/api/auctions/lot/${lotId}`)
    return response.data
  },

  async getAuctions(filters?: Record<string, string | number | undefined>): Promise<AuctionListItem[]> {
    const response = await api.get<AuctionListItem[]>('/api/auctions', {
      params: filters,
    })
    return response.data
  },

  async createAuction(payload: CreateAuctionPayload): Promise<string> {
    const response = await api.post<string>('/api/auctions/create', payload)
    return response.data
  },

  async startAuction(auctionId: string, duration: StartAuctionDuration): Promise<void> {
    await api.post(`/api/auction/start/${auctionId}`, null, {
      params: {
        duration: durationMap[duration],
      },
    })
  },

  async stopAuction(auctionId: string): Promise<void> {
    await api.post(`/api/auction/stop/${auctionId}`)
  },
}
