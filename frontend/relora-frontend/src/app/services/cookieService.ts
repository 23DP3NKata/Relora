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