import api from '@/api'

export interface UploadMediaResponse {
  key: string
}

export const mediaService = {
  async uploadPhoto(file: File): Promise<UploadMediaResponse> 
  {
    const formData = new FormData()
    formData.append("file", file)

    const response = await api.post<UploadMediaResponse>("/api/media/upload", formData, 
    {
      headers: {
        "Content-Type": "multipart/form-data",
      },
      withCredentials: true,
    })

    return response.data
  },
  async deletePhoto(key: string) 
  {
    await api.post("/api/media/delete", null, {
      params: { key },
    })
  },
}
