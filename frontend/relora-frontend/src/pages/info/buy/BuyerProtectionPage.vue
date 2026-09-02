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
    title: 'Pay through Relora',
    description:
      'Complete the payment through Relora after winning an auction. Payments made outside the platform are not protected.',
  },
  {
    number: '02',
    title: 'Seller ships the item',
    description:
      'The seller must ship the item using a trackable delivery method and provide shipment information.',
  },
  {
    number: '03',
    title: 'Inspect your order',
    description:
      'After delivery, you have 48 hours to inspect the item and report a qualifying problem.',
  },
  {
    number: '04',
    title: 'Order is completed',
    description:
      'If no problem is reported during the protection window, the order is completed and the seller becomes eligible for payment.',
  },
]

const sections: ProtectionSection[] = [
  {
    id: 'overview',
    title: '1. What Buyer Protection Means',
    content: [
      'Buyer Protection is designed to make purchases on Relora safer. It applies to eligible orders that are paid for and completed entirely through the Relora platform.',
      'After the item is delivered, you receive a limited inspection period. During this time, you may report a problem if the item was not received, is materially different from the listing, or appears to be counterfeit.',
      'Buyer Protection is not a general return policy and does not cover a change of mind, an incorrect size choice, or details that were clearly disclosed before bidding.',
    ],
  },
  {
    id: 'before-bidding',
    title: '2. Before Placing a Bid',
    content: [
      'Every bid placed on Relora is intended to be binding. Before bidding, carefully review the complete listing and make sure that the item is suitable for you.',
    ],
    list: [
      'Read the full title, description and condition information',
      'Review all available photographs',
      'Check the listed size and available measurements',
      'Review disclosed defects, alterations and signs of wear',
      'Ask for additional information before placing a bid',
      'Confirm that the delivery destination is correct',
    ],
    note:
      'Buyer Protection does not replace the buyer’s responsibility to review the listing before bidding.',
  },
  {
    id: 'covered',
    title: '3. What May Be Covered',
    content: [
      'A purchase may be eligible for Buyer Protection when one of the following problems can be supported by the order information and evidence provided.',
    ],
    list: [
      'The item was not delivered',
      'The buyer received a different item',
      'The item appears to be counterfeit or an unauthorised replica',
      'The item has major damage that was not disclosed in the listing',
      'The condition is materially worse than described',
      'Important components or accessories shown in the listing are missing',
      'The item arrived significantly damaged during delivery',
    ],
  },
  {
    id: 'not-covered',
    title: '4. What Is Not Covered',
    content: [
      'Buyer Protection is intended for serious order problems. It is not intended to provide unrestricted returns for auction purchases.',
    ],
    list: [
      'You changed your mind after winning the auction',
      'The item does not fit or the size was selected incorrectly',
      'You no longer want or need the item',
      'A defect or sign of wear was clearly shown or described',
      'Minor differences caused by lighting, screens or photography',
      'Normal wear expected from the item’s disclosed condition',
      'The item was altered, washed, repaired or damaged after delivery',
      'Payment or communication was moved outside Relora',
      'The problem was reported after the protection period expired',
    ],
  },
  {
    id: 'delivery',
    title: '5. Delivery and Inspection Period',
    content: [
      'The seller must use a delivery method that provides shipment tracking. Buyers should monitor the order status and make sure that the delivery address is complete and accurate.',
      'Once the order is marked as delivered, the buyer has 48 hours to inspect the item. The seller’s payment remains pending during this inspection period.',
      'If no problem is reported before the inspection period expires, the order may be automatically completed and the seller may become eligible to receive the payment.',
    ],
    list: [
      'Check the package before discarding any packaging',
      'Compare the received item with the listing photographs',
      'Inspect the condition, label, size and included accessories',
      'Keep the shipping label and original packaging until the order is completed',
      'Report any serious problem as soon as it is discovered',
    ],
  },
  {
    id: 'reporting',
    title: '6. Reporting a Problem',
    content: [
      'To request assistance, open the relevant order and select “Report a problem” before the 48-hour inspection period expires.',
      'Describe the issue clearly and provide evidence that shows how the received item differs from the listing.',
    ],
    list: [
      'Photographs of the complete item',
      'Close-up photographs of the reported problem',
      'Photographs of labels, serial numbers or authenticity details',
      'Photographs of the package and shipping label',
      'A delivery or unboxing video, when available',
      'Any other information requested during the review',
    ],
    note:
      'Do not return the item without instructions from Relora. An unauthorised return may delay or affect the review.',
  },
  {
    id: 'review',
    title: '7. How a Case Is Reviewed',
    content: [
      'When a qualifying problem is reported, Relora may pause completion of the order while the case is reviewed.',
      'Relora may examine the original listing, photographs, condition description, tracking information, payment records and evidence submitted by both parties.',
      'The buyer and seller may be asked to provide additional information within a specified period. Failure to respond may affect the final decision.',
    ],
    list: [
      'Approve a return and refund',
      'Reject the claim and complete the order',
      'Request additional evidence from the buyer or seller',
      'Require the item to be returned using tracked delivery',
      'Take action against an account that violates platform rules',
    ],
  },
  {
    id: 'returns-refunds',
    title: '8. Returns and Refunds',
    content: [
      'When a return is approved, the buyer must follow the return instructions provided by Relora and use a trackable delivery method.',
      'The item must be returned in the same condition in which it was received. It must not be worn, washed, repaired, altered or damaged.',
      'A refund may be issued after the return has been delivered and the required conditions have been verified. Refund processing times depend on the payment provider and the buyer’s bank.',
      'Responsibility for return delivery costs may depend on the reason for the return and the outcome of the case.',
    ],
  },
  {
    id: 'authenticity',
    title: '9. Counterfeit and Authenticity Concerns',
    content: [
      'Counterfeit, replica and intentionally misrepresented items are prohibited on Relora.',
      'Relora may review listings before publication and may request additional photographs, purchase records, serial numbers or other supporting information from a seller.',
      'Moderation reduces risk but does not mean that every item has received a physical expert authentication. Buyers should review each listing carefully and report any authenticity concerns during the protection period.',
      'If an item is determined to violate Relora’s authenticity rules, the listing may be removed and action may be taken against the seller’s account.',
    ],
  },
  {
    id: 'off-platform',
    title: '10. Off-Platform Transactions',
    content: [
      'Buyer Protection applies only when the auction, payment and order process remain on Relora.',
      'Never send money directly to a seller, use an external payment link, or agree to complete the transaction privately.',
      'Relora cannot reliably investigate or protect payments, deliveries or agreements made outside the platform.',
    ],
  },
  {
    id: 'buyer-responsibilities',
    title: '11. Buyer Responsibilities',
    content: [
      'To remain eligible for Buyer Protection, buyers must act honestly, respond to requests for information and preserve the item while a case is being reviewed.',
    ],
    list: [
      'Provide complete and accurate information',
      'Submit original and relevant evidence',
      'Do not make false or misleading claims',
      'Do not alter the item during a review',
      'Follow all return and delivery instructions',
      'Communicate respectfully with the seller and Relora support',
    ],
  },
  {
    id: 'contact',
    title: '12. Contact and Support',
    content: [
      'If you have a problem with an active order, use the “Report a problem” option from the order page.',
      'For general questions about Buyer Protection, delivery or refunds, contact Relora through the Contact us page.',
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
              Shop premium and archival fashion with greater confidence.
              Eligible purchases are protected when the payment and order
              process are completed through Relora.
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
                Pay through Relora, receive tracked delivery and inspect your
                order within 48 hours. Report serious problems before the
                protection period expires.
              </p>

              <div class="mt-5 space-y-3">
                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Secure platform payment
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Trackable delivery
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    48-hour inspection period
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
          Protected from payment to delivery
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
          Bid carefully. Pay securely. Inspect every delivery.
        </p>
      </div>
    </section>
  </div>
</template>