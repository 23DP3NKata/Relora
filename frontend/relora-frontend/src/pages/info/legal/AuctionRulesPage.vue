<script setup lang="ts">
import { RouterLink } from 'vue-router'

type AuctionRuleSection = {
  id: string
  title: string
  content?: string[]
  list?: string[]
  note?: string
}

const auctionSteps = [
  {
    number: '01',
    title: 'Review the listing',
    description:
      'Check the photographs, description, condition, size and disclosed defects before placing a bid.',
  },
  {
    number: '02',
    title: 'Place your bid',
    description:
      'Enter an amount that meets the minimum bid requirements shown on the auction page.',
  },
  {
    number: '03',
    title: 'Follow the auction',
    description:
      'Other buyers may place higher bids until the displayed auction closing time.',
  },
  {
    number: '04',
    title: 'Complete the purchase',
    description:
      'If you are the highest bidder when the auction ends, you must complete payment within the stated deadline.',
  },
]

const sections: AuctionRuleSection[] = [
  {
    id: 'overview',
    title: '1. Overview',
    content: [
      'These Auction Rules explain how auctions, bids and winning purchases operate on Relora.',
      'By listing an item, starting an auction or placing a bid, you agree to follow these rules together with the Relora Terms of Service and other applicable policies.',
      'Relora provides the marketplace and auction functionality but is not normally the owner or direct seller of items listed by users.',
    ],
  },
  {
    id: 'auction-format',
    title: '2. Auction Format',
    content: [
      'Items on Relora may be offered through time-limited online auctions.',
      'During an active auction, eligible users may place bids until the displayed closing time.',
      'Unless the auction page clearly states otherwise, the highest valid bid recorded by Relora when the auction ends becomes the winning bid.',
    ],
    note:
      'The information displayed by Relora’s auction system, including bid amounts and closing time, is treated as the official auction record.',
  },
  {
    id: 'eligibility',
    title: '3. Who Can Participate',
    content: [
      'You must have an eligible Relora account to create an auction or place a bid.',
      'Relora may require account, identity, contact or payment verification before allowing certain auction activity.',
    ],
    list: [
      'Your account information must be accurate and current',
      'You must have the legal capacity to enter into the transaction',
      'Your account must not be suspended or restricted',
      'You must comply with any age requirements applicable to the platform',
      'You must use your own account and payment method',
      'You must not participate on behalf of another person without permission',
    ],
  },
  {
    id: 'listing-approval',
    title: '4. Listing Approval',
    content: [
      'A seller must create an item listing before starting an auction.',
      'Listings may be reviewed by Relora before they are approved for auction.',
      'Relora may reject a listing or request additional photographs, item information, measurements or authenticity evidence.',
      'Approval does not remove the seller’s responsibility for the legality, authenticity, accuracy and condition of the item.',
    ],
  },
  {
    id: 'auction-duration',
    title: '5. Auction Duration',
    content: [
      'After a listing has been approved, the seller may select one of the auction durations currently available on Relora.',
      'Available durations may include 3, 7 or 14 days.',
      'The auction begins when it is successfully started and remains active until the closing time displayed on the auction page.',
    ],
    list: [
      '3-day auction for a faster sale',
      '7-day auction for a standard listing period',
      '14-day auction for longer marketplace exposure',
    ],
    note:
      'The seller should confirm all listing details before starting the auction. Important details may become restricted once bidding begins.',
  },
  {
    id: 'starting-price',
    title: '6. Starting Price',
    content: [
      'The seller chooses the starting price before the auction begins.',
      'The starting price should be an amount the seller is genuinely prepared to accept if the item receives only one valid bid.',
      'The seller should consider Relora fees and other applicable costs before confirming the auction.',
    ],
  },
  {
    id: 'placing-bids',
    title: '7. Placing a Bid',
    content: [
      'To place a bid, enter or select an amount that satisfies the minimum bid requirement displayed on the auction page.',
      'A bid is accepted only after it has been successfully recorded by the Relora system.',
      'Refreshing the page, losing an internet connection or closing the browser does not cancel a bid that has already been accepted.',
    ],
    list: [
      'Review the listing before bidding',
      'Check the current highest bid',
      'Confirm the minimum amount required',
      'Make sure you are willing and able to pay',
      'Submit the bid before the auction closes',
      'Check that the bid was successfully recorded',
    ],
  },
  {
    id: 'binding-bids',
    title: '8. Bids Are Binding',
    content: [
      'A bid represents a serious and binding intention to purchase the item.',
      'By placing a bid, you agree to complete payment if your bid is the highest valid bid when the auction ends.',
      'You should not place a bid simply to test the platform, reserve the item, increase the price or interfere with another buyer.',
    ],
    note:
      'Do not bid unless you have reviewed the item and are prepared to complete the purchase.',
  },
  {
    id: 'bid-cancellation',
    title: '9. Cancelling or Withdrawing a Bid',
    content: [
      'Buyers cannot normally withdraw or reduce a valid bid after it has been placed.',
      'Changing your mind, selecting the wrong size or finding another item is not normally a valid reason to cancel a bid.',
      'Relora may consider correcting a bid only in exceptional circumstances, such as a clear technical error, account compromise or an obvious input mistake.',
      'Any request must be submitted immediately and before the auction ends. Relora does not guarantee that a bid can be removed.',
    ],
  },
  {
    id: 'valid-bids',
    title: '10. Valid and Invalid Bids',
    content: [
      'Relora may reject, remove or disregard bids that violate platform rules or cannot be verified.',
    ],
    list: [
      'Bids placed through an unauthorised or compromised account',
      'Bids placed by the seller on their own item',
      'Bids placed to manipulate the auction price',
      'Bids coordinated between connected users',
      'Bids generated through prohibited automated tools',
      'Bids placed without a genuine intention to purchase',
      'Bids connected to fraud, payment abuse or policy violations',
      'Bids affected by a confirmed platform error',
    ],
  },
  {
    id: 'bid-manipulation',
    title: '11. Bid Manipulation',
    content: [
      'Any attempt to artificially influence the price, outcome or visibility of an auction is prohibited.',
      'This includes shill bidding, where the seller or another person connected to the seller places bids to increase the price.',
    ],
    list: [
      'Bidding on your own item',
      'Asking friends or relatives to increase the price',
      'Using multiple accounts to influence the auction',
      'Coordinating bids with another user',
      'Threatening or pressuring another bidder',
      'Offering compensation to stop someone from bidding',
      'Creating false demand or misleading auction activity',
    ],
    note:
      'Accounts involved in bid manipulation may be restricted or permanently suspended.',
  },
  {
    id: 'listing-changes',
    title: '12. Changes During an Auction',
    content: [
      'A seller should not make material changes to an item after the auction has started.',
      'Changes that could affect a buyer’s decision, including changes to the condition, size, authenticity information, included accessories or item availability, must be reported to Relora.',
      'Relora may update, pause or cancel an auction when important information is inaccurate or incomplete.',
    ],
  },
  {
    id: 'seller-cancellation',
    title: '13. Seller Auction Cancellation',
    content: [
      'A seller cannot normally cancel an auction simply because the current price is lower than expected.',
      'Once valid bids have been placed, the seller is expected to complete the sale if the auction ends successfully.',
      'An auction may be cancelled when there is a legitimate problem that prevents the transaction from continuing.',
    ],
    list: [
      'The item was lost, stolen or seriously damaged',
      'The listing contains a material error',
      'There is a credible authenticity concern',
      'The seller no longer has the legal right to sell the item',
      'The listing violates Relora rules or applicable law',
      'Relora identifies fraud, manipulation or a technical problem',
    ],
    note:
      'Repeatedly cancelling auctions after receiving bids may result in account restrictions.',
  },
  {
    id: 'auction-end',
    title: '14. When an Auction Ends',
    content: [
      'The auction ends at the closing time displayed on the auction page, subject to any clearly displayed platform rules or technical adjustments.',
      'If at least one valid bid has been accepted, the highest valid bidder becomes the winning buyer.',
      'Relora may automatically create an order connecting the seller and the winning buyer.',
      'The final auction price becomes the item price used for the order, together with any applicable fees, delivery costs or taxes shown during checkout.',
    ],
  },
  {
    id: 'same-bid',
    title: '15. Equal or Competing Bids',
    content: [
      'When multiple users attempt to place the same bid amount, the bid recorded first by the Relora system normally takes priority.',
      'A bid shown as pending, failed or rejected is not treated as a valid bid.',
      'Users should not rely only on notifications and should review the auction page to confirm their current bid status.',
    ],
  },
  {
    id: 'notifications',
    title: '16. Auction Notifications',
    content: [
      'Relora may send notifications about bids, outbid activity, auction status, payment deadlines and order updates.',
      'Notifications are provided for convenience and may be delayed, blocked or affected by device and email settings.',
      'The absence of a notification does not change a valid bid or extend an auction deadline.',
    ],
    note:
      'Bidders remain responsible for checking the auction page and their Relora account.',
  },
  {
    id: 'winner-payment',
    title: '17. Winning Bid and Payment',
    content: [
      'The winning buyer must complete payment through Relora within the deadline displayed on the order.',
      'The total payment may include the winning bid, delivery costs, buyer fees, taxes or other applicable amounts shown before payment is confirmed.',
      'The buyer should not send payment directly to the seller or use an external payment link.',
    ],
    list: [
      'Open the order created after the auction',
      'Review the item price and additional charges',
      'Confirm the delivery information',
      'Complete payment through Relora checkout',
      'Wait for payment confirmation',
    ],
  },
  {
    id: 'failed-payment',
    title: '18. Failure to Pay',
    content: [
      'If the winning buyer does not complete payment within the required period, the order may be cancelled.',
      'Relora may restrict accounts that repeatedly win auctions without completing payment.',
      'Depending on the available platform functionality, Relora may allow the seller to relist the item or take another appropriate action.',
    ],
    list: [
      'Cancellation of the unpaid order',
      'Temporary bidding restrictions',
      'Account warnings',
      'Suspension for repeated non-payment',
      'Relisting of the item by the seller',
    ],
  },
  {
    id: 'seller-obligations',
    title: '19. Seller Obligations After Payment',
    content: [
      'The seller must not ship the item until Relora confirms that payment has been successfully completed.',
      'After payment confirmation, the seller must send the correct item to the delivery address shown on the order.',
    ],
    list: [
      'Ship the item within the stated deadline',
      'Use secure and appropriate packaging',
      'Use a trackable delivery method',
      'Upload the correct tracking number',
      'Ship only to the official order address',
      'Keep shipping and packaging evidence',
      'Respond to order-related support requests',
    ],
  },
  {
    id: 'buyer-protection',
    title: '20. Buyer Protection After the Auction',
    content: [
      'A completed auction does not remove Buyer Protection or mandatory consumer rights that may apply to the transaction.',
      'The buyer may report a qualifying problem when the item is not delivered, appears counterfeit, contains significant undisclosed damage or is materially different from the listing.',
      'Change-of-mind returns, size issues and other return requests are handled according to the Relora Returns Policy and the seller’s legal status.',
    ],
  },
  {
    id: 'technical-issues',
    title: '21. Technical Problems',
    content: [
      'Online auctions may occasionally be affected by service interruptions, payment outages, incorrect timestamps, duplicate requests or other technical problems.',
      'Relora may investigate auction records and take reasonable corrective action when a confirmed technical problem affected the fairness or validity of an auction.',
    ],
    list: [
      'Correct an incorrectly recorded bid',
      'Remove an invalid bid',
      'Pause or cancel an affected auction',
      'Restore an auction where technically possible',
      'Adjust the closing time when clearly necessary',
      'Cancel an order created from an invalid auction result',
    ],
    note:
      'Personal internet problems or a failure to submit a bid before closing do not normally require Relora to reopen an auction.',
  },
  {
    id: 'off-platform',
    title: '22. Off-Platform Transactions',
    content: [
      'Buyers and sellers must not use an auction to arrange a private transaction outside Relora.',
      'Moving payment or delivery arrangements outside the platform may remove Buyer Protection and Seller Protection.',
    ],
    list: [
      'Do not request direct bank transfers',
      'Do not send external payment links',
      'Do not ask users to cancel an auction and buy privately',
      'Do not move transaction details to another marketplace',
      'Do not use listings only to advertise external sales',
    ],
  },
  {
    id: 'enforcement',
    title: '23. Rule Enforcement',
    content: [
      'Relora may investigate auction activity and take action when these rules or other platform policies are violated.',
      'The action taken may depend on the seriousness, frequency and impact of the violation.',
    ],
    list: [
      'Removing a bid',
      'Cancelling an auction or order',
      'Removing a listing',
      'Restricting bidding or selling functionality',
      'Withholding or delaying a payout during review',
      'Requesting identity or payment verification',
      'Temporarily suspending an account',
      'Permanently terminating an account',
      'Reporting unlawful activity where required',
    ],
  },
  {
    id: 'changes',
    title: '24. Changes to Auction Rules',
    content: [
      'Relora may update these Auction Rules when auction functionality, payment processes, marketplace policies or legal requirements change.',
      'The latest version will be published on this page together with an updated revision date.',
      'Changes should not normally alter the outcome of an auction that was already completed before the new rules took effect.',
    ],
  },
  {
    id: 'support',
    title: '25. Auction Support',
    content: [
      'For a problem with an active auction, bid or winning order, open the relevant auction or order and use the available support option.',
      'For general questions about bidding, payments, auction results or seller obligations, contact Relora through the Contact us page.',
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
              Buying and selling
            </p>

            <h1
              class="mt-4 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl xl:text-6xl"
            >
              Auction Rules
            </h1>

            <p class="mt-3 text-sm text-foreground/45">
              Last updated: July 24, 2026
            </p>

            <p
              class="mt-5 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base"
            >
              Learn how Relora auctions operate, what happens when you place a
              bid and which responsibilities apply to winning buyers and
              sellers.
            </p>

            <div class="mt-7 flex flex-wrap gap-3">
              <RouterLink
                to="/auctions"
                class="inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-6 text-sm font-medium text-background transition hover:opacity-80"
              >
                Browse auctions
              </RouterLink>

              <RouterLink
                to="/sell"
                class="inline-flex min-h-11 items-center justify-center rounded-full border px-6 text-sm font-medium text-foreground transition hover:bg-neutral-200/60 dark:hover:bg-neutral-700/60"
              >
                Sell an item
              </RouterLink>
            </div>
          </div>

          <div class="flex items-end lg:justify-end">
            <div class="w-full rounded-[28px] bg-background p-6">
              <p
                class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
              >
                Auction summary
              </p>

              <p class="mt-3 text-sm leading-7 text-foreground/65">
                Review the listing carefully before bidding. Every valid bid is
                binding, and the highest bidder must complete payment when the
                auction ends.
              </p>

              <div class="mt-5 space-y-3">
                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Bids represent an obligation to buy
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Bid manipulation is prohibited
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Winning buyers must pay on time
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Auction process -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="mb-8">
        <p
          class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
        >
          Auction process
        </p>

        <h2
          class="mt-3 text-3xl font-semibold tracking-tight text-foreground"
        >
          From first bid to completed order
        </h2>
      </div>

      <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <article
          v-for="step in auctionSteps"
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
          Relora · Auction Rules
        </p>

        <p class="mt-3 text-xl font-medium text-foreground/70">
          Review carefully. Bid responsibly. Complete every winning purchase.
        </p>

        <RouterLink
          to="/auctions"
          class="mt-6 inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-7 text-sm font-medium text-background transition hover:opacity-80"
        >
          Browse auctions
        </RouterLink>
      </div>
    </section>
  </div>
</template>