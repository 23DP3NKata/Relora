<script setup lang="ts">
import { RouterLink } from 'vue-router'

type ProtectionSection = {
  id: string
  title: string
  content?: string[]
  list?: string[]
  note?: string
}

const protectionSteps = [
  {
    number: '01',
    title: 'Wait for payment',
    description:
      'Do not ship until the order status shows that the buyer has paid.',
  },
  {
    number: '02',
    title: 'Ship with tracking',
    description:
      'Send the item to the address on the order within the handling time and enter the carrier and tracking number.',
  },
  {
    number: '03',
    title: 'Keep your evidence',
    description:
      'Keep photos of the item, the packaging and the shipping receipt until the order is completed.',
  },
  {
    number: '04',
    title: 'Order is completed',
    description:
      'The order is completed when the buyer confirms receipt or when a dispute is decided in your favour.',
  },
]

const sections: ProtectionSection[] = [
  {
    id: 'overview',
    title: '1. What Seller Protection Means',
    content: [
      'Seller Protection on Relora means that a buyer cannot cancel an order on their own. If the buyer opens a dispute, an administrator reviews it and makes a decision.',
      'Payments on Relora are test payments. Sellers do not receive real money and there are no payouts or fees.',
    ],
  },
  {
    id: 'listings',
    title: '2. Accurate Listings',
    content: [
      'An accurate listing is the best protection against disputes. Every listing is checked by an administrator before it can go to auction.',
    ],
    list: [
      'Use clear photos (5 to 15 photos) that show the real item',
      'Describe the condition, defects and signs of wear honestly',
      'Choose the correct size and add measurements when you can',
      'Set a correct shipping price and the countries you ship to',
    ],
    note:
      'Counterfeit and replica items are not allowed. Such listings are rejected.',
  },
  {
    id: 'payment',
    title: '3. Payment',
    content: [
      'When the auction ends, an order is created for the winner with the status "awaiting payment". The buyer has 3 days to pay.',
      'If the buyer does not pay in time, the order expires and the lot becomes unsold. You can start a new auction for it from your listings.',
    ],
  },
  {
    id: 'shipping',
    title: '4. Shipping',
    content: [
      'After payment the order shows the delivery address and the date by which the item must be shipped. This date is calculated from the handling time in your listing (in business days).',
      'Ship the item with a trackable delivery method and enter the carrier and tracking number on the order page. The buyer receives an email about the shipment.',
    ],
    list: [
      'Ship only to the address shown on the order',
      'Pack the item carefully',
      'Keep the shipping receipt',
    ],
    note:
      'Orders that are not shipped in time are shown to administrators.',
  },
  {
    id: 'disputes',
    title: '5. When a Buyer Opens a Dispute',
    content: [
      'A buyer can open a dispute for a paid order before confirming receipt, for example if the item was not received, is damaged or differs from the listing.',
      'The buyer adds a description and photos. An administrator compares them with the listing and makes a decision with a short explanation. You receive an email with the decision.',
    ],
    list: [
      'In favour of the buyer - the order is cancelled',
      'In favour of the seller - the order is completed',
    ],
  },
  {
    id: 'returns',
    title: '6. Returns',
    content: [
      'Relora does not process returns. A buyer cannot send an item back through the platform, and a change of mind or a wrong size is not a reason for a dispute.',
    ],
  },
  {
    id: 'evidence',
    title: '7. Evidence You Should Keep',
    list: [
      'Photos of the item before packing',
      'Photos of the packed parcel and the shipping label',
      'The shipping receipt and tracking number',
    ],
  },
  {
    id: 'off-platform',
    title: '8. Off-Platform Transactions',
    content: [
      'Keep the sale and the delivery on Relora. Agreements made outside the platform cannot be reviewed in a dispute.',
    ],
  },
  {
    id: 'support',
    title: '9. Seller Support',
    content: [
      'For questions about selling, send a request through the Contact us page. You will get a confirmation email.',
    ],
  },
]
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
              Selling on Relora
            </p>

            <h1
              class="mt-4 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl xl:text-6xl"
            >
              Seller Protection
            </h1>

            <p class="mt-3 text-sm text-foreground/45">
              Last updated: July 24, 2026
            </p>

            <p
              class="mt-5 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base"
            >
              Learn how accurate listings, tracked delivery and clear evidence
              help you when a buyer opens a dispute.
            </p>

            <p class="mt-4 text-xs text-foreground/40">
              Relora is not a real working marketplace yet. Payments are test
              payments and no real money is transferred.
            </p>

            <div class="mt-7 flex flex-wrap gap-3">
              <RouterLink
                to="/sell"
                class="inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-6 text-sm font-medium text-background transition hover:opacity-80"
              >
                Sell an item
              </RouterLink>

              <RouterLink
                to="/orders"
                class="inline-flex min-h-11 items-center justify-center rounded-full border px-6 text-sm font-medium text-foreground transition hover:bg-neutral-100 dark:hover:bg-neutral-800"
              >
                View sales
              </RouterLink>
            </div>
          </div>

          <div class="flex items-end lg:justify-end">
            <div class="w-full rounded-[28px] bg-background p-6">
              <p
                class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
              >
                Protection summary
              </p>

              <p class="mt-3 text-sm leading-7 text-foreground/65">
                Ship only after payment, use the order address, add a tracking
                number and keep evidence until the order is completed.
              </p>

              <div class="mt-5 space-y-3">
                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Ship only paid orders
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Tracked delivery to the order address
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Disputes reviewed by an administrator
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Protection process -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="mb-8">
        <p
          class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
        >
          Protection process
        </p>

        <h2
          class="mt-3 text-3xl font-semibold tracking-tight text-foreground"
        >
          Protect every successful sale
        </h2>
      </div>

      <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <article
          v-for="step in protectionSteps"
          :key="step.number"
          class="rounded-[28px] border bg-background p-6"
        >
          <span
            class="text-xs font-medium tracking-[0.18em] text-foreground/40"
          >
            {{ step.number }}
          </span>

          <h3
            class="mt-8 text-lg font-semibold tracking-tight text-foreground"
          >
            {{ step.title }}
          </h3>

          <p class="mt-3 text-sm leading-7 text-foreground/60">
            {{ step.description }}
          </p>
        </article>
      </div>
    </section>

    <!-- Main content -->
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
          Relora · Seller Protection
        </p>

        <p class="mt-3 text-xl font-medium text-foreground/70">
          List accurately. Ship securely. Keep your evidence.
        </p>

        <RouterLink
          to="/sell"
          class="mt-6 inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-7 text-sm font-medium text-background transition hover:opacity-80"
        >
          Create a listing
        </RouterLink>
      </div>
    </section>
  </div>
</template>