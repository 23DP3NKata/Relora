export type LotPreview = {
  id: string
  title?: string
  description?: string
  brand?: string
  price?: number
  amount?: number
  currency?: string
  status?: string | number | null
  statusName?: string | null
  sellerId?: string | null
  createdAt?: string | null
  createdDate?: string | null
  media?: Array<{ url?: string | null; key?: string | null }>
}
