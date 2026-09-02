<script setup lang="ts">
type PolicySection = {
  id: string
  title: string
  content?: string[]
  list?: string[]
  note?: string
}

const returnSteps = [
  {
    number: '01',
    title: 'Open your order',
    description:
      'Go to the relevant order in your Relora account and review its delivery and protection status.',
  },
  {
    number: '02',
    title: 'Report the return',
    description:
      'Select “Report a problem” or contact support within the applicable return or protection period.',
  },
  {
    number: '03',
    title: 'Wait for instructions',
    description:
      'Do not ship the item until you receive the return address and return instructions.',
  },
  {
    number: '04',
    title: 'Return with tracking',
    description:
      'Pack the item carefully, use trackable delivery and keep the shipping receipt until the case is completed.',
  },
]

const sections: PolicySection[] = [
  {
    id: 'overview',
    title: '1. Overview',
    content: [
      'This Returns Policy explains when an item purchased through Relora may be returned and how a return must be completed.',
      'Because Relora is a marketplace, your return rights may depend on the reason for the return, the seller’s status and the laws applicable to the transaction.',
      'This policy does not limit any mandatory consumer rights that cannot legally be excluded or restricted.',
    ],
  },
  {
    id: 'seller-status',
    title: '2. Private and Professional Sellers',
    content: [
      'Items on Relora may be offered by private sellers or by professional sellers acting as part of their business.',
      'The seller’s status affects the buyer’s return rights. Buyers should review the seller information displayed on the listing before placing a bid.',
    ],
    list: [
      'A private seller sells outside their trade, business or profession',
      'A professional seller sells as part of a commercial or professional activity',
      'Different statutory consumer rights may apply to professional sellers',
      'Purchases from private sellers generally do not include a statutory change-of-mind withdrawal right',
    ],
    note:
      'The seller’s status does not remove Buyer Protection for eligible problems such as an undisclosed defect, counterfeit item or materially inaccurate listing.',
  },
  {
    id: 'buyer-protection',
    title: '3. Returns Under Buyer Protection',
    content: [
      'Buyer Protection applies to eligible transactions completed and paid for entirely through Relora.',
      'After an order is marked as delivered, the buyer has 48 hours to inspect the item and report a qualifying problem through the order page.',
      'A Buyer Protection return is based on a serious problem with the transaction. It is different from returning an item simply because the buyer changed their mind.',
    ],
    list: [
      'The received item is materially different from the listing',
      'The item has significant damage that was not disclosed',
      'The condition is materially worse than described',
      'The buyer received a different item',
      'Important parts or accessories shown in the listing are missing',
      'The item appears to be counterfeit or unauthorised',
      'The parcel arrived significantly damaged during delivery',
    ],
    note:
      'The 48-hour reporting period concerns Relora Buyer Protection. It does not replace or reduce mandatory statutory rights that may apply to a purchase from a professional seller.',
  },
  {
    id: 'withdrawal',
    title: '4. Statutory Right of Withdrawal',
    content: [
      'Where required by applicable consumer law, a consumer purchasing online from a professional seller may have the right to withdraw from the transaction without providing a reason.',
      'The usual withdrawal period is 14 days from the day on which the buyer, or a person chosen by the buyer, receives the item.',
      'To exercise this right, the buyer must clearly inform the professional seller or Relora before the withdrawal period expires.',
      'The right of withdrawal generally does not apply when an item is purchased from a private individual rather than a professional seller.',
    ],
    note:
      'A transaction is not automatically excluded from withdrawal rights merely because the item was sold using an online bidding process.',
  },
  {
    id: 'change-of-mind',
    title: '5. Change-of-Mind Returns',
    content: [
      'Relora does not provide a general voluntary change-of-mind return right for purchases from private sellers.',
      'This means that a buyer normally cannot return an item from a private seller simply because the buyer no longer wants it, selected the wrong size or changed their mind after winning the auction.',
      'A professional seller may offer a separate voluntary return policy. Any additional conditions should be displayed on the listing or the seller’s profile.',
      'Where a statutory withdrawal right applies, it takes priority over any more restrictive marketplace or seller policy.',
    ],
  },
  {
    id: 'not-eligible',
    title: '6. Returns That May Be Rejected',
    content: [
      'A return request may be rejected when it is not supported by Buyer Protection, a seller’s voluntary return policy or an applicable statutory right.',
    ],
    list: [
      'The buyer changed their mind about an item bought from a private seller',
      'The item does not fit, but the size was correctly disclosed',
      'The reported defect was clearly shown or described before bidding',
      'The difference is minor and does not materially affect the item',
      'The item shows normal wear consistent with its listed condition',
      'The buyer damaged, washed, repaired or altered the item',
      'The buyer sent the item back without return instructions',
      'The buyer cannot provide evidence of shipment or delivery',
      'The transaction or payment was completed outside Relora',
      'The applicable reporting or return period has expired',
    ],
  },
  {
    id: 'request',
    title: '7. How to Request a Return',
    content: [
      'Open the relevant order in your Relora account and select “Report a problem”. If that option is unavailable, contact Relora support and provide the order details.',
      'Choose the reason for the request, describe the issue clearly and upload evidence where required.',
      'The buyer must submit the request within the period applicable to the specific type of return.',
    ],
    list: [
      'Clear photographs of the complete item',
      'Close-up photographs of the reported defect',
      'Photographs of labels, serial numbers and authenticity details',
      'Photographs of the parcel and shipping label',
      'A delivery or unboxing video, when available',
      'Any communication or documentation relevant to the request',
    ],
  },
  {
    id: 'approval',
    title: '8. Return Approval',
    content: [
      'Submitting a return request does not automatically mean that the return has been approved.',
      'Relora may review the listing, photographs, item description, tracking information, payment records and evidence provided by both parties.',
      'The buyer and seller may be required to provide additional information within a specified period.',
      'After reviewing the request, Relora may approve the return, reject the request, request further evidence or propose another appropriate resolution.',
    ],
    note:
      'Do not return an item before receiving confirmation and return instructions. An unauthorised return may be sent to the wrong address and may affect the refund process.',
  },
  {
    id: 'shipping',
    title: '9. Returning the Item',
    content: [
      'When a return is approved, the buyer must send the item to the address provided in the return instructions.',
      'The item must be packed securely and returned using a delivery method that provides valid end-to-end tracking.',
      'The buyer should keep the shipping receipt, tracking number and packaging evidence until the return and refund have been completed.',
    ],
    list: [
      'Return the correct item',
      'Include all accessories and components received',
      'Use appropriate protective packaging',
      'Remove or cover unrelated shipping labels',
      'Use the return address provided for the case',
      'Submit the tracking number through Relora',
      'Send the item before the stated return deadline',
    ],
  },
  {
    id: 'condition',
    title: '10. Condition of Returned Items',
    content: [
      'The returned item must normally be in the same condition in which it was received.',
      'The buyer may inspect an item only to the extent reasonably necessary to establish its nature, characteristics and condition.',
      'The buyer may be responsible for a reduction in value caused by handling beyond what was reasonably necessary to inspect the item.',
    ],
    list: [
      'The item must not be worn beyond reasonable inspection',
      'The item must not be washed or professionally cleaned',
      'Tags and security seals must not be removed unnecessarily',
      'The item must not be repaired, modified or altered',
      'All included packaging and accessories should be returned',
    ],
  },
  {
    id: 'costs',
    title: '11. Return Shipping Costs',
    content: [
      'Responsibility for return shipping costs depends on the reason for the return, the seller’s status and applicable law.',
      'For an approved Buyer Protection claim caused by an inaccurate listing, counterfeit item, undisclosed defect or incorrect item, Relora may require the seller to cover reasonable return shipping costs.',
      'For a statutory change-of-mind withdrawal, the buyer may be responsible for the direct return shipping cost where the professional seller properly informed the buyer of this responsibility before the purchase.',
      'Relora will communicate the applicable return shipping arrangement before the item is sent.',
    ],
  },
  {
    id: 'refunds',
    title: '12. Refunds',
    content: [
      'A refund may be processed after the returned item has been delivered and the return conditions have been verified.',
      'Where legally permitted, the refund may be withheld until the item has been received or the buyer provides sufficient evidence that it was sent.',
      'An approved refund may include the item price and other amounts required by applicable law or the outcome of the Buyer Protection review.',
      'Additional or upgraded delivery costs selected by the buyer may not always be refundable.',
      'After Relora or the payment provider processes the refund, the time required for it to appear depends on the buyer’s bank or payment method.',
    ],
  },
  {
    id: 'failed-return',
    title: '13. Failed or Incomplete Returns',
    content: [
      'A refund may be refused, reduced or reversed if the buyer does not follow the approved return instructions.',
    ],
    list: [
      'The buyer returns a different item',
      'The returned parcel is empty or missing important components',
      'The item was damaged after delivery',
      'The buyer used an untracked shipping method',
      'The buyer sent the parcel after the return deadline',
      'The buyer provided false or misleading information',
      'The parcel was sent to an address not provided in the instructions',
    ],
  },
  {
    id: 'lost-return',
    title: '14. Lost or Damaged Return Parcels',
    content: [
      'The party responsible for purchasing the return shipping service may also be responsible for filing a claim with the delivery provider if the return parcel is lost or damaged in transit.',
      'Relora may request the shipping receipt, tracking history, photographs of the packaging or other evidence before resolving the case.',
      'A refund may be delayed while the delivery provider investigates a missing or damaged return.',
    ],
  },
  {
    id: 'off-platform',
    title: '15. Off-Platform Returns',
    content: [
      'Buyers and sellers should not arrange returns, replacement payments or refunds outside Relora.',
      'Relora may be unable to verify or protect a return that was arranged privately without being recorded in the order.',
      'Never send money directly to another user or follow an external payment link as part of a return.',
    ],
  },
  {
    id: 'abuse',
    title: '16. Return Abuse',
    content: [
      'Relora may restrict or suspend accounts involved in fraudulent, misleading or abusive return activity.',
    ],
    list: [
      'Returning a different item',
      'Falsely claiming that an item was damaged or counterfeit',
      'Intentionally damaging an item to obtain a refund',
      'Manipulating photographs or other evidence',
      'Repeatedly making unsupported claims',
      'Using returns to avoid valid auction obligations',
    ],
  },
  {
    id: 'contact',
    title: '17. Contact',
    content: [
      'For a problem with an active order, open the order and select “Report a problem”.',
      'For general questions about returns, refunds or withdrawal rights, contact Relora through the Contact us page.',
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
              Buying on Relora
            </p>

            <h1
              class="mt-4 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl xl:text-6xl"
            >
              Returns Policy
            </h1>

            <p class="mt-3 text-sm text-foreground/45">
              Last updated: July 24, 2026
            </p>

            <p
              class="mt-5 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base"
            >
              Learn when an item may be returned, how to submit a return
              request and what happens after a return has been approved.
            </p>
          </div>

          <div class="flex items-end lg:justify-end">
            <div class="w-full rounded-[28px] bg-background p-6">
              <p
                class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
              >
                Return summary
              </p>

              <p class="mt-3 text-sm leading-7 text-foreground/65">
                Returns depend on the reason for the request and whether the
                seller is private or professional. Never return an item before
                receiving return instructions.
              </p>

              <div class="mt-5 space-y-3">
                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Report order problems within 48 hours
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Wait for approval before shipping
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Always use tracked return delivery
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Return process -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="mb-8">
        <p
          class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
        >
          Return process
        </p>

        <h2
          class="mt-3 text-3xl font-semibold tracking-tight text-foreground"
        >
          How to return an item
        </h2>
      </div>

      <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <article
          v-for="step in returnSteps"
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
          Relora · Returns Policy
        </p>

        <p class="mt-3 text-lg font-medium text-foreground/65">
          Clear instructions make every return safer.
        </p>
      </div>
    </section>
  </div>
</template>