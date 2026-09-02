<script setup lang="ts">
import { RouterLink } from 'vue-router'

type SustainabilityPrinciple = {
  number: string
  title: string
  description: string
}

type SustainabilitySection = {
  id: string
  eyebrow: string
  title: string
  content: string[]
  list?: string[]
  imageUrl?: string
  imageAlt?: string
  reverse?: boolean
}

const principles: SustainabilityPrinciple[] = [
  {
    number: '01',
    title: 'Longer product life',
    description:
      'Helping existing fashion remain useful, visible and appreciated instead of being replaced too quickly.',
  },
  {
    number: '02',
    title: 'Thoughtful selection',
    description:
      'Creating space for pieces chosen for their design, condition, identity and long-term appeal.',
  },
  {
    number: '03',
    title: 'Honest information',
    description:
      'Encouraging sellers to describe condition, wear, repairs and imperfections clearly.',
  },
  {
    number: '04',
    title: 'Better care',
    description:
      'Supporting responsible storage, cleaning, repair and preservation whenever appropriate.',
  },
]

const commitments = [
  'Encourage accurate descriptions of item condition',
  'Require sellers to disclose known defects and alterations',
  'Promote care, repair and responsible preservation',
  'Help existing pieces reach new owners',
  'Avoid unsupported environmental claims',
  'Improve packaging and delivery guidance over time',
]

const sections: SustainabilitySection[] = [
  {
    id: 'longer-life',
    eyebrow: 'Extending product life',
    title: 'The most responsible piece can be the one that already exists.',
    content: [
      'Fashion has an environmental impact at every stage of its life. Resale does not remove that impact, but it can help existing items remain useful and valued for longer.',
      'Relora gives premium, designer and archival pieces another opportunity to be discovered instead of being forgotten, stored indefinitely or discarded before the end of their useful life.',
      'Our aim is not to encourage endless purchasing. It is to create a better environment for choosing pieces with character, quality and lasting relevance.',
    ],
    imageUrl:
      'https://images.unsplash.com/photo-1529139574466-a303027c1d8b?q=80&w=1800&auto=format&fit=crop',
    imageAlt: 'Carefully selected fashion pieces',
  },
  {
    id: 'quality',
    eyebrow: 'Quality over replacement',
    title: 'Pieces with lasting value deserve lasting attention.',
    content: [
      'Relora focuses on items that stand out through design, construction, rarity, history or cultural relevance.',
      'A well-made piece can remain meaningful beyond its original season. When buyers understand what they are purchasing and sellers present items honestly, resale can support a more deliberate relationship with fashion.',
      'The goal is not to describe every older item as sustainable. The goal is to help worthwhile pieces stay in circulation when they still have value and use.',
    ],
    list: [
      'Designer and archival clothing',
      'Footwear and accessories worth maintaining',
      'Rare pieces with cultural or design relevance',
      'Items that can be repaired, preserved or reused',
    ],
    imageUrl:
      'https://images.unsplash.com/photo-1490481651871-ab68de25d43d?q=80&w=1800&auto=format&fit=crop',
    imageAlt: 'Long-lasting designer fashion',
    reverse: true,
  },
  {
    id: 'condition',
    eyebrow: 'Transparent condition',
    title: 'Responsible resale begins with honest information.',
    content: [
      'Buyers should understand the condition of an item before placing a bid. Sellers are expected to disclose visible wear, repairs, alterations, missing components and other details that may affect the item’s value or use.',
      'Clear photographs and accurate descriptions reduce unnecessary returns, delivery journeys and disputes.',
      'An item does not need to be perfect to deserve another owner. It does need to be represented truthfully.',
    ],
    list: [
      'Photograph the actual item clearly',
      'Show signs of wear and damage',
      'Describe previous repairs or alterations',
      'Provide useful measurements',
      'Select a realistic condition',
    ],
    imageUrl:
      'https://images.unsplash.com/photo-1551488831-00ddcb6c6bd3?q=80&w=1800&auto=format&fit=crop',
    imageAlt: 'Fashion details and material texture',
  },
  {
    id: 'care',
    eyebrow: 'Care and preservation',
    title: 'Good care can protect both function and history.',
    content: [
      'Storage, cleaning and repair decisions can significantly affect how long a piece remains wearable.',
      'Relora encourages owners to follow material-specific care guidance, use qualified repair services where necessary and preserve original details whenever possible.',
      'For rare and archival items, responsible care may also protect historical, cultural and collectible value.',
    ],
    list: [
      'Follow the item’s care instructions',
      'Store garments in suitable conditions',
      'Repair damage before it becomes more serious',
      'Avoid unnecessary alterations',
      'Use professional care for delicate materials',
    ],
    imageUrl:
      'https://images.unsplash.com/photo-1603252109303-2751441dd157?q=80&w=1800&auto=format&fit=crop',
    imageAlt: 'Fashion garment being carefully maintained',
    reverse: true,
  },
]

const goals = [
  {
    title: 'Clearer packaging guidance',
    text: 'Develop practical recommendations that protect items while avoiding unnecessary packaging.',
  },
  {
    title: 'Better care education',
    text: 'Publish useful Journal guides about storage, cleaning, restoration and long-term ownership.',
  },
  {
    title: 'More informed purchases',
    text: 'Help buyers understand materials, condition and measurements before placing a bid.',
  },
]
</script>

<template>
  <div class="bg-background text-foreground">
    <!-- Hero -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="overflow-hidden rounded-[32px] bg-neutral-100 dark:bg-neutral-800">
        <div class="grid lg:grid-cols-[1.05fr_0.95fr]">
          <div class="flex items-center px-6 py-12 sm:px-10 lg:px-12 xl:px-16">
            <div class="max-w-2xl">
              <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
                Sustainability at Relora
              </p>

              <h1
                class="mt-4 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl xl:text-6xl"
              >
                Better pieces deserve longer lives.
              </h1>

              <p class="mt-5 max-w-xl text-sm leading-7 text-foreground/65 sm:text-base">
                Relora supports a more thoughtful approach to fashion by
                helping existing designer, premium and archival pieces remain
                visible, useful and valued.
              </p>

              <div class="mt-8 flex flex-wrap gap-3">
                <RouterLink
                  to="/auctions"
                  class="inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-6 text-sm font-medium text-background transition hover:opacity-80"
                >
                  Explore the marketplace
                </RouterLink>

                <RouterLink
                  to="/journal"
                  class="inline-flex min-h-11 items-center justify-center rounded-full border px-6 text-sm font-medium text-foreground transition hover:bg-background/60"
                >
                  Read the Journal
                </RouterLink>
              </div>
            </div>
          </div>

          <div class="min-h-[380px] lg:min-h-[620px]">
            <img
              src="https://images.unsplash.com/photo-1539109136881-3be0616acf4b?q=80&w=1800&auto=format&fit=crop"
              alt="Timeless fashion pieces"
              class="h-full w-full object-cover"
            />
          </div>
        </div>
      </div>
    </section>

    <!-- Principles -->
    <section class="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
      <div class="max-w-3xl">
        <p class="text-[11px] uppercase tracking-[0.22em] text-foreground/60">
          Our approach
        </p>

        <h2 class="mt-4 text-3xl font-semibold tracking-tight sm:text-4xl">
          Resale is one part of a more thoughtful fashion system.
        </h2>

        <p class="mt-5 text-sm leading-7 text-foreground/65 sm:text-base">
          We do not claim that buying or selling an item has no environmental
          impact. Our role is to help existing pieces stay in use for longer
          and to make their condition, history and value easier to understand.
        </p>
      </div>

      <div class="mt-10 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <article
          v-for="principle in principles"
          :key="principle.number"
          class="rounded-[28px] border bg-background p-6"
        >
          <span class="text-xs font-medium tracking-[0.18em] text-foreground/40">
            {{ principle.number }}
          </span>

          <h3 class="mt-8 text-xl font-semibold tracking-tight">
            {{ principle.title }}
          </h3>

          <p class="mt-3 text-sm leading-7 text-foreground/60">
            {{ principle.description }}
          </p>
        </article>
      </div>
    </section>

    <!-- Editorial sections -->
    <section
      v-for="section in sections"
      :id="section.id"
      :key="section.id"
      class="mx-auto max-w-7xl scroll-mt-36 px-4 py-10 sm:px-6 lg:px-8"
    >
      <div
        class="grid gap-10 lg:grid-cols-2 lg:items-center"
        :class="{ 'lg:[&>*:first-child]:order-2': section.reverse }"
      >
        <div class="overflow-hidden rounded-[32px]">
          <img
            :src="section.imageUrl"
            :alt="section.imageAlt"
            class="h-[420px] w-full object-cover sm:h-[520px]"
          />
        </div>

        <div class="max-w-2xl">
          <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
            {{ section.eyebrow }}
          </p>

          <h2 class="mt-4 text-3xl font-semibold tracking-tight sm:text-4xl">
            {{ section.title }}
          </h2>

          <div
            class="mt-6 space-y-5 text-sm leading-7 text-foreground/65 sm:text-base"
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
            class="mt-6 space-y-3"
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
        </div>
      </div>
    </section>

    <!-- Commitments -->
    <section class="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
      <div class="rounded-[32px] bg-neutral-100 px-6 py-10 dark:bg-neutral-800 sm:px-10 lg:px-12">
        <div class="grid gap-10 lg:grid-cols-[0.9fr_1.1fr]">
          <div>
            <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
              Our commitments
            </p>

            <h2 class="mt-4 text-3xl font-semibold tracking-tight sm:text-4xl">
              Practical progress instead of empty promises.
            </h2>

            <p class="mt-5 text-sm leading-7 text-foreground/65 sm:text-base">
              Relora is still developing. We will communicate carefully about
              sustainability and avoid claims that cannot be properly
              supported.
            </p>
          </div>

          <div class="rounded-[28px] bg-background p-6 sm:p-8">
            <ul class="space-y-4">
              <li
                v-for="commitment in commitments"
                :key="commitment"
                class="flex items-start gap-3 text-sm leading-7 text-foreground/70 sm:text-base"
              >
                <span
                  class="mt-2.5 h-1.5 w-1.5 shrink-0 rounded-full bg-foreground/70"
                />
                <span>{{ commitment }}</span>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </section>

    <!-- Future goals -->
    <section class="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
      <div class="mb-8 max-w-3xl">
        <p class="text-[11px] uppercase tracking-[0.22em] text-foreground/60">
          What we are working toward
        </p>

        <h2 class="mt-4 text-3xl font-semibold tracking-tight sm:text-4xl">
          Building better habits into the marketplace.
        </h2>
      </div>

      <div class="grid gap-4 md:grid-cols-3">
        <article
          v-for="goal in goals"
          :key="goal.title"
          class="rounded-[28px] border bg-background p-6 sm:p-8"
        >
          <h3 class="text-xl font-semibold tracking-tight">
            {{ goal.title }}
          </h3>

          <p class="mt-4 text-sm leading-7 text-foreground/60">
            {{ goal.text }}
          </p>
        </article>
      </div>
    </section>

    <!-- Statement -->
    <section class="mx-auto max-w-5xl px-4 py-16 text-center sm:px-6 lg:px-8">
      <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/40">
        Our perspective
      </p>

      <h2
        class="mx-auto mt-4 max-w-4xl text-3xl font-semibold tracking-tight sm:text-4xl lg:text-5xl"
      >
        Sustainability is not a label attached to every purchase. It is a
        longer relationship with the things we choose to own.
      </h2>

      <p class="mx-auto mt-6 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base">
        Relora aims to make that relationship more informed, transparent and
        intentional.
      </p>
    </section>

    <!-- Bottom CTA -->
    <section class="mx-auto max-w-7xl px-4 pb-16 sm:px-6 lg:px-8">
      <div class="rounded-[32px] border bg-background px-6 py-10 text-center sm:px-10">
        <p class="text-sm text-foreground/45">
          Relora · Sustainability
        </p>

        <p class="mt-3 text-xl font-medium text-foreground/70">
          Discover pieces worth keeping.
        </p>

        <RouterLink
          to="/auctions"
          class="mt-6 inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-7 text-sm font-medium text-background transition hover:opacity-80"
        >
          Browse Relora
        </RouterLink>
      </div>
    </section>
  </div>
</template>