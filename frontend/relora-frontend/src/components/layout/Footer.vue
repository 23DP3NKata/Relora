<script setup lang="ts">
import Container from '@/components/ui/Container.vue'
import { RouterLink } from 'vue-router'
import { useLocalePath } from '@/composables/useLocalePath'

const currentYear = new Date().getFullYear()
const localePath = useLocalePath()

const footerColumns = [
  {
    titleKey: 'common.brand',
    links: [
      { labelKey: 'navigation.about', to: '/about' },
      { labelKey: 'navigation.sustainability', to: '/sustainability' },
      { labelKey: 'navigation.journal', to: '/journal' },
    ],
  },
  {
    titleKey: 'navigation.buy',
    links: [
      { labelKey: 'navigation.buyerProtection', to: '/buyer-protection' },
      { labelKey: 'navigation.returnsPolicy', to: '/returns-policy' },
    ],
  },
  {
    titleKey: 'navigation.sell',
    links: [
      { labelKey: 'navigation.howToSell', to: '/how-to-sell' },
      { labelKey: 'navigation.sellerProtection', to: '/seller-protection' },
      { labelKey: 'navigation.sellAnItem', to: '/sell' },
      { labelKey: 'navigation.brandPartnerships', to: '/brand-partnerships' },
    ],
  },
  {
    titleKey: 'navigation.help',
    links: [
      { labelKey: 'navigation.contactUs', to: '/contact' },
      { labelKey: 'navigation.faq', to: '/faq' },
    ],
  },
  {
    titleKey: 'navigation.legal',
    links: [
      { labelKey: 'navigation.termsOfService', to: '/terms' },
      { labelKey: 'navigation.privacyPolicy', to: '/privacy-policy' },
      { labelKey: 'navigation.cookiePolicy', to: '/cookie-policy' },
      { labelKey: 'navigation.auctionRules', to: '/auction-rules' },
    ],
  },
]

const socialLinks = [
  {
    labelKey: 'footer.social.instagram',
    href: 'https://www.instagram.com/relora_auctions/',
    icon: 'instagram',
  },
  {
    labelKey: 'footer.social.youtube',
    href: 'https://www.youtube.com/channel/UC5dwCRPbNoxNyYrbd6RSwRA',
    icon: 'youtube',
  }
]
</script>

<template>
  <footer class="border-t border-border">
    <Container>
      <div class="py-10">
        <div class="grid grid-cols-2 gap-x-8 gap-y-8 md:grid-cols-3 lg:grid-cols-6">
          <div
            v-for="column in footerColumns"
            :key="column.titleKey"
          >
            <h3 class="mb-4 text-xs font-bold uppercase tracking-[0.18em] text-foreground/80">
              {{ $t(column.titleKey) }}
            </h3>

            <ul class="space-y-3">
              <li
                v-for="link in column.links"
                :key="link.labelKey"
              >
                <RouterLink
                  :to="localePath(link.to)"
                  class="text-sm text-foreground/60 transition-colors hover:text-foreground"
                >
                  {{ $t(link.labelKey) }}
                </RouterLink>
              </li>
            </ul>
          </div>

          <div>
            <h3 class="mb-4 text-xs font-bold uppercase tracking-[0.18em] text-foreground/80">
              {{ $t('navigation.followUs') }}
            </h3>

            <div class="flex items-center gap-3">
              <a
                v-for="social in socialLinks"
                :key="social.labelKey"
                :href="social.href"
                :aria-label="$t(social.labelKey)"
                target="_blank"
                rel="noopener noreferrer"
                class="flex size-9 items-center justify-center rounded-full border border-border text-foreground/60 transition-colors hover:border-foreground hover:text-foreground"
              >
                <svg
                  v-if="social.icon === 'instagram'"
                  class="size-4"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="1.8"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  aria-hidden="true"
                >
                  <rect
                    x="3"
                    y="3"
                    width="18"
                    height="18"
                    rx="5"
                    ry="5"
                  />
                  <circle
                    cx="12"
                    cy="12"
                    r="4"
                  />
                  <circle
                    cx="17.5"
                    cy="6.5"
                    r="1"
                    fill="currentColor"
                    stroke="none"
                  />
                </svg>

                <svg
                  v-else-if="social.icon === 'youtube'"
                  class="size-4"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="1.8"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  aria-hidden="true"
                >
                  <path
                    d="M21.2 7.2a2.8 2.8 0 0 0-2-2C17.4 4.7 12 4.7 12 4.7s-5.4 0-7.2.5a2.8 2.8 0 0 0-2 2A29 29 0 0 0 2.3 12a29 29 0 0 0 .5 4.8 2.8 2.8 0 0 0 2 2c1.8.5 7.2.5 7.2.5s5.4 0 7.2-.5a2.8 2.8 0 0 0 2-2 29 29 0 0 0 .5-4.8 29 29 0 0 0-.5-4.8Z"
                  />
                  <path
                    d="m10 15 5-3-5-3v6Z"
                    fill="currentColor"
                    stroke="none"
                  />
                </svg>

                <span class="sr-only">
                  {{ $t(social.labelKey) }}
                </span>
              </a>
            </div>
          </div>
        </div>
      </div>
    </Container>

    <div class="border-t border-border py-6">
      <Container>
        <div class="space-y-4">
          <h4 class="text-xs font-semibold text-foreground/80">
            {{ $t('footer.noticeTitle') }}
          </h4>

          <p class="max-w-6xl text-[11px] leading-relaxed text-foreground/60">
            {{ $t('footer.notice') }}
          </p>

          <div class="flex flex-col justify-between gap-3 pt-2 md:flex-row md:items-center">
            <div class="flex flex-wrap gap-x-5 gap-y-2">
              <RouterLink
                :to="localePath('/about')"
                class="text-[11px] text-foreground/50 transition-colors hover:text-foreground"
              >
                {{ $t('navigation.about') }}
              </RouterLink>

              <RouterLink
                :to="localePath('/terms')"
                class="text-[11px] text-foreground/50 transition-colors hover:text-foreground"
              >
                {{ $t('navigation.termsOfService') }}
              </RouterLink>

              <RouterLink
                :to="localePath('/privacy-policy')"
                class="text-[11px] text-foreground/50 transition-colors hover:text-foreground"
              >
                {{ $t('navigation.privacyPolicy') }}
              </RouterLink>

              <RouterLink
                :to="localePath('/contact')"
                class="text-[11px] text-foreground/50 transition-colors hover:text-foreground"
              >
                {{ $t('navigation.contactUs') }}
              </RouterLink>
            </div>

            <p class="text-[11px] text-foreground/50">
              {{ $t('common.copyright', { year: currentYear }) }}
            </p>
          </div>
        </div>
      </Container>
    </div>
  </footer>
</template>
