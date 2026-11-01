import api from '@/api'

// tas ko atdod GET /api/admin/dashboard (AdminDashboardDto)
export type AdminDashboardData = {
  usersCount: number
  activeAuctionsCount: number
  pendingLotsCount: number
}

export const adminService = {
  async getDashboard(): Promise<AdminDashboardData> {
    const response = await api.get<AdminDashboardData>('/api/admin/dashboard')
    return response.data
  },

  async acceptLot(lotId: string): Promise<void> {
    await api.post(`/api/admin/accept/${lotId}`)
  },

  async rejectLot(lotId: string, reason: string): Promise<void> {
    await api.post(`/api/admin/reject/${lotId}`, { reason })
  },
}
