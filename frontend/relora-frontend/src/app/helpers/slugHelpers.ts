export function brandSlugToName(slug: string): string {
  return slug
    .split('-')
    .filter(Boolean)
    .map((word) => {
      return word.charAt(0).toUpperCase() + word.slice(1)
    })
    .join(' ')
}