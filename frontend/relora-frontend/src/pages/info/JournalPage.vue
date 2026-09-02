<script setup lang="ts">
import { computed, ref } from 'vue'
import { RouterLink } from 'vue-router'

type JournalCategory =
  | 'All'
  | 'Archive'
  | 'Designers'
  | 'Guides'
  | 'Collecting'
  | 'Care'
  | 'Culture'

type JournalArticle = {
  slug: string
  title: string
  excerpt: string
  category: Exclude<JournalCategory, 'All'>
  imageUrl: string
  publishedAt: string
  readTime: string
  featured?: boolean
}

const categories: JournalCategory[] = [
  'All',
  'Archive',
  'Designers',
  'Guides',
  'Collecting',
  'Care',
  'Culture',
]

const articles: JournalArticle[] = [
  {
    slug: 'why-archive-fashion-still-matters',
    title: 'Why archive fashion still matters',
    excerpt:
      'A closer look at the design, history and cultural relevance that allow older collections to remain important today.',
    category: 'Archive',
    imageUrl:
      'https://images.unsplash.com/photo-1529139574466-a303027c1d8b?q=80&w=1800&auto=format&fit=crop',
    publishedAt: 'July 24, 2026',
    readTime: '6 min read',
    featured: true,
  },
  {
    slug: 'how-to-photograph-designer-clothing',
    title: 'How to photograph designer clothing for an auction',
    excerpt:
      'A practical guide to lighting, labels, details and honest condition photography.',
    category: 'Guides',
    imageUrl:
      'https://images.unsplash.com/photo-1551488831-00ddcb6c6bd3?q=80&w=1600&auto=format&fit=crop',
    publishedAt: 'July 21, 2026',
    readTime: '5 min read',
  },
  {
    slug: 'understanding-condition',
    title: 'Understanding condition in pre-owned fashion',
    excerpt:
      'What new, like new, good and fair should mean when an item is presented for resale.',
    category: 'Guides',
    imageUrl:
      'https://images.unsplash.com/photo-1591047139829-d91aecb6caea?q=80&w=1600&auto=format&fit=crop',
    publishedAt: 'July 18, 2026',
    readTime: '4 min read',
  },
  {
    slug: 'building-an-archive-wardrobe',
    title: 'Building an archive wardrobe without buying everything',
    excerpt:
      'How to select fewer pieces with stronger identity, history and long-term relevance.',
    category: 'Collecting',
    imageUrl:
      'https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?q=80&w=1600&auto=format&fit=crop',
    publishedAt: 'July 14, 2026',
    readTime: '7 min read',
  },
  {
    slug: 'how-to-care-for-designer-knitwear',
    title: 'How to care for designer knitwear',
    excerpt:
      'Storage, cleaning and repair practices that can help delicate garments remain wearable for longer.',
    category: 'Care',
    imageUrl:
      'https://images.unsplash.com/photo-1434389677669-e08b4cac3105?q=80&w=1600&auto=format&fit=crop',
    publishedAt: 'July 10, 2026',
    readTime: '5 min read',
  },
  {
    slug: 'why-auctions-change-value-discovery',
    title: 'Why auctions change how value is discovered',
    excerpt:
      'How timing, scarcity and real buyer interest shape the final value of an item.',
    category: 'Culture',
    imageUrl:
      'https://images.unsplash.com/photo-1523398002811-999ca8dec234?q=80&w=1600&auto=format&fit=crop',
    publishedAt: 'July 7, 2026',
    readTime: '6 min read',
  },
  {
    slug: 'details-that-reveal-quality',
    title: 'The details that reveal quality',
    excerpt:
      'Construction, materials, hardware and finishing details worth examining before placing a bid.',
    category: 'Designers',
    imageUrl:
      'https://images.unsplash.com/photo-1496747611176-843222e1e57c?q=80&w=1600&auto=format&fit=crop',
    publishedAt: 'July 3, 2026',
    readTime: '5 min read',
  },
  {
    slug: 'collecting-without-chasing-hype',
    title: 'Collecting without chasing hype',
    excerpt:
      'Why personal taste, research and patience often matter more than temporary market attention.',
    category: 'Collecting',
    imageUrl:
      'https://images.unsplash.com/photo-1509631179647-0177331693ae?q=80&w=1600&auto=format&fit=crop',
    publishedAt: 'June 28, 2026',
    readTime: '8 min read',
  },
  {
    slug: 'repair-or-leave-original',
    title: 'Repair it or leave it original?',
    excerpt:
      'How to think about restoration, wear and authenticity when caring for older fashion pieces.',
    category: 'Care',
    imageUrl:
      'https://images.unsplash.com/photo-1603252109303-2751441dd157?q=80&w=1600&auto=format&fit=crop',
    publishedAt: 'June 22, 2026',
    readTime: '6 min read',
  },
]

const activeCategory = ref<JournalCategory>('All')
const fallbackArticle = articles[0] as JournalArticle

const featuredArticle = computed(() => {
  return articles.find((article) => article.featured) ?? fallbackArticle
})

const filteredArticles = computed(() => {
  return articles.filter((article) => {
    if (article.featured) {
      return false
    }

    if (activeCategory.value === 'All') {
      return true
    }

    return article.category === activeCategory.value
  })
})
</script>

<template>
  <div class="bg-background text-foreground">
    <!-- Hero -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="rounded-[32px] bg-neutral-100 px-6 py-12 dark:bg-neutral-800 sm:px-10 lg:px-12 xl:px-16">
        <div class="max-w-4xl">
          <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
            Relora Journal
          </p>

          <h1
            class="mt-4 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl xl:text-6xl"
          >
            Stories about archive fashion, design, collecting and care.
          </h1>

          <p class="mt-5 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base">
            Explore the ideas, histories and practical knowledge behind pieces
            worth discovering, wearing and preserving.
          </p>
        </div>
      </div>
    </section>

    <!-- Featured article -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <RouterLink
        :to="`/journal/${featuredArticle.slug}`"
        class="group grid overflow-hidden rounded-[32px] border bg-background lg:grid-cols-[1.15fr_0.85fr]"
      >
        <div class="min-h-[380px] overflow-hidden lg:min-h-[620px]">
          <img
            :src="featuredArticle.imageUrl"
            :alt="featuredArticle.title"
            class="h-full w-full object-cover transition duration-700 group-hover:scale-[1.02]"
          />
        </div>

        <div class="flex items-end p-6 sm:p-10 lg:p-12">
          <div>
            <div class="flex flex-wrap items-center gap-3">
              <span
                class="rounded-full bg-neutral-100 px-3 py-1 text-[10px] font-medium uppercase tracking-[0.16em] text-foreground/60 dark:bg-neutral-800"
              >
                Featured
              </span>

              <span class="text-xs text-foreground/45">
                {{ featuredArticle.category }}
              </span>
            </div>

            <h2
              class="mt-6 text-3xl font-semibold tracking-tight transition group-hover:opacity-65 sm:text-4xl"
            >
              {{ featuredArticle.title }}
            </h2>

            <p class="mt-5 text-sm leading-7 text-foreground/65 sm:text-base">
              {{ featuredArticle.excerpt }}
            </p>

            <div class="mt-8 flex items-center gap-3 text-xs text-foreground/45">
              <span>{{ featuredArticle.publishedAt }}</span>
              <span>·</span>
              <span>{{ featuredArticle.readTime }}</span>
            </div>

            <span
              class="mt-8 inline-flex items-center text-sm font-medium text-foreground"
            >
              Read story
              <span class="ml-2 transition group-hover:translate-x-1">→</span>
            </span>
          </div>
        </div>
      </RouterLink>
    </section>

    <!-- Categories -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="flex flex-col gap-6 border-b pb-8 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p class="text-[11px] uppercase tracking-[0.22em] text-foreground/60">
            Latest stories
          </p>

          <h2 class="mt-3 text-3xl font-semibold tracking-tight">
            Explore the Journal
          </h2>
        </div>

        <div class="flex max-w-full gap-2 overflow-x-auto pb-1">
          <button
            v-for="category in categories"
            :key="category"
            type="button"
            class="shrink-0 rounded-full border px-4 py-2 text-sm transition"
            :class="
              activeCategory === category
                ? 'border-foreground bg-foreground text-background'
                : 'border-foreground/15 text-foreground/60 hover:border-foreground/40 hover:text-foreground'
            "
            @click="activeCategory = category"
          >
            {{ category }}
          </button>
        </div>
      </div>
    </section>

    <!-- Article grid -->
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div
        v-if="filteredArticles.length"
        class="grid gap-x-5 gap-y-10 sm:grid-cols-2 lg:grid-cols-3"
      >
        <RouterLink
          v-for="article in filteredArticles"
          :key="article.slug"
          :to="`/journal/${article.slug}`"
          class="group"
        >
          <article>
            <div class="aspect-[4/5] overflow-hidden rounded-[28px] bg-neutral-100 dark:bg-neutral-800">
              <img
                :src="article.imageUrl"
                :alt="article.title"
                class="h-full w-full object-cover transition duration-700 group-hover:scale-[1.025]"
              />
            </div>

            <div class="pt-5">
              <div class="flex items-center gap-3 text-xs text-foreground/45">
                <span>{{ article.category }}</span>
                <span>·</span>
                <span>{{ article.readTime }}</span>
              </div>

              <h3
                class="mt-3 text-xl font-semibold tracking-tight transition group-hover:opacity-65 sm:text-2xl"
              >
                {{ article.title }}
              </h3>

              <p class="mt-3 line-clamp-3 text-sm leading-7 text-foreground/60">
                {{ article.excerpt }}
              </p>

              <p class="mt-5 text-xs text-foreground/40">
                {{ article.publishedAt }}
              </p>
            </div>
          </article>
        </RouterLink>
      </div>

      <div
        v-else
        class="rounded-[28px] border px-6 py-16 text-center"
      >
        <p class="text-sm text-foreground/55">
          There are no stories in this category yet.
        </p>
      </div>
    </section>

    <!-- Editorial statement -->
    <section class="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
      <div
        class="grid overflow-hidden rounded-[32px] bg-neutral-100 dark:bg-neutral-800 lg:grid-cols-2"
      >
        <div class="flex items-center px-6 py-12 sm:px-10 lg:px-12">
          <div class="max-w-xl">
            <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
              Editorial perspective
            </p>

            <h2 class="mt-4 text-3xl font-semibold tracking-tight sm:text-4xl">
              More than products and prices.
            </h2>

            <p class="mt-5 text-sm leading-7 text-foreground/65 sm:text-base">
              Fashion carries design, history, technique and personal meaning.
              The Relora Journal looks beyond the transaction to understand
              why certain pieces remain relevant and how they can be owned more
              thoughtfully.
            </p>

            <RouterLink
              to="/about"
              class="mt-7 inline-flex min-h-11 items-center justify-center rounded-full border px-6 text-sm font-medium transition hover:bg-background"
            >
              About Relora
            </RouterLink>
          </div>
        </div>

        <div class="min-h-[400px]">
          <img
            src="https://images.unsplash.com/photo-1490481651871-ab68de25d43d?q=80&w=1800&auto=format&fit=crop"
            alt="Fashion editorial"
            class="h-full w-full object-cover"
          />
        </div>
      </div>
    </section>

    <!-- Journal topics -->
    <section class="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
      <div class="grid gap-8 lg:grid-cols-[0.9fr_1.1fr]">
        <div>
          <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
            What we cover
          </p>

          <h2 class="mt-4 text-3xl font-semibold tracking-tight sm:text-4xl">
            Knowledge for buyers, sellers and collectors.
          </h2>
        </div>

        <div class="grid gap-4 sm:grid-cols-2">
          <article class="rounded-[28px] border p-6">
            <h3 class="text-lg font-semibold">
              Archive and design
            </h3>
            <p class="mt-3 text-sm leading-7 text-foreground/60">
              Collections, designers, construction, references and cultural
              context.
            </p>
          </article>

          <article class="rounded-[28px] border p-6">
            <h3 class="text-lg font-semibold">
              Buying and collecting
            </h3>
            <p class="mt-3 text-sm leading-7 text-foreground/60">
              Research, condition, measurements, value and thoughtful
              selection.
            </p>
          </article>

          <article class="rounded-[28px] border p-6">
            <h3 class="text-lg font-semibold">
              Selling guides
            </h3>
            <p class="mt-3 text-sm leading-7 text-foreground/60">
              Photography, descriptions, pricing, packaging and successful
              auctions.
            </p>
          </article>

          <article class="rounded-[28px] border p-6">
            <h3 class="text-lg font-semibold">
              Care and preservation
            </h3>
            <p class="mt-3 text-sm leading-7 text-foreground/60">
              Cleaning, repair, storage and long-term ownership.
            </p>
          </article>
        </div>
      </div>
    </section>

    <!-- Bottom CTA -->
    <section class="mx-auto max-w-7xl px-4 pb-16 sm:px-6 lg:px-8">
      <div class="rounded-[32px] border bg-background px-6 py-10 text-center sm:px-10">
        <p class="text-sm text-foreground/45">
          Relora · Journal
        </p>

        <p class="mt-3 text-xl font-medium text-foreground/70">
          Discover the stories behind pieces worth noticing.
        </p>

        <RouterLink
          to="/auctions"
          class="mt-6 inline-flex min-h-11 items-center justify-center rounded-full bg-foreground px-7 text-sm font-medium text-background transition hover:opacity-80"
        >
          Explore auctions
        </RouterLink>
      </div>
    </section>
  </div>
</template>
