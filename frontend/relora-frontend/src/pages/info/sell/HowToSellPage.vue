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
      'Add clear photographs, item details, condition information, size and your starting price.',
  },
  {
    number: '02',
    title: 'Pass moderation',
    description:
      'Relora reviews the listing before it becomes available for auction.',
  },
  {
    number: '03',
    title: 'Start the auction',
    description:
      'Choose the auction duration and publish the approved item for buyers to bid on.',
  },
  {
    number: '04',
    title: 'Ship and get paid',
    description:
      'After payment, ship the item with tracking. Your payout becomes available after the order is completed.',
  },
]

const sections: SellSection[] = [
  {
    id: 'overview',
    title: '1. Selling on Relora',
    content: [
      'Relora is an auction marketplace for premium, designer and archival fashion. Sellers can create listings, receive bids and sell items directly to buyers through the platform.',
      'To provide a safer marketplace, listings may be reviewed before publication and sellers must provide accurate information about every item they offer.',
      'The complete sale, payment, delivery and order process should remain on Relora.',
    ],
  },
  {
    id: 'seller-account',
    title: '2. Prepare Your Seller Account',
    content: [
      'Before listing an item, make sure your Relora account contains accurate personal and contact information.',
      'You may also be required to complete seller onboarding and provide information requested by Relora or its payment provider before receiving payouts.',
    ],
    list: [
      'Use your real name and current contact information',
      'Confirm your email address',
      'Add a valid shipping location',
      'Complete identity or payment verification when requested',
      'Provide payout information through the approved payment provider',
      'Keep your account information updated',
    ],
    note:
      'The name and information used for seller verification should match the person or business receiving the payout.',
  },
  {
    id: 'eligible-items',
    title: '3. What You Can Sell',
    content: [
      'Relora is intended for authentic premium, designer, luxury and archival fashion items.',
      'Every item must be legally owned by the seller and available to ship when the auction ends.',
    ],
    list: [
      'Clothing',
      'Footwear',
      'Bags',
      'Accessories',
      'Jewellery permitted by Relora',
      'Collectible and archival fashion pieces',
    ],
    note:
      'Only list an item when you are certain that it is authentic and that you have the legal right to sell it.',
  },
  {
    id: 'prohibited-items',
    title: '4. Items You Must Not List',
    content: [
      'Listings that violate Relora rules or applicable law may be rejected or removed.',
    ],
    list: [
      'Counterfeit, replica or unauthorised items',
      'Stolen goods',
      'Items that you do not own',
      'Items containing misleading brand information',
      'Illegal, dangerous or restricted goods',
      'Items with intentionally hidden damage',
      'Digital products or services',
      'Items that cannot be shipped legally',
      'Listings created only to advertise another website or platform',
    ],
  },
  {
    id: 'prepare-item',
    title: '5. Prepare the Item',
    content: [
      'Inspect the item carefully before creating the listing. Buyers rely on your description and photographs when deciding whether to place a bid.',
      'Clean the item where appropriate, but do not use treatments that could damage it or hide its true condition.',
    ],
    list: [
      'Check the item for stains, holes, scratches and repairs',
      'Confirm the brand, model, size and material',
      'Locate labels, serial numbers and authenticity details',
      'Collect original packaging, receipts or accessories when available',
      'Measure the item accurately',
      'Make sure the item is ready to ship',
    ],
  },
  {
    id: 'photographs',
    title: '6. Take Clear Photographs',
    content: [
      'Photographs should show the actual item being sold. Do not use only catalogue images, promotional photographs or photographs taken from another seller.',
      'Use good lighting and show the item from several angles. Buyers should be able to understand the item’s condition before bidding.',
    ],
    list: [
      'Front and back of the complete item',
      'Brand label and size label',
      'Material and care labels',
      'Serial numbers or production codes when applicable',
      'Close-up photographs of important details',
      'Photographs of every defect or sign of wear',
      'Original packaging and included accessories',
      'Soles, hardware, corners and other high-wear areas',
    ],
    note:
      'Do not edit photographs in a way that changes the item’s colour, condition or appearance.',
  },
  {
    id: 'description',
    title: '7. Write an Accurate Description',
    content: [
      'The listing description must be clear, complete and truthful. Important information should not be hidden or left only in photographs.',
      'Describe the item as it currently exists, not how it looked when it was new.',
    ],
    list: [
      'Correct brand and item name',
      'Category and gender classification where relevant',
      'Labelled size and practical measurements',
      'Material and colour',
      'Current condition',
      'Known defects, repairs or alterations',
      'Included packaging and accessories',
      'Purchase history or proof of purchase when available',
    ],
  },
  {
    id: 'condition',
    title: '8. Select the Correct Condition',
    content: [
      'Choose the condition that most accurately represents the item. Selecting a better condition than the item actually has may result in a Buyer Protection claim.',
    ],
    list: [
      'New — unused and in new condition',
      'Like new — used very little with minimal signs of wear',
      'Good — used with visible but acceptable signs of wear',
      'Fair — heavily used with clear wear or defects',
    ],
    note:
      'Always describe specific defects even when they are normal for the selected condition.',
  },
  {
    id: 'price',
    title: '9. Set Your Auction Price',
    content: [
      'Choose a starting price that you would genuinely be willing to accept if the item receives only one valid bid.',
      'Before setting the price, consider the item’s brand, condition, rarity, recent market demand and comparable sales.',
      'Once a valid bid has been placed, you may not be able to change important auction details or withdraw the item without a valid reason.',
    ],
    list: [
      'Research similar items',
      'Consider the current condition',
      'Account for Relora seller fees',
      'Consider packaging and shipping responsibilities',
      'Do not artificially inflate the price using false bids',
    ],
  },
  {
    id: 'auction-duration',
    title: '10. Choose the Auction Duration',
    content: [
      'After the listing has been approved, you can select the available auction duration and start the auction.',
      'Depending on the options currently available on Relora, an auction may run for 3, 7 or 14 days.',
      'While the auction is active, buyers can place bids until the closing time.',
    ],
    list: [
      '3 days for a faster sale',
      '7 days for a standard auction',
      '14 days for additional exposure',
    ],
  },
  {
    id: 'moderation',
    title: '11. Listing Moderation',
    content: [
      'Listings may be reviewed before they appear publicly on Relora.',
      'Moderation helps identify incomplete, misleading, prohibited or suspicious listings. It does not replace the seller’s responsibility for authenticity and accuracy.',
      'Relora may approve the listing, reject it or request additional information.',
    ],
    list: [
      'Additional photographs',
      'More detailed condition information',
      'Measurements',
      'Proof of purchase',
      'Serial numbers or authenticity details',
      'Corrections to the category, brand or description',
    ],
    note:
      'Approval does not mean that Relora becomes the owner or seller of the item.',
  },
  {
    id: 'active-auction',
    title: '12. During the Auction',
    content: [
      'Keep the item safe and available while the auction is active.',
      'Respond honestly if Relora or a potential buyer requests additional information.',
      'You must not manipulate the auction or encourage buyers to complete the transaction outside Relora.',
    ],
    list: [
      'Do not place bids on your own item',
      'Do not ask another person to increase the price artificially',
      'Do not advertise external payment methods',
      'Do not sell the item elsewhere while valid bids are active',
      'Do not change or damage the item after listing it',
      'Keep all transaction-related communication on Relora',
    ],
  },
  {
    id: 'auction-end',
    title: '13. When the Auction Ends',
    content: [
      'When an auction ends with a valid winning bid, an order is created for the winning buyer.',
      'The buyer is required to complete payment within the period shown on the order.',
      'Do not ship the item before Relora confirms that the buyer’s payment has been completed.',
      'If the buyer does not pay within the required period, the order may be cancelled or handled according to Relora’s auction rules.',
    ],
  },
  {
    id: 'shipping',
    title: '14. Ship the Item',
    content: [
      'After payment has been confirmed, prepare the item for delivery and ship it within the deadline displayed on the order.',
      'Use a reliable delivery method that provides end-to-end tracking.',
    ],
    list: [
      'Confirm the shipping address shown on the order',
      'Photograph the item before packing it',
      'Use clean and protective packaging',
      'Protect the item from moisture and impact',
      'Use tracked delivery',
      'Keep the shipping receipt',
      'Add the tracking number to the Relora order',
      'Ship before the order deadline',
    ],
    note:
      'Only ship to the address shown in the Relora order. A privately requested address change may not be protected.',
  },
  {
    id: 'packaging',
    title: '15. Packaging Guidelines',
    content: [
      'The seller is responsible for packaging the item appropriately for delivery.',
      'Premium items should be protected from water, dirt, compression and impact. Poor packaging may make the seller responsible for damage that occurs during delivery.',
    ],
    list: [
      'Use a strong box or suitable mailing package',
      'Place the item inside protective wrapping',
      'Protect fragile hardware and accessories',
      'Do not place shipping tape directly on product packaging',
      'Remove or cover old delivery labels',
      'Seal the parcel securely',
    ],
  },
  {
    id: 'delivery',
    title: '16. Delivery and Buyer Inspection',
    content: [
      'Tracking information is used to follow the delivery and determine when the order was delivered.',
      'After delivery, the buyer receives the applicable period to inspect the item and report a qualifying problem.',
      'During this period, the seller may be asked to provide photographs, shipping documents or other information.',
    ],
  },
  {
    id: 'payout',
    title: '17. Receiving Your Payout',
    content: [
      'Seller payouts are processed through Relora’s supported payment provider.',
      'A payout may become available after delivery has been confirmed, the buyer inspection period has passed and no unresolved problem remains.',
      'The amount received by the seller may be reduced by applicable marketplace fees, payment processing costs, refunds, adjustments or other amounts displayed before the sale.',
      'Payment processing times may depend on the payment provider, the seller’s verification status and the seller’s bank.',
    ],
    note:
      'Complete seller onboarding and payment verification early to avoid unnecessary payout delays.',
  },
  {
    id: 'fees',
    title: '18. Seller Fees',
    content: [
      'Relora may charge a seller fee when an item is successfully sold.',
      'The applicable fee and estimated seller earnings should be displayed before the seller confirms the listing or auction.',
      'Fees may be deducted automatically from the sale amount before the seller receives the payout.',
    ],
    list: [
      'Marketplace commission',
      'Payment processing costs where applicable',
      'Refund or dispute adjustments where applicable',
      'Optional paid services selected by the seller',
    ],
  },
  {
    id: 'returns',
    title: '19. Returns and Buyer Protection',
    content: [
      'A buyer may report a problem when an item is not delivered, appears counterfeit, has significant undisclosed damage or is materially different from the listing.',
      'If a case is opened, Relora may pause completion of the order and request evidence from both parties.',
      'When a return is approved, the seller must cooperate with the return instructions and inspect the returned item after delivery.',
      'The seller’s legal responsibilities may also depend on whether the seller acts as a private individual or a professional seller.',
    ],
  },
  {
    id: 'seller-responsibilities',
    title: '20. Seller Responsibilities',
    content: [
      'Sellers are responsible for the legality, authenticity, accuracy, condition, packaging and timely shipment of the items they list.',
    ],
    list: [
      'List only authentic items that you own',
      'Describe every item accurately',
      'Disclose all known defects',
      'Use original photographs',
      'Keep the item available during the auction',
      'Ship the correct item on time',
      'Provide valid tracking information',
      'Respond to order and support requests',
      'Follow Relora’s policies and applicable law',
    ],
  },
  {
    id: 'tips',
    title: '21. Tips for a Successful Sale',
    content: [
      'The strongest listings give buyers enough information to bid confidently.',
    ],
    list: [
      'Use bright, sharp and consistent photographs',
      'Write a specific title instead of a generic one',
      'Provide useful measurements',
      'Show every defect clearly',
      'Choose a realistic starting price',
      'Reply quickly when more information is requested',
      'Package the item professionally',
      'Ship as soon as possible after payment',
    ],
  },
  {
    id: 'support',
    title: '22. Seller Support',
    content: [
      'For a problem with an active listing, auction or order, open the relevant page and use the available support option.',
      'For general questions about listing items, verification, shipping, fees or payouts, contact Relora through the Contact us page.',
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
              List premium and archival fashion, reach interested buyers and
              manage the complete auction, delivery and payout process through
              Relora.
            </p>

            <div class="mt-7 flex flex-wrap gap-3">
              <RouterLink
                to="/sell"
                class="inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-6 text-sm font-medium text-background transition hover:opacity-80"
              >
                Sell an item
              </RouterLink>

              <RouterLink
                to="/seller/listings"
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
          From listing to payout
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