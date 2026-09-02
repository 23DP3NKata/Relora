<script setup lang="ts">
const sections = [
  {
    id: 'acceptance',
    title: '1. Acceptance of Terms',
    content: [
      'By accessing or using Relora, you agree to be bound by these Terms. If you do not agree, you should not use the platform.',
    ],
  },
  {
    id: 'accounts',
    title: '2. User Accounts',
    content: [
      'You may be required to create an account to access certain features. You are responsible for maintaining the confidentiality of your account credentials.',
      'You agree to provide accurate and complete information and to keep it updated.',
    ],
  },
  {
    id: 'auctions',
    title: '3. Auctions and Bidding',
    content: [
      'Relora operates as an auction-based marketplace where users can list items and place bids.',
      'By placing a bid, you agree that your bid is binding and that you intend to purchase the item if your bid is the highest at the end of the auction.',
      'Sellers are responsible for accurately describing items, including condition, authenticity, and details.',
    ],
  },
  {
    id: 'transactions',
    title: '4. Transactions',
    content: [
      'When an auction ends, the highest bidder is obligated to complete the transaction.',
      'Payment and delivery terms may depend on the platform’s policies and integrations with payment providers.',
      'Relora may act as an intermediary but is not the direct seller of listed items.',
    ],
  },
  {
    id: 'fees',
    title: '5. Fees and Payments',
    content: [
      'Certain features or transactions may be subject to fees. These fees will be clearly communicated before they apply.',
      'Relora reserves the right to update pricing and fee structures at any time.',
    ],
  },
  {
    id: 'conduct',
    title: '6. Prohibited Conduct',
    list: [
      'Listing counterfeit or illegal items',
      'Manipulating bids or engaging in fraudulent behavior',
      'Misrepresenting items or providing false information',
      'Using the platform for unlawful purposes',
    ],
  },
  {
    id: 'liability',
    title: '7. Limitation of Liability',
    content: [
      'Relora provides the platform "as is" and does not guarantee the accuracy of listings, the behavior of users, or the outcome of transactions.',
      'To the maximum extent permitted by law, Relora is not liable for any direct or indirect damages.',
    ],
  },
  {
    id: 'termination',
    title: '8. Account Suspension and Termination',
    content: [
      'Relora reserves the right to suspend or terminate accounts that violate these Terms or engage in harmful or abusive behavior.',
    ],
  },
  {
    id: 'changes',
    title: '9. Changes to Terms',
    content: [
      'We may update these Terms from time to time. Continued use of the platform after updates constitutes acceptance of the revised Terms.',
    ],
  },
  {
    id: 'contact',
    title: '10. Contact',
    content: [
      'If you have questions about these Terms, you may contact us through the platform.',
    ],
  },
]
</script>

<template>
  <div class="bg-background text-foreground">
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="overflow-hidden rounded-[32px] bg-neutral-100 dark:bg-neutral-800">
        <div class="grid gap-8 px-6 py-10 sm:px-10 lg:grid-cols-[1.1fr_0.9fr] lg:px-12 xl:px-16">
          <div class="max-w-3xl">
            <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
              Legal
            </p>

            <h1 class="mt-4 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl xl:text-6xl">
              Terms of Service
            </h1>

            <p class="mt-3 text-sm text-foreground/45">
              Last updated: March 17, 2026
            </p>

            <p class="mt-5 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base">
              These Terms of Service govern your access to and use of the Relora platform, including browsing, listing items, bidding, and completing transactions.
            </p>
          </div>

          <div class="flex items-end lg:justify-end">
            <div class="w-full rounded-[28px] bg-background p-6">
              <p class="text-[11px] uppercase tracking-[0.22em] text-foreground/60">
                Summary
              </p>
              <p class="mt-3 text-sm leading-7 text-foreground/65">
                By using Relora, you agree to follow the platform rules, provide accurate information, and respect the auction process, payment obligations, and listing standards.
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
            <p class="text-[11px] uppercase tracking-[0.22em] text-foreground/60">
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
                <span class="mt-2 h-1.5 w-1.5 rounded-full bg-foreground/70"></span>
                <span>{{ item }}</span>
              </li>
            </ul>
          </section>
        </div>
      </div>
    </section>

    <section class="mx-auto max-w-7xl px-4 pb-16 sm:px-6 lg:px-8">
      <div class="rounded-[32px] border bg-background px-6 py-8 text-center sm:px-10">
        <p class="text-sm text-foreground/45">
          Relora · Terms of Service
        </p>
        <p class="mt-3 text-lg font-medium text-foreground/65">
          Clear rules support a cleaner marketplace.
        </p>
      </div>
    </section>
  </div>
</template>