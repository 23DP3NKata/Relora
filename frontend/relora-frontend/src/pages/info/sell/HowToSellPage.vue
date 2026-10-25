<script setup lang="ts">
import { RouterLink } from 'vue-router'

type SellSection = {
  id: string
  title: string
  content?: string[]
  list?: string[]
  note?: string
}

const sellingSteps = [
  {
    number: '01',
    title: 'Create your listing',
    description:
      'Add photos, item details, condition, size, starting price and shipping conditions.',
  },
  {
    number: '02',
    title: 'Pass moderation',
    description:
      'An administrator checks the listing and approves or rejects it with a reason.',
  },
  {
    number: '03',
    title: 'Start the auction',
    description:
      'Choose the auction duration. The lot appears in the catalog and buyers can bid.',
  },
  {
    number: '04',
    title: 'Ship the item',
    description:
      'After the buyer pays, ship the item and add the carrier and tracking number.',
  },
]

const sections: SellSection[] = [
  {
    id: 'overview',
    title: '1. Selling on Relora',
    content: [
      'Relora is an auction platform for used clothing, shoes and accessories. Every registered user can both buy and sell.',
      'Every listing is checked by an administrator before it can go to auction. The sale, payment and delivery all happen on Relora.',
    ],
  },
  {
    id: 'eligible-items',
    title: '2. What You Can Sell',
    content: [
      'You can sell items for women, men or unisex in these categories:',
    ],
    list: [
      'Clothing',
      'Shoes',
      'Bags',
      'Accessories',
      'Jewellery',
      'Watches',
    ],
    note:
      'Only list items that you own and that you can ship when the auction ends.',
  },
  {
    id: 'prohibited-items',
    title: '3. Items You Must Not List',
    list: [
      'Counterfeit or replica items',
      'Stolen goods or items you do not own',
      'Items with hidden damage',
      'Illegal or dangerous goods',
    ],
    note:
      'Such listings are rejected by the administrator.',
  },
  {
    id: 'create-listing',
    title: '4. Create a Listing',
    content: [
      'The listing form has 7 steps: start, category, details, measurements, photos, price and shipping, and review. Each step checks the required fields before you can continue.',
      'Required details are the title (5 to 120 characters), description (100 to 2000 characters), brand, size, condition, main colour and the country where the item is located.',
      'Materials, measurements, model name, production year and purchase year are optional but help buyers.',
    ],
  },
  {
    id: 'photos',
    title: '5. Photos',
    content: [
      'Add 5 to 15 photos in JPEG, PNG or WebP format, each up to 10 MB. Photos are made smaller in the browser before upload.',
      'Choose a cover photo - it is shown in the catalog. By default it is the first photo.',
    ],
    list: [
      'Show the whole item from the front and back',
      'Add close-ups of labels and details',
      'Show every defect clearly',
      'Use natural light and a plain background',
    ],
  },
  {
    id: 'condition',
    title: '6. Condition and Vintage',
    content: [
      'Choose the condition that matches the item: new, used or refurbished. Always describe defects in the description.',
      'An item can be marked as vintage if it is at least 15 years old. If you do not know the production year, explain in the vintage notes why the item is vintage.',
    ],
  },
  {
    id: 'price',
    title: '7. Price and Shipping',
    content: [
      'Set the starting price - bidding starts from this amount. Also set the shipping price, the country you ship from and the countries you ship to (or "Worldwide").',
      'Set the handling time - how many business days (1 to 30) you need to ship the item after payment.',
    ],
  },
  {
    id: 'moderation',
    title: '8. Drafts and Moderation',
    content: [
      'A new listing is saved as a draft. When it is ready, submit it for moderation and its status changes to "pending review".',
      'An administrator approves or rejects the listing. A rejected listing shows the reason in your listings. After you fix it, it becomes a draft again and you can submit it again.',
    ],
    list: [
      'You can edit a listing with the status draft, pending review or rejected',
      'You can delete only a draft',
    ],
  },
  {
    id: 'auction',
    title: '9. Start the Auction',
    content: [
      'When the listing is approved, start the auction from your listings and choose the duration: 1 hour, 6 hours, 3 days, 7 days or 14 days.',
      'The current price starts from your starting price and the lot appears in the catalog.',
    ],
  },
  {
    id: 'during-auction',
    title: '10. During the Auction',
    content: [
      'Every new bid must be higher than the current price. All viewers see new bids without reloading the page.',
      'If a bid is placed in the last 10 seconds, the auction is extended by 30 seconds. You cannot bid on your own lot.',
    ],
  },
  {
    id: 'auction-end',
    title: '11. When the Auction Ends',
    content: [
      'The auction ends automatically. The highest bidder wins and an order is created with the status "awaiting payment". The buyer has 3 days to pay.',
      'If there were no bids or the buyer did not pay in time, the lot becomes unsold and you can start a new auction for it.',
    ],
  },
  {
    id: 'shipping',
    title: '12. Ship the Item',
    content: [
      'After payment you can see the delivery address and the date by which you must ship. Ship the item with tracking and enter the carrier and tracking number on the order page.',
      'The buyer receives an email about the shipment. Orders that are not shipped in time are shown to administrators.',
    ],
  },
  {
    id: 'completion',
    title: '13. Completion and Disputes',
    content: [
      'The order is completed when the buyer confirms receipt.',
      'If there is a problem, the buyer can open a dispute. An administrator decides it: in favour of the buyer the order is cancelled, in favour of the seller the order is completed. Relora does not process returns.',
    ],
  },
  {
    id: 'payments',
    title: '14. Payments',
    content: [
      'Payments on Relora are test payments. Sellers do not receive real money and there are no payouts or seller fees.',
    ],
  },
  {
    id: 'support',
    title: '15. Seller Support',
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
              How to Sell
            </h1>

            <p class="mt-3 text-sm text-foreground/45">
              Last updated: July 24, 2026
            </p>

            <p
              class="mt-5 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base"
            >
              List your clothing, shoes and accessories, reach interested
              buyers and manage the auction and delivery on Relora.
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
                to="/listings"
                class="inline-flex min-h-11 items-center justify-center rounded-full border px-6 text-sm font-medium text-foreground transition hover:bg-neutral-100 dark:hover:bg-neutral-800"
              >
                My listings
              </RouterLink>
            </div>
          </div>

          <div class="flex items-end lg:justify-end">
            <div class="w-full rounded-[28px] bg-background p-6">
              <p
                class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
              >
                Seller summary
              </p>

              <p class="mt-3 text-sm leading-7 text-foreground/65">
                Create an accurate listing, pass moderation, start your auction
                and ship the item with tracking after the buyer has paid.
              </p>

              <div class="mt-5 space-y-3">
                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Authentic items only
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Clear photos and descriptions
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Tracked delivery after payment
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Selling process -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="mb-8">
        <p
          class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
        >
          Selling process
        </p>

        <h2
          class="mt-3 text-3xl font-semibold tracking-tight text-foreground"
        >
          From listing to delivery
        </h2>
      </div>

      <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <article
          v-for="step in sellingSteps"
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

            <nav class="mt-4 max-h-[calc(100vh-12rem)] space-y-3 overflow-y-auto pr-2">
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
          Relora · How to Sell
        </p>

        <p class="mt-3 text-xl font-medium text-foreground/70">
          Ready to give your item its next owner?
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