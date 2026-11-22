export type SupportRequestStatus = 'New' | 'InProgress' | 'Closed'

export type SupportRequest = {
  id: string
  userId: string | null
  username: string | null
  email: string
  category: string
  subject: string
  message: string
  statusName: SupportRequestStatus
  createdAt: string
}
