<script setup lang="ts">
import { RouterLink } from 'vue-router'

type CookiePolicySection = {
  id: string
  title: string
  content?: string[]
  list?: string[]
  note?: string
}

type CookieCategory = {
  name: string
  status: string
  description: string
}

type CookieRecord = {
  name: string
  provider: string
  category: string
  purpose: string
  duration: string
}

const cookieCategories: CookieCategory[] = [
  {
    name: 'Strictly necessary',
    status: 'Always active',
    description:
      'Required for authentication, security, checkout and other essential platform functions.',
  },
  {
    name: 'Preferences',
    status: 'Based on your choice',
    description:
      'Remember settings you select, such as your browsing or catalogue preference.',
  },
  {
    name: 'Analytics',
    status: 'Optional',
    description:
      'May help us understand how visitors use Relora and improve the platform.',
  },
  {
    name: 'Marketing',
    status: 'Optional',
    description:
      'May be used to measure campaigns or provide more relevant advertising.',
  },
]

/*
 * Important:
 * Replace generic authentication names and retention periods with the exact
 * values used by your backend before publishing this policy.
 *
 * Do not add analytics or marketing cookies here until they are actually
 * implemented on Relora.
 */
const cookies: CookieRecord[] = [
  {
    name: 'Authentication cookie(s)',
    provider: 'Relora',
    category: 'Strictly necessary',
    purpose:
      'Keeps you signed in, maintains your authenticated session and protects access to account features.',
    duration: 'Session or until the authentication token expires',
  },
  {
    name: 'user_preference',
    provider: 'Relora',
    category: 'Preferences',
    purpose:
      'Remembers the catalogue preference selected by the user, such as Men or Women.',
    duration: 'Until changed, expired or deleted',
  },
  {
    name: 'Cookie settings record',
    provider: 'Relora',
    category: 'Strictly necessary',
    purpose:
      'Remembers your cookie choices so that Relora can respect them during future visits.',
    duration: 'According to the cookie consent configuration',
  },
]

const sections: CookiePolicySection[] = [
  {
    id: 'overview',
    title: '1. Overview',
    content: [
      'This Cookie Policy explains how Relora uses cookies and similar technologies when you visit or use the platform.',
      'It describes the types of cookies that may be used, why they are used, how long they may remain on your device and how you can manage your choices.',
      'This Cookie Policy should be read together with the Relora Privacy Policy.',
    ],
  },
  {
    id: 'what-are-cookies',
    title: '2. What Cookies Are',
    content: [
      'Cookies are small text files stored on your computer, phone, tablet or another device when you visit a website.',
      'Cookies may allow a website to recognise a browser, maintain a secure session, remember selected preferences and understand how platform features are used.',
      'Some cookies exist only during a browsing session. Other cookies remain on the device for a defined period or until they are removed.',
    ],
  },
  {
    id: 'similar-technologies',
    title: '3. Similar Technologies',
    content: [
      'Relora may also use technologies that operate in a similar way to cookies.',
      'References to cookies in this policy may therefore also include local storage, session storage, software development kit identifiers, pixels and similar browser or device technologies.',
    ],
    list: [
      'Local storage used to retain browser-side information',
      'Session storage removed after the browsing session',
      'Pixels used to measure whether content was viewed',
      'Device or browser identifiers used for security and fraud prevention',
    ],
  },
  {
    id: 'necessary',
    title: '4. Strictly Necessary Cookies',
    content: [
      'Strictly necessary cookies are required for Relora to provide essential platform functions.',
      'These cookies may be used without optional cookie consent because the platform may not work correctly without them.',
      'Disabling these cookies through your browser may prevent you from signing in, completing checkout or accessing protected account features.',
    ],
    list: [
      'Authenticating registered users',
      'Maintaining secure account sessions',
      'Processing checkout and payment steps',
      'Protecting the platform against abuse and fraud',
      'Balancing traffic and maintaining technical stability',
      'Remembering cookie consent choices',
    ],
    note:
      'Strictly necessary cookies are not used for behavioural advertising.',
  },
  {
    id: 'preferences',
    title: '5. Preference Cookies',
    content: [
      'Preference cookies allow Relora to remember choices you make while using the platform.',
      'These choices may affect how catalogues, navigation or other parts of the platform are displayed.',
      'For example, Relora may use the user_preference cookie to remember whether you selected the Men or Women catalogue preference.',
    ],
    list: [
      'Catalogue preference',
      'Language selection where available',
      'Display or interface settings',
      'Dismissed notices and banners',
    ],
  },
  {
    id: 'analytics',
    title: '6. Analytics Cookies',
    content: [
      'Relora may use analytics cookies in the future to understand how visitors interact with the platform.',
      'Analytics information may include visited pages, session duration, navigation paths, general device information and technical errors.',
      'Optional analytics cookies should only be placed after the user has provided the required consent.',
    ],
    list: [
      'Measuring website traffic',
      'Understanding which pages are commonly visited',
      'Finding broken or confusing user journeys',
      'Measuring platform performance',
      'Improving features and page layouts',
    ],
    note:
      'Do not list an analytics provider in this policy until that provider has actually been added to Relora.',
  },
  {
    id: 'marketing',
    title: '7. Marketing Cookies',
    content: [
      'Relora may use marketing cookies only when such technologies are implemented and the required permission has been obtained.',
      'Marketing cookies may be used to measure advertising campaigns, limit repeated advertisements or understand whether a campaign resulted in a platform visit.',
      'Rejecting marketing cookies should not prevent access to the main Relora marketplace.',
    ],
    list: [
      'Advertising campaign measurement',
      'Conversion measurement',
      'Audience measurement',
      'Limiting repeated advertising',
      'Providing more relevant promotional content',
    ],
    note:
      'Marketing cookies and advertising pixels must not load before the user has provided the required consent.',
  },
  {
    id: 'first-third-party',
    title: '8. First-Party and Third-Party Cookies',
    content: [
      'First-party cookies are created directly by Relora and operate under the Relora domain.',
      'Third-party cookies may be created by another service used on the platform, such as a payment, analytics, embedded-content or advertising provider.',
      'Third-party providers may process information according to their own privacy and cookie policies.',
    ],
    list: [
      'Relora authentication and preference cookies are first-party cookies',
      'Payment providers may use their own security and fraud-prevention technologies',
      'Analytics providers may use identifiers when analytics have been enabled',
      'Embedded external content may be subject to the provider’s cookie settings',
    ],
  },
  {
    id: 'payments',
    title: '9. Payment and Security Technologies',
    content: [
      'Relora may use third-party payment services to process transactions and onboard sellers.',
      'Payment providers may use cookies or similar technologies for authentication, security, fraud detection and payment processing.',
      'Some payment-related technologies may be necessary to complete the service requested by the user.',
      'Relora does not receive or store complete payment card information when it is collected directly by the payment provider.',
    ],
  },
  {
    id: 'consent',
    title: '10. Your Cookie Choices',
    content: [
      'When optional cookies are used, Relora should provide controls that allow you to accept or reject them.',
      'You may be able to choose individual categories rather than accepting every optional category.',
      'Strictly necessary cookies cannot normally be disabled through the Relora cookie settings because they are required for essential platform functions.',
    ],
    list: [
      'Accept all optional cookies',
      'Reject optional cookies',
      'Enable or disable individual cookie categories',
      'Review information about each category',
      'Change previously selected choices',
    ],
    note:
      'Not selecting optional cookies should be treated as no permission to activate them.',
  },
  {
    id: 'withdraw-consent',
    title: '11. Changing or Withdrawing Consent',
    content: [
      'You may change or withdraw your optional cookie consent at any time through the Cookie settings control.',
      'Withdrawing consent does not affect processing that took place before the withdrawal.',
      'After a category is disabled, Relora should stop placing cookies for that category and remove accessible cookies where technically possible.',
      'Some information may remain with a third-party provider according to its applicable retention and deletion rules.',
    ],
  },
  {
    id: 'browser-settings',
    title: '12. Browser Controls',
    content: [
      'Most browsers allow you to inspect, block and delete cookies through their settings.',
      'Browser controls operate separately from the Relora cookie settings.',
      'Blocking all cookies may prevent important parts of Relora from working correctly.',
    ],
    list: [
      'View cookies stored by individual websites',
      'Delete existing cookies',
      'Block cookies from specific websites',
      'Block third-party cookies',
      'Delete cookies when the browser closes',
    ],
  },
  {
    id: 'retention',
    title: '13. Cookie Retention',
    content: [
      'Cookies are retained only for as long as reasonably required for their stated purpose.',
      'Session cookies are generally removed when the browser session ends, while persistent cookies remain until their expiration date or until they are deleted.',
      'The precise duration depends on the cookie, its purpose and the technical configuration of the service that created it.',
    ],
  },
  {
    id: 'personal-data',
    title: '14. Cookies and Personal Data',
    content: [
      'Information collected through cookies may sometimes be linked to an account, browser, device, IP address or another identifier.',
      'Where cookie information constitutes personal data, it is processed according to the Relora Privacy Policy and applicable data-protection requirements.',
      'The Privacy Policy explains the purposes and legal grounds for processing, data recipients, retention periods and user rights.',
    ],
  },
  {
    id: 'changes',
    title: '15. Changes to This Policy',
    content: [
      'Relora may update this Cookie Policy when platform functionality, service providers, cookies or legal requirements change.',
      'The latest version will be published on this page together with an updated revision date.',
      'Where required, Relora may request cookie consent again after a significant change.',
    ],
  },
  {
    id: 'contact',
    title: '16. Contact',
    content: [
      'For questions about cookies, privacy or personal-data processing, contact Relora through the Contact us page.',
      'Please include enough information for Relora to understand and respond to your request.',
    ],
  },
]

function openCookieSettings() {
  /*
   * Your CookieBanner or CookieConsentModal component can listen for:
   *
   * window.addEventListener(
   *   'relora:open-cookie-settings',
   *   openPreferencesModal,
   * )
   */
  window.dispatchEvent(
    new CustomEvent('relora:open-cookie-settings'),
  )
}
</script>

<template>
  <div class="bg-background text-foreground">
    <!-- Hero -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div
        class="overflow-hidden rounded-[32px] bg-neutral-100 dark:bg-neutral-800"
      >
        <div
          class="grid gap-8 px-6 py-10 sm:px-10 lg:grid-cols-[1.1fr_0.9fr] lg:px-12 xl:px-16"
        >
          <div class="max-w-3xl">
            <p
              class="text-[11px] uppercase tracking-[0.24em] text-foreground/60"
            >
              Privacy and data
            </p>

            <h1
              class="mt-4 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl xl:text-6xl"
            >
              Cookie Policy
            </h1>

            <p class="mt-3 text-sm text-foreground/45">
              Last updated: July 24, 2026
            </p>

            <p
              class="mt-5 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base"
            >
              Learn how Relora uses cookies and similar technologies to keep
              the marketplace secure, remember your preferences and improve the
              platform.
            </p>

            <div class="mt-7 flex flex-wrap gap-3">
              <button
                type="button"
                class="inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-6 text-sm font-medium text-background transition hover:opacity-80"
                @click="openCookieSettings"
              >
                Manage cookie settings
              </button>

              <RouterLink
                to="/privacy-policy"
                class="inline-flex min-h-11 items-center justify-center rounded-full border px-6 text-sm font-medium text-foreground transition hover:bg-neutral-200/60 dark:hover:bg-neutral-700/60"
              >
                Privacy Policy
              </RouterLink>
            </div>
          </div>

          <div class="flex items-end lg:justify-end">
            <div class="w-full rounded-[28px] bg-background p-6">
              <p
                class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
              >
                Cookie summary
              </p>

              <p class="mt-3 text-sm leading-7 text-foreground/65">
                Relora uses necessary cookies for security, authentication and
                core platform functions. Optional technologies should only be
                enabled according to your cookie choices.
              </p>

              <div class="mt-5 space-y-3">
                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Necessary cookies keep Relora working
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Optional categories depend on your choice
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Settings can be changed later
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Cookie categories -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="mb-8">
        <p
          class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
        >
          Cookie categories
        </p>

        <h2
          class="mt-3 text-3xl font-semibold tracking-tight text-foreground"
        >
          How different cookies are used
        </h2>
      </div>

      <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <article
          v-for="category in cookieCategories"
          :key="category.name"
          class="rounded-[28px] border bg-background p-6"
        >
          <div class="flex items-start justify-between gap-3">
            <h3 class="text-lg font-semibold tracking-tight text-foreground">
              {{ category.name }}
            </h3>

            <span
              class="shrink-0 rounded-full bg-neutral-100 px-3 py-1 text-[10px] font-medium uppercase tracking-[0.12em] text-foreground/55 dark:bg-neutral-800"
            >
              {{ category.status }}
            </span>
          </div>

          <p class="mt-5 text-sm leading-7 text-foreground/60">
            {{ category.description }}
          </p>
        </article>
      </div>
    </section>

    <!-- Cookie table -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="rounded-[32px] border bg-background p-6 sm:p-8">
        <div class="max-w-3xl">
          <p
            class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
          >
            Cookies currently described
          </p>

          <h2
            class="mt-3 text-3xl font-semibold tracking-tight text-foreground"
          >
            Cookie details
          </h2>

          <p class="mt-4 text-sm leading-7 text-foreground/60 sm:text-base">
            This list should be updated whenever Relora adds, removes or
            changes a cookie or similar technology.
          </p>
        </div>

        <!-- Desktop table -->
        <div class="mt-8 hidden overflow-x-auto md:block">
          <table class="w-full min-w-[900px] text-left">
            <thead>
              <tr class="border-b">
                <th
                  class="px-4 py-4 text-[11px] font-medium uppercase tracking-[0.16em] text-foreground/45"
                >
                  Name
                </th>

                <th
                  class="px-4 py-4 text-[11px] font-medium uppercase tracking-[0.16em] text-foreground/45"
                >
                  Provider
                </th>

                <th
                  class="px-4 py-4 text-[11px] font-medium uppercase tracking-[0.16em] text-foreground/45"
                >
                  Category
                </th>

                <th
                  class="px-4 py-4 text-[11px] font-medium uppercase tracking-[0.16em] text-foreground/45"
                >
                  Purpose
                </th>

                <th
                  class="px-4 py-4 text-[11px] font-medium uppercase tracking-[0.16em] text-foreground/45"
                >
                  Duration
                </th>
              </tr>
            </thead>

            <tbody>
              <tr
                v-for="cookie in cookies"
                :key="`${cookie.name}-${cookie.category}`"
                class="border-b last:border-b-0"
              >
                <td
                  class="px-4 py-5 align-top text-sm font-medium text-foreground"
                >
                  {{ cookie.name }}
                </td>

                <td
                  class="px-4 py-5 align-top text-sm text-foreground/60"
                >
                  {{ cookie.provider }}
                </td>

                <td class="px-4 py-5 align-top">
                  <span
                    class="inline-flex rounded-full bg-neutral-100 px-3 py-1 text-xs text-foreground/60 dark:bg-neutral-800"
                  >
                    {{ cookie.category }}
                  </span>
                </td>

                <td
                  class="max-w-md px-4 py-5 align-top text-sm leading-7 text-foreground/60"
                >
                  {{ cookie.purpose }}
                </td>

                <td
                  class="px-4 py-5 align-top text-sm leading-7 text-foreground/60"
                >
                  {{ cookie.duration }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Mobile cards -->
        <div class="mt-8 space-y-4 md:hidden">
          <article
            v-for="cookie in cookies"
            :key="`${cookie.name}-${cookie.category}-mobile`"
            class="rounded-[24px] bg-neutral-100 p-5 dark:bg-neutral-800"
          >
            <div class="flex flex-wrap items-start justify-between gap-3">
              <h3 class="font-medium text-foreground">
                {{ cookie.name }}
              </h3>

              <span
                class="rounded-full bg-background px-3 py-1 text-xs text-foreground/60"
              >
                {{ cookie.category }}
              </span>
            </div>

            <dl class="mt-5 space-y-4">
              <div>
                <dt
                  class="text-[10px] uppercase tracking-[0.16em] text-foreground/40"
                >
                  Provider
                </dt>

                <dd class="mt-1 text-sm text-foreground/65">
                  {{ cookie.provider }}
                </dd>
              </div>

              <div>
                <dt
                  class="text-[10px] uppercase tracking-[0.16em] text-foreground/40"
                >
                  Purpose
                </dt>

                <dd class="mt-1 text-sm leading-7 text-foreground/65">
                  {{ cookie.purpose }}
                </dd>
              </div>

              <div>
                <dt
                  class="text-[10px] uppercase tracking-[0.16em] text-foreground/40"
                >
                  Duration
                </dt>

                <dd class="mt-1 text-sm leading-7 text-foreground/65">
                  {{ cookie.duration }}
                </dd>
              </div>
            </dl>
          </article>
        </div>
      </div>
    </section>

    <!-- Main policy -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="grid gap-8 lg:grid-cols-[260px_minmax(0,1fr)]">
        <aside class="hidden lg:block">
          <div class="sticky top-36 rounded-[28px] border bg-background p-5">
            <p
              class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
            >
              On this page
            </p>

            <nav
              class="mt-4 max-h-[calc(100vh-12rem)] space-y-3 overflow-y-auto pr-2"
            >
              <a
                v-for="section in sections"
                :key="section.id"
                :href="`#${section.id}`"
                class="block text-sm text-foreground/60 transition hover:text-foreground"
              >
                {{ section.title }}
              </a>
            </nav>
          </div>
        </aside>

        <div class="space-y-6">
          <section
            v-for="section in sections"
            :id="section.id"
            :key="section.id"
            class="scroll-mt-40 rounded-[28px] border bg-background p-6 sm:p-8"
          >
            <h2 class="text-2xl font-semibold tracking-tight text-foreground">
              {{ section.title }}
            </h2>

            <div
              v-if="section.content"
              class="mt-5 space-y-4 text-sm leading-8 text-foreground/65 sm:text-base"
            >
              <p
                v-for="paragraph in section.content"
                :key="paragraph"
              >
                {{ paragraph }}
              </p>
            </div>

            <ul
              v-if="section.list"
              class="mt-5 space-y-3"
            >
              <li
                v-for="item in section.list"
                :key="item"
                class="flex items-start gap-3 text-sm leading-7 text-foreground/65 sm:text-base"
              >
                <span
                  class="mt-2.5 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                />

                <span>{{ item }}</span>
              </li>
            </ul>

            <div
              v-if="section.note"
              class="mt-6 rounded-[22px] bg-neutral-100 px-5 py-4 dark:bg-neutral-800"
            >
              <p
                class="text-[11px] uppercase tracking-[0.2em] text-foreground/50"
              >
                Important
              </p>

              <p class="mt-2 text-sm leading-7 text-foreground/65">
                {{ section.note }}
              </p>
            </div>
          </section>
        </div>
      </div>
    </section>

    <!-- Bottom CTA -->
    <section class="mx-auto max-w-7xl px-4 pb-16 sm:px-6 lg:px-8">
      <div
        class="rounded-[32px] border bg-background px-6 py-10 text-center sm:px-10"
      >
        <p class="text-sm text-foreground/45">
          Relora · Cookie Policy
        </p>

        <p class="mt-3 text-xl font-medium text-foreground/70">
          Your choices should remain under your control.
        </p>

        <p
          class="mx-auto mt-3 max-w-xl text-sm leading-7 text-foreground/55"
        >
          Review or update your optional cookie preferences whenever you need
          to.
        </p>

        <button
          type="button"
          class="mt-6 inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-7 text-sm font-medium text-background transition hover:opacity-80"
          @click="openCookieSettings"
        >
          Manage cookie settings
        </button>
      </div>
    </section>
  </div>
</template>