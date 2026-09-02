<script setup lang="ts">
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from '@/components/ui/accordion'
</script>

<template>
  <section class="mx-auto max-w-5xl px-4 py-16">
    <div class="mb-12">
      <h1 class="text-4xl font-bold tracking-tight">
        Frequently Asked Questions
      </h1>

      <p class="mt-4 text-lg text-muted-foreground">
        Everything you need to know about buying, selling, auctions,
        shipping, payments, returns, and authenticity on Relora —
        the curated marketplace for designer fashion, archive clothing,
        luxury streetwear, vintage garments, and collectible fashion pieces.
      </p>
    </div>

    <Accordion
      type="single"
      collapsible
      class="w-full space-y-4"
      default-value="item-1"
    >
      <AccordionItem value="item-1">
        <AccordionTrigger>
          What is Relora?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Relora is a curated marketplace dedicated to designer fashion,
            archive clothing, luxury streetwear, vintage fashion, and rare
            collectible garments.
          </p>

          <p class="mt-3">
            The platform allows users to buy and sell designer pieces from
            brands such as Rick Owens, Raf Simons, Balenciaga, Vetements,
            Helmut Lang, Number (N)ine, Undercover, Chrome Hearts,
            Maison Margiela, Yohji Yamamoto, and many more.
          </p>

          <p class="mt-3">
            Unlike traditional marketplaces, Relora focuses on fashion
            culture, curated listings, cleaner browsing, and premium user
            experience.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-2">
        <AccordionTrigger>
          What can I sell on Relora?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Users can sell designer clothing, archive fashion, luxury
            streetwear, vintage garments, sneakers, bags, accessories,
            runway pieces, collectible fashion items, and rare clothing.
          </p>

          <p class="mt-3">
            Popular categories include designer jackets, hoodies, denim,
            footwear, luxury accessories, runway archive pieces, and limited
            edition collections.
          </p>

          <p class="mt-3">
            Counterfeit products, replica items, and misleading listings are
            prohibited.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-3">
        <AccordionTrigger>
          How do I create a listing?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Sellers can create listings by uploading product photos,
            entering item details, selecting category information, and
            setting a price or auction configuration.
          </p>

          <p class="mt-3">
            Accurate descriptions, measurements, condition details, and
            clear product photos help improve buyer trust and increase sales
            potential.
          </p>

          <p class="mt-3">
            Some listings may go through moderation before becoming publicly
            visible on the marketplace.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-4">
        <AccordionTrigger>
          How do auctions work?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Sellers can start timed auctions for eligible fashion items.
            Buyers place bids during the auction period, and the highest bid
            at the end of the auction wins.
          </p>

          <p class="mt-3">
            Auctions are especially useful for rare archive fashion,
            collectible designer garments, and difficult-to-price items.
          </p>

          <p class="mt-3">
            Auction duration and availability may vary depending on seller
            settings and marketplace rules.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-5">
        <AccordionTrigger>
          How does Relora handle authenticity?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Relora aims to reduce counterfeit listings through moderation,
            seller monitoring, detailed product requirements, and manual
            review systems.
          </p>

          <p class="mt-3">
            Buyers should carefully inspect product photos, labels,
            stitching, hardware, tags, and condition details before
            purchasing luxury fashion or designer clothing.
          </p>

          <p class="mt-3">
            Listings suspected of violating authenticity standards may be
            removed, hidden, or manually reviewed.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-6">
        <AccordionTrigger>
          What condition should items be in?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Sellers should accurately describe item condition, including any
            flaws, signs of wear, discoloration, repairs, stains, or
            alterations.
          </p>

          <p class="mt-3">
            Transparency helps buyers make informed purchasing decisions and
            reduces disputes related to item condition.
          </p>

          <p class="mt-3">
            Archive and vintage fashion may naturally contain signs of age,
            which should always be clearly disclosed in the listing.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-7">
        <AccordionTrigger>
          Does Relora support worldwide shipping?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Many sellers on Relora offer international shipping for luxury
            fashion, archive clothing, and designer garments.
          </p>

          <p class="mt-3">
            Shipping costs, delivery times, customs fees, and import taxes
            may vary depending on destination country and shipping provider.
          </p>

          <p class="mt-3">
            Buyers should review shipping details before completing a
            purchase.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-8">
        <AccordionTrigger>
          What payment methods are supported?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Payment methods may vary depending on region, seller
            configuration, and platform integrations.
          </p>

          <p class="mt-3">
            Relora continues to expand secure payment support to improve
            the buying and selling experience for fashion enthusiasts,
            collectors, and luxury consumers.
          </p>

          <p class="mt-3">
            Buyers should always confirm order details before completing
            payment.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-9">
        <AccordionTrigger>
          Can I return an item?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Return eligibility depends on seller policies, item condition,
            authenticity concerns, and dispute review outcomes.
          </p>

          <p class="mt-3">
            Buyers are encouraged to carefully review all listing details,
            photos, measurements, and descriptions before purchasing
            designer clothing or archive fashion pieces.
          </p>

          <p class="mt-3">
            Items significantly different from their description may qualify
            for dispute review.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-10">
        <AccordionTrigger>
          What happens if there is a dispute?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Relora is developing dispute handling systems to assist buyers
            and sellers with issues related to delivery, authenticity,
            damaged items, or incorrect product descriptions.
          </p>

          <p class="mt-3">
            Buyers and sellers should keep shipping proof, tracking
            information, communication records, and detailed product photos
            whenever possible.
          </p>

          <p class="mt-3">
            Fraudulent activity, abuse of refunds, or dishonest behavior may
            result in account restrictions or removal from the platform.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-11">
        <AccordionTrigger>
          Why should I use Relora instead of other marketplaces?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Relora focuses on curated designer fashion instead of
            overwhelming users with low-quality mass-market listings.
          </p>

          <p class="mt-3">
            The platform is designed specifically for archive fashion,
            luxury clothing, collectible garments, designer auctions,
            vintage fashion, and fashion culture communities.
          </p>

          <p class="mt-3">
            Cleaner browsing, premium listings, fashion-focused categories,
            and auction functionality help create a more specialized
            marketplace experience.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-12">
        <AccordionTrigger>
          Is Relora free to use?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Browsing the marketplace is free. Selling fees, premium
            features, commissions, or optional marketplace tools may vary as
            the platform evolves.
          </p>

          <p class="mt-3">
            Relora continuously improves marketplace infrastructure,
            moderation systems, seller tools, and auction functionality to
            support long-term growth.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-13">
        <AccordionTrigger>
          How can I improve my chances of selling?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            High-quality product photos, accurate descriptions, transparent
            condition details, measurements, and competitive pricing help
            improve listing performance.
          </p>

          <p class="mt-3">
            Archive fashion buyers often search for specific collections,
            runway seasons, rare garments, and hard-to-find designer pieces.
          </p>

          <p class="mt-3">
            Detailed listings help build trust and improve visibility within
            the marketplace.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-14">
        <AccordionTrigger>
          Are fake or replica items allowed?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            No. Counterfeit products, replicas, fake designer items, and
            misleading listings are prohibited on Relora.
          </p>

          <p class="mt-3">
            Sellers attempting to distribute counterfeit fashion products
            may face listing removal, account suspension, or permanent
            marketplace bans.
          </p>
        </AccordionContent>
      </AccordionItem>

      <AccordionItem value="item-15">
        <AccordionTrigger>
          Is Relora focused only on luxury fashion?
        </AccordionTrigger>

        <AccordionContent>
          <p>
            Relora primarily focuses on designer fashion, archive
            collections, luxury streetwear, avant-garde fashion, and
            collectible garments.
          </p>

          <p class="mt-3">
            However, the platform also welcomes quality fashion pieces that
            align with the marketplace aesthetic and community interests.
          </p>
        </AccordionContent>
      </AccordionItem>
    </Accordion>
  </section>
</template>