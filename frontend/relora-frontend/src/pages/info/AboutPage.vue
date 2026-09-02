<script setup lang="ts">
const values = [
  'A curated marketplace instead of overwhelming mass-market inventory',
  'A focus on rare fashion, collectible design, and standout pieces',
  'Real-time bidding shaped by actual demand and market interest',
  'A clean interface that keeps attention on the listing',
]

const highlights = [
  {
    title: 'Curated selection',
    text: 'Pieces are meant to feel chosen, not lost inside endless generic inventory.',
  },
  {
    title: 'Auction energy',
    text: 'Bidding creates movement, timing, and a stronger sense of value discovery.',
  },
  {
    title: 'Clear presentation',
    text: 'A quieter interface gives more room for the item itself to stand out.',
  },
]

const gallery = [
  'https://images.unsplash.com/photo-1523398002811-999ca8dec234?q=80&w=1600&auto=format&fit=crop',
  'https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?q=80&w=1600&auto=format&fit=crop',
  'https://images.unsplash.com/photo-1496747611176-843222e1e57c?q=80&w=1600&auto=format&fit=crop',
]
</script>

<template>
  <div class="bg-background text-foreground">
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="overflow-hidden rounded-[32px] bg-background">
        <div class="grid gap-0 lg:grid-cols-[1.05fr_0.95fr]">
          <div class="flex items-center px-6 py-10 sm:px-10 lg:px-12 xl:px-16">
            <div class="max-w-2xl">
              <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
                About Relora
              </p>

              <h1 class="mt-4 text-4xl font-semibold tracking-tight text-foreground sm:text-5xl xl:text-6xl">
                Rare fashion, refined auctions, and better attention to standout pieces.
              </h1>

              <p class="mt-5 max-w-xl text-sm leading-7 text-foreground/65 sm:text-base">
                Relora is a curated auction marketplace for rare fashion, timeless pieces, and collectible design — built for people who value individuality over mass-market consumption.
              </p>

              <div class="mt-8 flex flex-wrap gap-3">
                <div class="rounded-full bg-black px-4 py-2 text-sm text-white dark:bg-white dark:text-black">
                  Founded in 2026
                </div>
                <div class="border rounded-full bg-black px-4 py-2 text-sm text-white dark:bg-white dark:text-black">
                  Riga, Latvia
                </div>
                <div class="rounded-full bg-black px-4 py-2 text-sm text-white dark:bg-white dark:text-black">
                  Curated marketplace
                </div>
              </div>
            </div>
          </div>

          <div class="min-h-320px lg:min-h-full">
            <img
              :src="gallery[0]"
              alt="Fashion editorial"
              class="h-full w-full object-cover"
            />
          </div>
        </div>
      </div>
    </section>

    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="grid gap-4 md:grid-cols-3">
        <div
          v-for="item in highlights"
          :key="item.title"
          class="rounded-[28px] border bg-background p-6"
        >
          <p class="text-[11px] uppercase tracking-[0.22em] text-foreground/60">
            Highlight
          </p>
          <h2 class="mt-3 text-2xl font-semibold tracking-tight text-foreground">
            {{ item.title }}
          </h2>
          <p class="mt-3 text-sm leading-7 text-foreground/65">
            {{ item.text }}
          </p>
        </div>
      </div>
    </section>

    <section class="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
      <div class="grid gap-10 lg:grid-cols-[0.95fr_1.05fr] lg:items-center">
        <div class="overflow-hidden rounded-[32px] bg-background">
          <img
            :src="gallery[1]"
            alt="Designer clothing"
            class="h-420px w-full object-cover"
          />
        </div>

        <div class="max-w-2xl">
          <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
            What Relora is
          </p>

          <h2 class="mt-4 text-3xl font-semibold tracking-tight text-foreground sm:text-4xl">
            A marketplace built around discovery, timing, and value shaped by real demand.
          </h2>

          <div class="mt-6 space-y-5 text-sm leading-7 text-foreground/65 sm:text-base">
            <p>
              Relora brings together buyers, collectors, enthusiasts, and sellers in a refined online auction environment. It is designed for discovering items that feel special — pieces with character, rarity, and lasting appeal.
            </p>

            <p>
              Instead of endless mass-produced inventory, Relora focuses on items that stand out. From designer clothing and archive fashion to unique lifestyle objects and collectible finds, the goal is to create a marketplace that feels more intentional from the start.
            </p>

            <p>
              The platform combines the excitement of live bidding with the clarity of modern e-commerce. It is not only about buying quickly — it is about discovery, timing, and value revealed through real interest.
            </p>
          </div>
        </div>
      </div>
    </section>

    <section class="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
      <div class="rounded-[32px] bg-background px-6 py-10 sm:px-10 lg:px-12">
        <div class="grid gap-10 lg:grid-cols-[1fr_1fr]">
          <div>
            <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
              Who it is for
            </p>
            <h2 class="mt-4 text-3xl font-semibold tracking-tight text-foreground sm:text-4xl">
              For buyers, collectors, fashion enthusiasts, and sellers who care about the right environment.
            </h2>
          </div>

          <div class="space-y-5 text-sm leading-7 text-foreground/65 sm:text-base">
            <p>
              Relora is built for people who want more than ordinary shopping experiences. It is made for those looking for rare wardrobe pieces, standout accessories, collectible objects, and a marketplace that feels calmer and more deliberate.
            </p>

            <p>
              It is also for sellers who want their items to be seen properly — not buried inside noise, but presented in a cleaner setting where quality and individuality matter more.
            </p>
          </div>
        </div>
      </div>
    </section>

    <section class="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
      <div class="grid gap-10 lg:grid-cols-[1.05fr_0.95fr] lg:items-center">
        <div class="max-w-2xl">
          <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
            What you can discover
          </p>

          <h2 class="mt-4 text-3xl font-semibold tracking-tight text-foreground sm:text-4xl">
            Designer fashion, archive clothing, accessories, footwear, bags, and collectible objects worth noticing.
          </h2>

          <div class="mt-6 space-y-5 text-sm leading-7 text-foreground/65 sm:text-base">
            <p>
              The marketplace is intended to feature categories such as designer fashion, archive clothing, accessories, footwear, bags, lifestyle objects, and other collectible items that deserve attention.
            </p>

            <p>
              The focus stays on pieces that feel curated rather than generic — items chosen for their design, rarity, identity, or cultural relevance.
            </p>
          </div>
        </div>

        <div class="overflow-hidden rounded-[32px] bg-background">
          <img
            :src="gallery[2]"
            alt="Curated fashion piece"
            class="h-420px w-full object-cover"
          />
        </div>
      </div>
    </section>

    <section class="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
      <div class="grid gap-8 lg:grid-cols-[0.9fr_1.1fr]">
        <div>
          <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/60">
            Why auctions matter
          </p>

          <h2 class="mt-4 text-3xl font-semibold tracking-tight text-foreground sm:text-4xl">
            Auctions create a different relationship between buyer, seller, and item.
          </h2>

          <p class="mt-6 text-sm leading-7 text-foreground/65 sm:text-base">
            Instead of simply scrolling and checking out, users participate in a process where value is discovered over time. For sellers, auctions create visibility and the possibility of reaching true market value. For buyers, they create anticipation, competition, and a stronger sense of reward.
          </p>
        </div>

        <div class="rounded-[32px] border bg-background p-6 sm:p-8">
          <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/40">
            What makes Relora different
          </p>

          <ul class="mt-6 space-y-4">
            <li
              v-for="value in values"
              :key="value"
              class="flex items-start gap-3 text-sm leading-7 text-foreground/70 sm:text-base"
            >
              <span class="mt-2 h-1.5 w-1.5 rounded-full bg-foreground/70"></span>
              <span>{{ value }}</span>
            </li>
          </ul>
        </div>
      </div>
    </section>

    <section class="mx-auto max-w-5xl px-4 py-16 text-center sm:px-6 lg:px-8 rounded-2xl mb-16">
      <p class="text-[11px] uppercase tracking-[0.24em] text-foreground/40">
        Our mission
      </p>

      <h2 class="mx-auto mt-4 max-w-4xl text-3xl font-semibold tracking-tight text-foreground sm:text-4xl lg:text-5xl">
        Build a marketplace where rare pieces are discovered, appreciated, and valued properly.
      </h2>

      <p class="mx-auto mt-6 max-w-2xl text-sm leading-7 text-foreground/65 sm:text-base">
        Relora is being shaped as more than a transactional platform. The long-term vision is a destination for curated discovery, trusted sellers, and an audience that values style, scarcity, and thoughtful selection.
      </p>
    </section>

    <section class="mx-auto max-w-7xl px-4 pb-16 sm:px-6 lg:px-8">
      <div class="rounded-[32px] border border-foreground/20 bg-background px-6 py-8 text-center sm:px-10">
        <p class="text-sm text-foreground/45">
          Founded in 2026 · Riga, Latvia
        </p>
        <p class="mt-3 text-lg font-medium text-foreground/70">
          Not mass market. Never ordinary.
        </p>
      </div>
    </section>
  </div>
</template>