export type AuctionStatus =
  | 'Draft'
  | 'Active'
  | 'Stopped'
  | 'Ended'
  | 'Sold'
  | 'Expired'
  | 'Unsold'
  | string

export type AuctionDetails = {
  auctionId: string
  status: AuctionStatus
  currentPrice: number | null
  currency: string | null
  startsAt: string | null
  endsAt: string | null
  lotId: string | null
  lotTitle: string
}

export type AuctionListItem = {
  auctionId: string
  lotId: string | null
  status: AuctionStatus
  endDate: string | null
  currentPrice: number | null
  currency: string | null
}

export type CreateAuctionPayload = {
  lotId: string
}

export type AuctionDurationOption =
  | 'Classic3Days'
  | 'Classic7Days'
  | 'Classic14Days'
  | 'Flash1Hour'
  | 'Flash6Hours'

export type StartAuctionDuration = '3' | '5' | '7'

export type Auction = {
  id: number
  brand: string
  title: string
  price: number
  imageUrl: string
  timeLeft: string
}
