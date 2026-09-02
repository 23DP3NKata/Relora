export interface UserProfileStatsDto {
  bidsPlaced: number
  activeListingsCount: number
  soldItemsCount: number
}

export interface UserProfileListingDto {
  lotId: string
  title: string
  brand?: string | null
  thumbnailUrl?: string | null
  currentPrice: number
  currency: string
  status: number
}

export interface UserProfileDto {
  id: string
  username: string
  name: string
  stats: UserProfileStatsDto
  activeListings: UserProfileListingDto[]
  soldListings: UserProfileListingDto[]
}