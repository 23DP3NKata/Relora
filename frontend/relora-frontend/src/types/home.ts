// lot shown on homepage
export type HomeLiveLot = {
  lotId: string
  auctionId: string | null
  title: string
  brand: string
  imageUrl: string
  sizeName: string
  conditionName: string
  currentPrice: number
  currency: string
  endsAt: string | null
  bidCount: number | null
  isDemo: boolean
}
