// tieši tas, ko atdod GET /api/admin/lots/pending (PendingLotPreviewDto)
export type LotPreview = {
  id: string
  title: string
  brand: string
  priceAmount: number
  currency: string
  condition: number
  status: number
  sellerId: string
  mainPhotoKey: string | null
  createdAt: string
}

// category un condition nāk kā skaitļi
export type PendingLotDetails = {
  id: string
  title: string
  descriptionPreview: string | null
  sellerId: string
  sellerUsername: string
  sellerEmail: string | null
  brand: string | null
  category: number | null
  condition: number | null
  startingPrice: number
  currency: string
  status: number
  mainImageUrl: string | null
  color: string | null
  country: string
  createdAt: string
  photoKeys: string[]
}
