/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL?: string
  readonly VITE_MEDIA_PUBLIC_BASE_URL?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
