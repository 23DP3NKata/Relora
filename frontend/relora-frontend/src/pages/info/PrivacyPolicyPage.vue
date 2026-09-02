<script setup lang="ts">
const sections = [
  {
    id: 'introduction',
    title: '1. Introduction',
    content: [
      'Relora respects your privacy and is committed to protecting your personal data. We aim to process personal information in a transparent, lawful, and secure manner.',
      'By using our platform, you acknowledge that your information may be collected and used in accordance with this Privacy Policy.',
    ],
  },
  {
    id: 'collected',
    title: '2. Information we collect',
    intro: 'We may collect the following categories of information:',
    list: [
      'Account information, such as name, email address, username, and password',
      'Profile information that you choose to provide',
      'Transaction and auction activity, including bids, listings, purchases, and sales',
      'Communication data, such as messages sent through the platform or support requests',
      'Technical data, such as IP address, browser type, device information, and usage activity',
      'Payment-related information, where applicable, processed through secure payment providers',
    ],
  },
  {
    id: 'usage',
    title: '3. How we use your information',
    intro: 'Your information may be used to:',
    list: [
      'Create and manage your account',
      'Operate auctions, listings, bidding, and transactions',
      'Provide customer support and respond to inquiries',
      'Improve platform functionality, performance, and security',
      'Prevent fraud, abuse, unauthorized access, and policy violations',
      'Send important service-related notifications',
      'Comply with applicable legal and regulatory obligations',
    ],
  },
  {
    id: 'legal-basis',
    title: '4. Legal basis for processing',
    content: [
      'Where applicable, we process personal data based on one or more of the following legal grounds: performance of a contract, compliance with legal obligations, legitimate interests, or your consent where consent is required.',
    ],
  },
  {
    id: 'sharing',
    title: '5. How we share information',
    intro: 'We do not sell personal information. We may share information only when necessary, including:',
    list: [
      'With service providers who support platform operations',
      'With payment processors and related financial service providers',
      'With legal authorities when required by law or to protect legal rights',
      'In connection with business changes such as a merger, acquisition, or restructuring',
    ],
  },
  {
    id: 'cookies',
    title: '6. Cookies and similar technologies',
    content: [
      'Relora may use cookies and similar technologies to improve user experience, remember preferences, analyze traffic, and maintain platform security.',
      'You may be able to control certain cookie settings through your browser.',
    ],
  },
  {
    id: 'retention',
    title: '7. Data retention',
    content: [
      'We retain personal information only for as long as necessary to provide our services, comply with legal obligations, resolve disputes, enforce agreements, and maintain legitimate business records.',
    ],
  },
  {
    id: 'security',
    title: '8. Data security',
    content: [
      'We take reasonable technical and organizational measures to protect personal information against unauthorized access, loss, misuse, disclosure, or alteration.',
      'However, no online system can be guaranteed to be completely secure.',
    ],
  },
  {
    id: 'rights',
    title: '9. Your rights',
    content: [
      'Depending on your jurisdiction, you may have rights related to your personal data, including the right to access, correct, delete, restrict, or object to certain processing activities.',
      'You may also have the right to withdraw consent where processing is based on consent.',
    ],
  },
  {
    id: 'third-party',
    title: '10. Third-party services and links',
    content: [
      'Our platform may contain links to third-party services or rely on external providers. We are not responsible for the privacy practices of third-party websites or services.',
    ],
  },
  {
    id: 'children',
    title: '11. Children’s privacy',
    content: [
      'Relora is not intended for children, and we do not knowingly collect personal information from children without an appropriate legal basis or parental authorization where required by law.',
    ],
  },
  {
    id: 'changes',
    title: '12. Changes to this Privacy Policy',
    content: [
      'We may update this Privacy Policy from time to time to reflect changes in our services, legal obligations, or operational practices.',
      'The updated version will be published on this page with a revised “Last updated” date.',
    ],
  },
  {
    id: 'contact',
    title: '13. Contact us',
    content: [
      'If you have questions about this Privacy Policy or how your personal information is handled, you can contact us through the platform’s support channels or by email.',
    ],
    email: 'privacy@relora.com',
  },
]
</script>

<template>
  <div class="bg-background text-foreground">
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="overflow-hidden rounded-[32px] bg-neutral-100 dark:bg-neutral-800">
        <div class="grid gap-8 px-6 py-10 sm:px-10 lg:grid-cols-[1.1fr_0.9fr] lg:px-12 xl:px-16">
          <div class="max-w-3xl">
            <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/40">
              Legal
            </p>

            <h1 class="mt-4 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl xl:text-6xl">
              Privacy Policy
            </h1>

            <p class="mt-3 text-sm text-foreground/45">
              Last updated: March 17, 2026
            </p>

            <p class="mt-5 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base">
              This Privacy Policy explains how Relora collects, uses, stores, and protects personal information when you use our platform, including browsing auctions, creating an account, listing items, placing bids, or communicating with us.
            </p>
          </div>

          <div class="flex items-end lg:justify-end">
            <div class="w-full rounded-[28px] bg-background p-6">
              <p class="text-[11px] uppercase tracking-[0.22em] text-foreground/40">
                Summary
              </p>
              <p class="mt-3 text-sm leading-7 text-foreground/65">
                We collect and use personal information to operate accounts, auctions, transactions, platform security, and support. We do not sell personal information.
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="grid gap-8 lg:grid-cols-[260px_minmax(0,1fr)]">
        <aside class="hidden lg:block">
          <div class="sticky top-36 rounded-[28px] border bg-background p-5">
            <p class="text-[11px] uppercase tracking-[0.22em] text-foreground/40">
              On this page
            </p>

            <nav class="mt-4 space-y-3">
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
            class="scroll-mt-32 rounded-[28px] border bg-background p-6 sm:p-8"
          >
            <h2 class="text-2xl font-semibold tracking-tight text-foreground">
              {{ section.title }}
            </h2>

            <p
              v-if="section.intro"
              class="mt-5 text-sm leading-8 text-foreground/65 sm:text-base"
            >
              {{ section.intro }}
            </p>

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
                <span class="mt-2 h-1.5 w-1.5 rounded-full bg-foreground/70"></span>
                <span>{{ item }}</span>
              </li>
            </ul>

            <p
              v-if="section.email"
              class="mt-5 text-sm text-foreground/45"
            >
              Email: {{ section.email }}
            </p>
          </section>
        </div>
      </div>
    </section>

    <section class="mx-auto max-w-7xl px-4 pb-16 sm:px-6 lg:px-8">
      <div class="rounded-[32px] border bg-background px-6 py-8 text-center sm:px-10">
        <p class="text-sm text-foreground/45">
          Relora · Privacy Policy
        </p>
        <p class="mt-3 text-lg font-medium text-foreground/65">
          Clear privacy standards support a more trustworthy marketplace.
        </p>
      </div>
    </section>
  </div>
</template>