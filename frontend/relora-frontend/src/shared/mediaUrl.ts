// src/shared/media/mediaUrl.ts

const MEDIA_PUBLIC_BASE_URL = import.meta.env.VITE_MEDIA_PUBLIC_BASE_URL

export function buildMediaUrl(media?: { key?: string | null; url?: string | null } | null) {
  if (!media) return '/images/placeholder.jpg'

  if (media.url) return media.url

  if (!media.key) return '/images/placeholder.jpg'

  return `${MEDIA_PUBLIC_BASE_URL}/${media.key}`
}