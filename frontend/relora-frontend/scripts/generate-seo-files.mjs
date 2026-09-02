import { mkdirSync, readFileSync, writeFileSync } from 'node:fs'
import { dirname, resolve } from 'node:path'

const root = resolve(import.meta.dirname, '..')
const envPath = resolve(root, '.env')
const env = Object.fromEntries(
  readFileSync(envPath, 'utf8')
    .split(/\r?\n/)
    .map(line => line.trim())
    .filter(line => line && !line.startsWith('#'))
    .map(line => {
      const separator = line.indexOf('=')
      return [line.slice(0, separator), line.slice(separator + 1)]
    }),
)

const siteUrl = env.VITE_SITE_URL?.replace(/\/$/, '')

if (!siteUrl || !URL.canParse(siteUrl)) {
  throw new Error('VITE_SITE_URL must contain an absolute URL.')
}

const sitemap = ['/', '/about', '/terms', '/privacy-policy']
  .map(path => `  <url>\n    <loc>${siteUrl}${path}</loc>\n  </url>`)
  .join('\n')

const publicDir = resolve(root, 'public')
mkdirSync(publicDir, { recursive: true })
writeFileSync(resolve(publicDir, 'sitemap.xml'), `<?xml version="1.0" encoding="UTF-8"?>\n<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">\n${sitemap}\n</urlset>\n`)
writeFileSync(resolve(publicDir, 'robots.txt'), `User-agent: *\nAllow: /\n\nSitemap: ${siteUrl}/sitemap.xml\n`)
