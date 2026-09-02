// src/utils/compressImage.ts

import imageCompression from "browser-image-compression"

export const compressAndConvertToWebp = async (file: File): Promise<File> => {
  if (!file.type.startsWith("image/")) {
    throw new Error("Only image files are allowed.")
  }

  const compressedBlob = await imageCompression(file, {
    maxSizeMB: 0.6,
    maxWidthOrHeight: 1600,
    useWebWorker: true,
    initialQuality: 0.8,
    fileType: "image/webp",
  })

  const originalName = file.name.replace(/\.[^/.]+$/, "")

  return new File(
    [compressedBlob],
    `${originalName}.webp`,
    {
      type: "image/webp",
      lastModified: Date.now(),
    }
  )
}