<script setup lang="ts">
import { RouterLink } from 'vue-router'

type PartnershipSection = {
  id: string
  title: string
  content?: string[]
  list?: string[]
  note?: string
}

const partnershipTypes = [
  {
    number: '01',
    title: 'Brands and designers',
    description:
      'Present selected collections, archive pieces and brand stories to a fashion-focused audience.',
  },
  {
    number: '02',
    title: 'Boutiques and retailers',
    description:
      'Give past-season, rare or curated inventory another route to reach interested buyers.',
  },
  {
    number: '03',
    title: 'Creators and media',
    description:
      'Collaborate on editorial stories, educational content, campaigns and community projects.',
  },
  {
    number: '04',
    title: 'Industry services',
    description:
      'Work with Relora across authentication, delivery, preservation, technology and other services.',
  },
]

const partnershipSteps = [
  {
    number: '01',
    title: 'Tell us about your idea',
    description:
      'Share information about your organisation, audience, goals and the type of collaboration you have in mind.',
  },
  {
    number: '02',
    title: 'Review and discussion',
    description:
      'We review whether the proposal fits Relora, its users and the direction of the marketplace.',
  },
  {
    number: '03',
    title: 'Plan the collaboration',
    description:
      'Together, we define the format, responsibilities, timeline and expected outcome of the partnership.',
  },
  {
    number: '04',
    title: 'Launch and evaluate',
    description:
      'After launch, we review performance, feedback and opportunities for future collaboration.',
  },
]

const sections: PartnershipSection[] = [
  {
    id: 'overview',
    title: '1. Partnering with Relora',
    content: [
      'Relora is building an auction-based marketplace for premium, designer and archival fashion.',
      'We welcome partnership proposals from organisations and individuals who can help create a safer, more useful and more interesting marketplace for buyers and sellers.',
      'Every partnership should provide clear value to the Relora community and align with our standards for authenticity, transparency and responsible commerce.',
    ],
  },
  {
    id: 'who-we-work-with',
    title: '2. Who We Work With',
    content: [
      'Potential partnerships may include established businesses, independent professionals and emerging projects connected to fashion, resale, technology and culture.',
    ],
    list: [
      'Designer and independent fashion brands',
      'Boutiques, retailers and curated stores',
      'Vintage and archival fashion sellers',
      'Authentication and verification specialists',
      'Fashion publications and media platforms',
      'Photographers, stylists and content creators',
      'Museums, exhibitions and cultural organisations',
      'Delivery, insurance and logistics providers',
      'Garment care, repair and preservation services',
      'Technology and payment service providers',
    ],
  },
  {
    id: 'brand-collaborations',
    title: '3. Brand Collaborations',
    content: [
      'Relora may collaborate with brands on projects that introduce their work, history and selected inventory to a relevant audience.',
      'A brand partnership does not have to focus only on selling products. It may also support education, storytelling, sustainability or community development.',
    ],
    list: [
      'Curated auctions',
      'Archive and heritage collections',
      'Limited releases',
      'Editorial brand stories',
      'Designer interviews',
      'Collection guides',
      'Resale and circular-fashion initiatives',
      'Community events and campaigns',
    ],
    note:
      'All items offered through a collaboration must meet Relora’s listing, authenticity and condition requirements.',
  },
  {
    id: 'retail-partnerships',
    title: '4. Boutiques and Retail Partners',
    content: [
      'Boutiques and retailers may use Relora to introduce selected inventory to buyers interested in premium and archival fashion.',
      'Suitable inventory may include past-season pieces, rare products, display items, customer returns where permitted, and curated pre-owned collections.',
      'The exact listing and fulfilment process should be agreed before the partnership begins.',
    ],
    list: [
      'Curated store auctions',
      'Past-season inventory',
      'Rare and limited pieces',
      'Store archive items',
      'Authenticated pre-owned collections',
      'Joint promotional campaigns',
    ],
  },
  {
    id: 'creator-partnerships',
    title: '5. Creators and Media',
    content: [
      'Relora welcomes proposals from creators, photographers, stylists, writers and media platforms whose work is relevant to fashion, design, collecting and responsible resale.',
      'Collaborations should be transparent and should not mislead users about sponsorship, payment or editorial independence.',
    ],
    list: [
      'Editorial articles and interviews',
      'Brand and designer guides',
      'Auction previews',
      'Photography and visual campaigns',
      'Educational videos',
      'Social media collaborations',
      'Community events',
      'Fashion history and archive content',
    ],
    note:
      'Paid, gifted or sponsored collaborations should always be disclosed clearly.',
  },
  {
    id: 'authentication',
    title: '6. Authentication and Verification',
    content: [
      'Authenticity is important to the Relora marketplace. We are interested in working with qualified specialists and organisations that can support item verification and seller education.',
      'A verification partnership may involve reviewing selected items, developing internal guidelines or helping users understand common authenticity indicators.',
    ],
    list: [
      'Item authentication services',
      'Category-specific expert reviews',
      'Seller verification support',
      'Educational authenticity content',
      'Brand reference materials',
      'Fraud prevention processes',
      'Moderation and risk assessment support',
    ],
  },
  {
    id: 'sustainability',
    title: '7. Sustainability Partnerships',
    content: [
      'Extending the useful life of fashion can reduce unnecessary consumption and keep valuable items in circulation.',
      'Relora may collaborate with organisations that support repair, reuse, responsible delivery, preservation and measurable circular-fashion initiatives.',
    ],
    list: [
      'Garment repair and restoration',
      'Professional cleaning and care',
      'Reusable or lower-impact packaging',
      'Responsible delivery options',
      'Textile education',
      'Circular-fashion research',
      'Donation and reuse initiatives',
    ],
    note:
      'Environmental claims should be specific, supportable and not presented in a misleading way.',
  },
  {
    id: 'technology',
    title: '8. Technology and Service Partners',
    content: [
      'Relora may work with technology and service providers that improve marketplace safety, payments, delivery, moderation, customer support or user experience.',
      'Potential providers should be able to explain how their service works, how data is handled and how it benefits Relora users.',
    ],
    list: [
      'Payment infrastructure',
      'Identity and business verification',
      'Fraud prevention',
      'Shipping and tracking',
      'Insurance services',
      'Customer support technology',
      'Image and content moderation',
      'Analytics and marketplace tools',
    ],
  },
  {
    id: 'partner-standards',
    title: '9. Partnership Standards',
    content: [
      'Relora reviews partnership proposals based on relevance, trust, quality and the expected benefit for users.',
      'Partners are expected to communicate accurately, protect confidential information and follow applicable laws and Relora policies.',
    ],
    list: [
      'Provide accurate information about the organisation and proposal',
      'Respect intellectual property and brand ownership',
      'Avoid counterfeit, misleading or prohibited products',
      'Disclose commercial relationships clearly',
      'Protect personal and confidential information',
      'Follow agreed deadlines and responsibilities',
      'Communicate honestly with Relora and its users',
      'Comply with applicable consumer and advertising laws',
    ],
  },
  {
    id: 'proposal',
    title: '10. What to Include in Your Proposal',
    content: [
      'A clear proposal helps us understand the opportunity and decide whether the collaboration is suitable for Relora.',
    ],
    list: [
      'Your name, organisation and website',
      'A short description of your business or project',
      'The type of partnership you are proposing',
      'The value the collaboration would provide to Relora users',
      'Your preferred format and timeline',
      'Relevant audience or market information',
      'Examples of previous work or partnerships',
      'Any technical, legal or commercial requirements',
      'The best contact person for further discussion',
    ],
    note:
      'Please do not send confidential information, personal documents or sensitive commercial data in the first message.',
  },
  {
    id: 'review-process',
    title: '11. How Proposals Are Reviewed',
    content: [
      'Submitting a proposal does not guarantee that Relora will accept or proceed with the partnership.',
      'We may review the relevance of the proposal, the reputation of the potential partner, operational requirements and the possible effect on users.',
      'Relora may request additional information before making a decision.',
    ],
    list: [
      'Alignment with Relora’s marketplace and audience',
      'Clear benefit for buyers or sellers',
      'Product and service quality',
      'Authenticity and reputational risk',
      'Technical and operational feasibility',
      'Legal and regulatory requirements',
      'Commercial terms and expected costs',
      'Ability to measure the outcome',
    ],
  },
  {
    id: 'commercial-terms',
    title: '12. Commercial Terms',
    content: [
      'Some partnerships may be unpaid, while others may involve fees, commissions, product access, shared costs or another commercial arrangement.',
      'Any commercial terms, responsibilities and usage rights must be agreed in writing before the collaboration begins.',
      'Relora does not guarantee a minimum level of sales, traffic, exposure or other commercial results unless this is explicitly included in a written agreement.',
    ],
  },
  {
    id: 'content-rights',
    title: '13. Content and Intellectual Property',
    content: [
      'Each party should retain ownership of its existing brands, trademarks, designs and content unless otherwise agreed.',
      'A partnership agreement should explain how names, logos, photographs, videos and other materials may be used.',
      'Partners must have the necessary rights and permissions for any materials they provide to Relora.',
    ],
  },
  {
    id: 'data-privacy',
    title: '14. Data and Privacy',
    content: [
      'Partnerships involving user information, analytics or technical integrations must follow applicable privacy and data-protection requirements.',
      'Access to user information should be limited to what is necessary for the agreed purpose.',
      'Relora does not provide partners with unrestricted access to private user information.',
    ],
  },
  {
    id: 'contact',
    title: '15. Contact Us',
    content: [
      'To discuss a potential brand, retail, media, authentication, sustainability or technology partnership, contact Relora through the Contact us page.',
      'Select “Brand partnership” as the subject and include a clear summary of your proposal.',
      'We review proposals individually and may contact you if the opportunity is relevant to Relora’s current plans.',
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
              Work with Relora
            </p>

            <h1
              class="mt-4 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl xl:text-6xl"
            >
              Brand Partnerships
            </h1>

            <p class="mt-3 text-sm text-foreground/45">
              Last updated: July 24, 2026
            </p>

            <p
              class="mt-5 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base"
            >
              We collaborate with brands, boutiques, creators and industry
              specialists who can help build a stronger marketplace for
              premium and archival fashion.
            </p>

            <div class="mt-7 flex flex-wrap gap-3">
              <RouterLink
                to="/contact?subject=brand-partnership"
                class="inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-6 text-sm font-medium text-background transition hover:opacity-80"
              >
                Submit a proposal
              </RouterLink>

              <RouterLink
                to="/about"
                class="inline-flex min-h-11 items-center justify-center rounded-full border px-6 text-sm font-medium text-foreground transition hover:bg-neutral-100 dark:hover:bg-neutral-800"
              >
                About Relora
              </RouterLink>
            </div>
          </div>

          <div class="flex items-end lg:justify-end">
            <div class="w-full rounded-[28px] bg-background p-6">
              <p
                class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
              >
                Partnership summary
              </p>

              <p class="mt-3 text-sm leading-7 text-foreground/65">
                We are interested in thoughtful collaborations that support
                authenticity, fashion culture, responsible resale and a better
                experience for Relora users.
              </p>

              <div class="mt-5 space-y-3">
                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Curated and relevant collaborations
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Clear value for marketplace users
                  </span>
                </div>

                <div class="flex items-start gap-3">
                  <span
                    class="mt-2 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                  />

                  <span class="text-sm leading-6 text-foreground/60">
                    Transparent commercial relationships
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Partnership types -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="mb-8">
        <p
          class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
        >
          Partnership opportunities
        </p>

        <h2
          class="mt-3 text-3xl font-semibold tracking-tight text-foreground"
        >
          Different ways to work together
        </h2>
      </div>

      <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <article
          v-for="item in partnershipTypes"
          :key="item.number"
          class="rounded-[28px] border bg-background p-6"
        >
          <span
            class="text-xs font-medium tracking-[0.18em] text-foreground/40"
          >
            {{ item.number }}
          </span>

          <h3
            class="mt-8 text-lg font-semibold tracking-tight text-foreground"
          >
            {{ item.title }}
          </h3>

          <p class="mt-3 text-sm leading-7 text-foreground/60">
            {{ item.description }}
          </p>
        </article>
      </div>
    </section>

    <!-- Partnership process -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="mb-8">
        <p
          class="text-[11px] uppercase tracking-[0.22em] text-foreground/60"
        >
          Partnership process
        </p>

        <h2
          class="mt-3 text-3xl font-semibold tracking-tight text-foreground"
        >
          From proposal to launch
        </h2>
      </div>

      <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <article
          v-for="step in partnershipSteps"
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
          Relora · Brand Partnerships
        </p>

        <p class="mt-3 text-xl font-medium text-foreground/70">
          Have an idea that belongs on Relora?
        </p>

        <p class="mx-auto mt-3 max-w-xl text-sm leading-7 text-foreground/55">
          Tell us who you are, what you would like to create and how the
          partnership would benefit the Relora community.
        </p>

        <RouterLink
          to="/contact?subject=brand-partnership"
          class="mt-6 inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-7 text-sm font-medium text-background transition hover:opacity-80"
        >
          Contact Relora
        </RouterLink>
      </div>
    </section>
  </div>
</template>