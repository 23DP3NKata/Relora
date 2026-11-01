// src/app/services/cookieService.ts

export function getCookie(name: string): string | null {
  const value = document.cookie
    .split('; ')
    .find((row) => row.startsWith(`${name}=`))
    ?.split('=')[1]

  return value ? decodeURIComponent(value) : null
}

export function hasCookie(name: string): boolean {
  return getCookie(name) !== null
}

export function setCookie(name: string, value: string, days = 365): void {
  const maxAge = days * 24 * 60 * 60
  document.cookie = `${name}=${encodeURIComponent(value)}; path=/; max-age=${maxAge}; samesite=lax`
}
