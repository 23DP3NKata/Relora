<script setup lang="ts">
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
    title: 'Pay for the order',
    description:
      'After winning an auction, enter your delivery address and pay within 3 days. It is a test payment and no real money is charged.',
  },
  {
    number: '02',
    title: 'Seller ships the item',
    description:
      'The seller ships the item within the handling time from the listing and adds the carrier and tracking number.',
  },
  {
    number: '03',
    title: 'Check the delivery',
    description:
      'You receive an email when the item is shipped. When it arrives, compare it with the listing.',
  },
  {
    number: '04',
    title: 'Confirm or open a dispute',
    description:
      'Confirm receipt to complete the order. If something is wrong, open a dispute instead of confirming.',
  },
]

const sections: ProtectionSection[] = [
  {
    id: 'overview',
    title: '1. What Buyer Protection Means',
    content: [
      'Buyer Protection on Relora means that a buyer can open a dispute when there is a serious problem with an order. An administrator reviews the dispute and makes a decision.',
      'Relora does not process returns or refunds. Payments on Relora are test payments, so no money is transferred between the buyer and the seller.',
    ],
  },
  {
    id: 'before-bidding',
    title: '2. Before Placing a Bid',
    content: [
      'A bid cannot be withdrawn after it is placed. Before bidding, review the listing carefully.',
    ],
    list: [
      'Read the title, description and condition',
      'Review all photographs',
      'Check the size and measurements',
      'Check the shipping price and the countries the seller ships to',
    ],
  },
  {
    id: 'payment',
    title: '3. Payment',
    content: [
      'When an auction ends, the winner gets an order with the status "awaiting payment". The total is the winning bid plus the shipping price.',
      'The buyer enters a delivery address and confirms the payment within 3 days. The seller must ship to the selected country. If the order is not paid in time, it expires and the seller can list the item again.',
    ],
  },
  {
    id: 'delivery',
    title: '4. Shipping and Delivery',
    content: [
      'After payment the seller has to ship the item within the handling time shown in the listing (in business days) and enter the carrier and tracking number.',
      'The buyer receives an email when the item is shipped and can see the tracking number on the order page. Administrators can see orders that were not shipped in time.',
      'When the item arrives, the buyer confirms receipt and the order is completed.',
    ],
  },
  {
    id: 'covered',
    title: '5. When You Can Open a Dispute',
    content: [
      'A dispute can be opened for a paid order that is not completed or cancelled, so open it before confirming receipt.',
    ],
    list: [
      'The item was not shipped',
      'The item was not received',
      'A different item was received',
      'The item is significantly different from the description',
      'The item is damaged',
      'The item may be counterfeit',
    ],
  },
  {
    id: 'not-covered',
    title: '6. What Is Not a Reason for a Dispute',
    list: [
      'You changed your mind after winning the auction',
      'The item does not fit or the size was chosen incorrectly',
      'A defect was clearly shown or described in the listing',
      'Small colour differences caused by photos or screens',
    ],
  },
  {
    id: 'reporting',
    title: '7. Opening a Dispute',
    content: [
      'Open the order and choose "Report a problem". Select a reason and describe the problem (up to 2000 characters).',
      'You can add up to 10 photos as evidence (JPEG, PNG or WebP, up to 10 MB each).',
    ],
    note:
      'Do not send the item back. Relora does not process returns.',
  },
  {
    id: 'review',
    title: '8. How a Dispute Is Decided',
    content: [
      'When a dispute is opened, the order gets the status "disputed" and administrators receive an email.',
      'An administrator reviews the description and photos and makes a decision with a short explanation. The buyer and the seller receive an email with the decision.',
    ],
    list: [
      'In favour of the buyer - the order is cancelled',
      'In favour of the seller - the order is completed',
    ],
  },
  {
    id: 'authenticity',
    title: '9. Counterfeit Items',
    content: [
      'Counterfeit and replica items are not allowed. Every listing is checked by an administrator before it can go to auction.',
      'Moderation is based on the listing photos and description and is not a physical authentication. If you suspect a counterfeit, open a dispute with the reason "possible counterfeit".',
    ],
  },
  {
    id: 'contact',
    title: '10. Contact and Support',
    content: [
      'For a problem with an order, use "Report a problem" on the order page.',
      'For other questions, send a request through the Contact us page. You will get a confirmation email.',
    ],
  },
]
</script>

<template>
  <div class="bg-background text-foreground">
    <!-- Hero -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="overflow-hidden rounded-[32px] bg-neutral-100 dark:bg-neutral-800">
        <div
          class="grid gap-8 px-6 py-10 sm:px-10 lg:grid-cols-[1.1fr_0.9fr] lg:px-12 xl:px-16"
        >
          <div class="max-w-3xl">
            <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
              Buying on Relora
            </p>

            <h1
              class="mt-4 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl xl:text-6xl"
            >
              Buyer Protection
            </h1>

            <p class="mt-3 text-sm text-foreground/45">
              Last updated: July 24, 2026
            </p>

            <p
              class="mt-5 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base"
            >
              If something goes wrong with an order, you can open a dispute
              and an administrator will make a decision.
            </p>

            <p class="mt-4 text-xs text-foreground/40">
              Relora is not a real working marketplace yet. Payments are test
              payments and no real money is transferred.
            </p>
          </div>

          <div class="flex items-end lg:justify-end">
            <div class="w-full rounded-[28px] bg-background p-6">
              <p
                class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
              >
                Protection summary
              </p>

              <p class="mt-3 text-sm leading-7 text-foreground/65">
                Pay for the order, wait for the tracked delivery and check the
                item. If there is a serious problem, open a dispute before
                confirming receipt.
              </p>

              <div class="mt-5 space-y-3">
                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Test payment within 3 days
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Tracking number from the seller
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Dispute reviewed by an administrator
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- How it works -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="mb-8">
        <p class="text-[11px] uppercase tracking-[0.22em] text-foreground/60">
          How it works
        </p>

        <h2 class="mt-3 text-3xl font-semibold tracking-tight text-foreground">
          From payment to delivery
        </h2>
      </div>

      <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <article
          v-for="step in protectionSteps"
          :key="step.number"
          class="rounded-[28px] border bg-background p-6"
        >
          <span class="text-xs font-medium tracking-[0.18em] text-foreground/40">
            {{ step.number }}
          </span>

          <h3 class="mt-8 text-lg font-semibold tracking-tight text-foreground">
            {{ step.title }}
          </h3>

          <p class="mt-3 text-sm leading-7 text-foreground/60">
            {{ step.description }}
          </p>
        </article>
      </div>
    </section>

    <!-- Page content -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="grid gap-8 lg:grid-cols-[260px_minmax(0,1fr)]">
        <aside class="hidden lg:block">
          <div class="sticky top-36 rounded-[28px] border bg-background p-5">
            <p
              class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
            >
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

    <!-- Bottom card -->
    <section class="mx-auto max-w-7xl px-4 pb-16 sm:px-6 lg:px-8">
      <div
        class="rounded-[32px] border bg-background px-6 py-8 text-center sm:px-10"
      >
        <p class="text-sm text-foreground/45">
          Relora · Buyer Protection
        </p>

        <p class="mt-3 text-lg font-medium text-foreground/65">
          Bid carefully. Check every delivery.
        </p>
      </div>
    </section>
  </div>
</template>