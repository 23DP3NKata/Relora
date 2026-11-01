// auction timer helpers
export function secondsLeft(endsAt: string | null, now: number): number {
  if (!endsAt) return 0
  return Math.max(0, Math.floor((new Date(endsAt).getTime() - now) / 1000))
}

// timer "02:14:37" or "3d 04:12:00"
export function formatTimeLeft(endsAt: string | null, now: number): string {
  if (!endsAt) return '--:--:--'

  const total = secondsLeft(endsAt, now)
  const days = Math.floor(total / 86400)
  const hours = Math.floor((total % 86400) / 3600)
  const minutes = Math.floor((total % 3600) / 60)
  const seconds = total % 60

  const time = [hours, minutes, seconds]
    .map((part) => part.toString().padStart(2, '0'))
    .join(':')

  return days > 0 ? `${days}d ${time}` : time
}

export function formatPrice(value: number, currency: string, locale: string): string {
  try {
    return new Intl.NumberFormat(locale, {
      style: 'currency',
      currency,
      maximumFractionDigits: 0,
    }).format(value)
  } catch {
    return `${value} ${currency}`
  }
}

// lot number for signature: 1 to "001"
export function formatLotNumber(index: number): string {
  return (index + 1).toString().padStart(3, '0')
}
