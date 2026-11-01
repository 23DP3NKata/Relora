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
