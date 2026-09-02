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
    title: 'Receive confirmed payment',
    description:
      'Do not ship until the order shows that the buyer’s payment has been successfully completed.',
  },
  {
    number: '02',
    title: 'Ship with tracking',
    description:
      'Send the item to the address shown on the order and upload valid end-to-end tracking information.',
  },
  {
    number: '03',
    title: 'Keep your evidence',
    description:
      'Save photographs of the item, packaging, shipping label and delivery receipt until the order is completed.',
  },
  {
    number: '04',
    title: 'Receive your payout',
    description:
      'Your payout may become available after delivery, the buyer inspection period and any required review.',
  },
]

const sections: ProtectionSection[] = [
  {
    id: 'overview',
    title: '1. What Seller Protection Means',
    content: [
      'Seller Protection is designed to support eligible sellers when an order is completed through Relora and the seller follows the platform’s listing, payment and delivery requirements.',
      'Relora may review order information, listing details, tracking records and evidence submitted by both parties when a buyer reports a problem.',
      'Seller Protection does not guarantee the outcome of every case. Eligibility depends on the circumstances of the transaction and the evidence available.',
    ],
  },
  {
    id: 'eligibility',
    title: '2. Seller Protection Eligibility',
    content: [
      'An order may be eligible for Seller Protection when the entire transaction remains on Relora and the seller follows the required order process.',
    ],
    list: [
      'The item was listed and sold through Relora',
      'The buyer completed payment through the supported Relora checkout',
      'The seller shipped only after payment was confirmed',
      'The item was sent to the address displayed on the order',
      'The seller used a trackable delivery method',
      'Valid tracking information was added to the order',
      'The item matched the listing description and photographs',
      'The seller shipped within the required deadline',
      'The seller responded to requests for information',
    ],
    note:
      'Protection may be limited when an order is completed partially or entirely outside Relora.',
  },
  {
    id: 'covered',
    title: '3. Situations That May Be Covered',
    content: [
      'Relora may assist the seller when a buyer makes a claim that is inconsistent with the available order and delivery evidence.',
    ],
    list: [
      'The buyer claims that the item was not delivered, but valid tracking confirms delivery',
      'The buyer claims that the wrong item was received, but the seller has clear packing evidence',
      'The buyer reports damage that appears to have occurred after delivery',
      'The buyer returns an item that is different from the item originally sent',
      'The returned item is missing components that were included in the original order',
      'The buyer submits misleading, altered or inconsistent evidence',
      'A buyer repeatedly opens unsupported claims',
      'A delivery issue requires review of tracking and shipping records',
    ],
  },
  {
    id: 'not-covered',
    title: '4. Situations That Are Not Covered',
    content: [
      'Seller Protection does not cover losses caused by inaccurate listings, prohibited items, insufficient delivery evidence or failure to follow Relora’s order process.',
    ],
    list: [
      'The item was counterfeit, stolen, prohibited or illegally sold',
      'The listing contained inaccurate or misleading information',
      'Important defects were not disclosed',
      'The item condition was materially worse than described',
      'The seller shipped before payment was confirmed',
      'The item was sent to an address outside the Relora order',
      'The seller used an untracked delivery method',
      'The tracking number was invalid or belonged to another parcel',
      'The item was shipped after the required deadline without approval',
      'The parcel was inadequately packaged',
      'The seller completed payment or communication outside Relora',
      'The seller did not respond during a dispute review',
    ],
  },
  {
    id: 'accurate-listings',
    title: '5. Accurate Listings',
    content: [
      'Seller Protection begins with an accurate listing. Buyers must receive the same item that was shown and described before they placed a bid.',
      'The listing must accurately describe the item’s brand, size, condition, colour, material, measurements, included accessories and known defects.',
      'Moderation or approval of a listing does not remove the seller’s responsibility for the item’s authenticity, legality and condition.',
    ],
    list: [
      'Use photographs of the actual item',
      'Show all visible defects and signs of wear',
      'Provide accurate measurements',
      'Describe repairs, alterations and missing components',
      'Use the correct brand, category and condition',
      'Do not edit photographs in a misleading way',
    ],
  },
  {
    id: 'authenticity',
    title: '6. Authenticity Requirements',
    content: [
      'Only authentic items may be listed on Relora. Counterfeit, replica and intentionally misrepresented items are prohibited.',
      'Sellers should retain any available proof of purchase, serial numbers, authenticity cards, receipts or other supporting documents.',
      'Relora may request additional evidence before approving a listing, releasing a payout or resolving a dispute.',
    ],
    note:
      'A seller may lose protection and face account restrictions when an item cannot be reasonably supported as authentic.',
  },
  {
    id: 'payment',
    title: '7. Confirmed Payments',
    content: [
      'The seller must not ship an item until the order shows that the buyer’s payment has been completed.',
      'Screenshots, messages or statements from the buyer are not payment confirmation. The order status shown by Relora should be treated as the source of truth.',
      'Never accept a direct bank transfer, external payment link or separate payment arrangement for a Relora order.',
    ],
    note:
      'If a buyer asks you to ship before payment is confirmed, do not send the item and contact Relora support.',
  },
  {
    id: 'shipping-address',
    title: '8. Shipping Address',
    content: [
      'The item must be shipped only to the delivery address shown on the Relora order.',
      'Do not accept a privately requested address change through messages, email or another platform.',
      'If the buyer states that the delivery address is incorrect, wait until the order is officially updated or cancelled before shipping.',
    ],
  },
  {
    id: 'tracked-delivery',
    title: '9. Tracked Delivery',
    content: [
      'Use a reliable delivery service that provides valid end-to-end tracking from acceptance of the parcel until delivery.',
      'The tracking number must relate to the correct order and should be uploaded as soon as the parcel has been shipped.',
    ],
    list: [
      'Use a delivery provider recognised in the destination country',
      'Keep the original shipping receipt',
      'Check that the tracking number is correct',
      'Make sure the parcel receives an acceptance scan',
      'Use signature confirmation for higher-value items when available',
      'Consider shipping insurance for valuable items',
      'Monitor tracking until the parcel is delivered',
    ],
  },
  {
    id: 'packaging',
    title: '10. Safe Packaging',
    content: [
      'The seller is responsible for packaging the item in a way that is appropriate for its type, value and delivery method.',
      'Insufficient packaging may reduce or remove Seller Protection when the item is damaged during delivery.',
    ],
    list: [
      'Photograph the item immediately before packing',
      'Use clean protective wrapping',
      'Protect the item from moisture',
      'Use a strong outer package',
      'Protect fragile hardware and accessories',
      'Fill empty space to prevent movement',
      'Seal the parcel securely',
      'Photograph the sealed parcel and shipping label',
    ],
  },
  {
    id: 'evidence',
    title: '11. Evidence You Should Keep',
    content: [
      'Good evidence makes it easier to review a buyer claim. Keep all order-related evidence until the order has been completed and the payout has been received.',
    ],
    list: [
      'Original listing photographs',
      'Additional photographs showing the item’s condition',
      'Photographs of labels, serial numbers and authenticity details',
      'Photographs or video of the packing process',
      'Photographs of the sealed parcel',
      'Photograph of the shipping label',
      'Shipping receipt',
      'Tracking history',
      'Proof of parcel weight when available',
      'Relevant communication recorded through Relora',
    ],
    note:
      'Evidence should be original, clear and connected to the specific item and order.',
  },
  {
    id: 'delivery',
    title: '12. Delivery and Buyer Inspection',
    content: [
      'Tracking information may be used to determine when the parcel was delivered.',
      'After delivery, the buyer receives a limited period to inspect the item and report a qualifying problem.',
      'During the inspection period, the seller’s payout may remain pending.',
      'If the buyer does not report a problem within the applicable period, the order may be completed and the payout may become available.',
    ],
  },
  {
    id: 'reported-problem',
    title: '13. When a Buyer Reports a Problem',
    content: [
      'When a buyer reports a qualifying problem, Relora may pause completion of the order while the case is reviewed.',
      'The seller will be informed about the reported issue and may be asked to provide a response and supporting evidence.',
      'Respond clearly, honestly and within the deadline shown in the case.',
    ],
    list: [
      'Read the buyer’s claim carefully',
      'Respond to each reported issue',
      'Provide original photographs and shipping evidence',
      'Explain how the item matched the listing',
      'Do not threaten or pressure the buyer',
      'Do not arrange a private refund or return',
      'Continue communication through Relora',
    ],
  },
  {
    id: 'case-review',
    title: '14. How a Case Is Reviewed',
    content: [
      'Relora may review the listing, description, photographs, order history, tracking information, delivery records and evidence submitted by both parties.',
      'Relora may also request additional information from the seller, buyer, delivery provider or payment provider.',
      'The outcome depends on the available evidence and the circumstances of the order.',
    ],
    list: [
      'Complete the order in favour of the seller',
      'Approve a return to the seller',
      'Approve a full or partial refund',
      'Request additional evidence',
      'Require cooperation with the delivery provider',
      'Restrict an account involved in policy violations',
    ],
  },
  {
    id: 'returns',
    title: '15. Returned Items',
    content: [
      'When a return is approved, the buyer should follow the return instructions and use trackable delivery.',
      'The seller should inspect the returned parcel promptly after delivery.',
      'If the returned item is damaged, incomplete or different from the item originally sent, the seller must report the issue through the order as soon as possible.',
    ],
    list: [
      'Photograph the parcel before opening it',
      'Record the package condition and shipping label',
      'Consider recording the opening of higher-value returns',
      'Compare the returned item with the original listing evidence',
      'Check labels, serial numbers and distinguishing details',
      'Check that all accessories were returned',
      'Report any problem before the return review period expires',
    ],
    note:
      'Do not dispose of the packaging or alter the returned item while a return issue is being reviewed.',
  },
  {
    id: 'return-substitution',
    title: '16. Different or Altered Return',
    content: [
      'Returning a different item, removing components or intentionally damaging an item is prohibited.',
      'If the seller believes that the returned item has been substituted, altered or damaged after delivery, the seller should provide clear comparison evidence.',
      'Relora may compare serial numbers, labels, item details, parcel weight, photographs and other identifying characteristics.',
    ],
  },
  {
    id: 'payouts',
    title: '17. Seller Payouts',
    content: [
      'Seller payouts are processed through Relora’s supported payment provider.',
      'A payout may become available after delivery has been confirmed, the buyer inspection period has ended and no unresolved case remains.',
      'Payout availability and bank processing times may depend on seller verification, the payment provider and the receiving bank.',
      'Applicable marketplace fees, payment costs, refunds, adjustments or dispute-related amounts may be deducted from the seller’s balance.',
    ],
    note:
      'Seller Protection does not guarantee immediate payout availability or a specific bank processing time.',
  },
  {
    id: 'chargebacks',
    title: '18. Payment Disputes and Chargebacks',
    content: [
      'A buyer may sometimes dispute a payment directly with their bank or card provider. This process is separate from a standard Relora return request.',
      'When a payment dispute occurs, Relora may submit available order, delivery and communication evidence to the payment provider.',
      'The final decision may be made by the buyer’s bank or card network and cannot always be controlled by Relora.',
    ],
    list: [
      'Valid tracking and delivery confirmation',
      'The item listing and description',
      'Communication connected to the order',
      'Proof that the buyer used or acknowledged the item',
      'Evidence showing compliance with Relora policies',
    ],
  },
  {
    id: 'delivery-problems',
    title: '19. Lost or Damaged Parcels',
    content: [
      'If a parcel is delayed, lost or damaged, the seller may be required to contact the delivery provider and open a shipping claim.',
      'Responsibility for the loss may depend on the delivery service, insurance coverage, packaging and tracking information.',
      'Relora may request evidence of the shipping claim before resolving the order.',
    ],
    list: [
      'Shipping receipt',
      'Tracking history',
      'Proof of parcel value',
      'Photographs of the item and packaging',
      'Communication with the delivery provider',
      'The delivery provider’s final decision',
    ],
  },
  {
    id: 'buyer-abuse',
    title: '20. Buyer Abuse and Fraud',
    content: [
      'Relora may investigate buyers who submit false claims, manipulate evidence, return different items or repeatedly misuse Buyer Protection.',
      'Accounts involved in fraudulent or abusive behaviour may be restricted, suspended or terminated.',
      'Sellers should report suspicious behaviour through the relevant order instead of confronting the buyer outside the platform.',
    ],
  },
  {
    id: 'seller-responsibilities',
    title: '21. Seller Responsibilities',
    content: [
      'Seller Protection depends on the seller meeting their responsibilities throughout the listing and order process.',
    ],
    list: [
      'Sell only authentic items that you legally own',
      'Provide accurate listing information',
      'Disclose every known material defect',
      'Keep the item in the listed condition',
      'Ship only after confirmed payment',
      'Use the official order address',
      'Package the item securely',
      'Use trackable delivery',
      'Upload correct tracking information',
      'Keep transaction evidence',
      'Respond to support requests',
      'Follow all applicable laws and Relora policies',
    ],
  },
  {
    id: 'off-platform',
    title: '22. Off-Platform Transactions',
    content: [
      'Seller Protection applies only to eligible activity completed through Relora.',
      'Do not accept direct payments, external payment links or private delivery arrangements.',
      'Relora may be unable to verify or protect transactions completed outside the platform.',
    ],
  },
  {
    id: 'support',
    title: '23. Seller Support',
    content: [
      'For a problem with an active order, open the order and use the available support or dispute option.',
      'For general questions about shipping, returns, payment verification or payouts, contact Relora through the Contact us page.',
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
              Learn how accurate listings, confirmed payments, tracked
              delivery and clear evidence can help protect eligible Relora
              sellers.
            </p>

            <div class="mt-7 flex flex-wrap gap-3">
              <RouterLink
                to="/sell"
                class="inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-6 text-sm font-medium text-background transition hover:opacity-80"
              >
                Sell an item
              </RouterLink>

              <RouterLink
                to="/seller/orders"
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
                Ship only after confirmed payment, use the order address,
                provide valid tracking and keep evidence until the order and
                payout are completed.
              </p>

              <div class="mt-5 space-y-3">
                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Confirmed platform payment
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
                    Evidence-based dispute review
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