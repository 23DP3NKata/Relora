import api from '@/api'

export interface UploadMediaResponse {
  key: string
}

export interface UploadProofDocumentResponse {
  uploadId: string
  documentTypeId: string
  originalFileName: string
  mimeType: string
  sizeBytes: number
  createdAtUtc: string
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

  async uploadProofDocument(
    file: File,
    documentTypeId: string,
  ): Promise<UploadProofDocumentResponse> {
    const formData = new FormData()
    formData.append("file", file)
    formData.append("documentTypeId", documentTypeId)

    const response = await api.post<UploadProofDocumentResponse>(
      "/api/media/proof-documents/upload",
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
        withCredentials: true,
      },
    )

    return response.data
  },

  async deleteProofDocument(lotId: string, documentId: string): Promise<void> {
    await api.delete(`/api/media/proof-documents/${lotId}/${documentId}`)
  },
}
