import type { UserProfileDto } from '@/types/userProfile'
import api from '@/api'

export function ProfileService() {
  async function getUserProfile(username: string): Promise<UserProfileDto> 
  {
    const response = await api.get(`/api/profile/${username}`)
    return response.data
  }

  return {
    getUserProfile,
  }
}