import api from '@/api'

export type AdminDashboardData = Record<string, string | number | null>

export const adminService = {
  async getDashboard(): Promise<AdminDashboardData> {
    try {
      const response = await api.get<AdminDashboardData>('/api/admin/dashbaord')
      return response.data
    } catch (error: any) {
      if (error?.response?.status === 404) {
        const fallbackResponse = await api.get<AdminDashboardData>('/api/admin/dashbaord')
        return fallbackResponse.data
      }

      throw error
    }
  },

  async acceptLot(lotId: string): Promise<void> {
    await api.post(`/api/admin/accept/${lotId}`)
  },

  async rejectLot(lotId: string): Promise<void> {
    await api.post(`/api/admin/reject/${lotId}`)
  },
}
