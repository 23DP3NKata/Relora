import { auctionService } from '@/app/services/auctionService'
import { bidService } from '@/app/services/bidService'
import { itemService } from '@/app/services/lotService'
import { buildMediaUrl } from '@/shared/mediaUrl'
import { heroConfig } from '@/config/homeHero'
import type { AuctionListItem } from '@/types/auction'
import type { HomeLiveLot } from '@/types/home'

// 1 lot for hero + up to 6 for live auctions
const LIVE_LOTS_LIMIT = 7

// prices and timers come from the server, frontend only shows them
export async function getHomeLiveLots(): Promise<HomeLiveLot[]> {
  try {
    const auctions = await auctionService.getAuctions({
      status: 'Active',
      page: 1,
      pageSize: LIVE_LOTS_LIMIT,
    })

    const withLot = auctions.filter((auction) => auction.lotId)

    // pinned hero lot goes first
    if (heroConfig.pinnedLotId) {
      const pinned = withLot.find((auction) => auction.lotId === heroConfig.pinnedLotId)

      if (pinned) {
        withLot.splice(withLot.indexOf(pinned), 1)
        withLot.unshift(pinned)
      }
    }

    const results = await Promise.allSettled(withLot.map(loadLiveLot))

    const lots = results
      .filter((result) => result.status === 'fulfilled')
      .map((result) => (result as PromiseFulfilledResult<HomeLiveLot>).value)

    if (lots.length > 0) {
      return lots
    }
  } catch {
    // backend is down, show demo lots
  }

  return getDemoLots()
}

async function loadLiveLot(auction: AuctionListItem): Promise<HomeLiveLot> {
  const lot = await itemService.getLot(auction.lotId!)
  const cover = lot.media.find((media) => media.isCover) ?? lot.media[0]

  return {
    lotId: lot.id,
    auctionId: auction.auctionId,
    title: lot.title,
    brand: lot.brand,
    imageUrl: cover ? buildMediaUrl(cover) : '',
    sizeName: lot.sizeName,
    conditionName: lot.conditionName,
    currentPrice: auction.currentPrice ?? lot.price,
    currency: auction.currency ?? lot.currency ?? 'EUR',
    endsAt: auction.endDate,
    bidCount: null,
    isDemo: false,
  }
}

export async function getBidCount(auctionId: string): Promise<number | null> {
  try {
    const bids = await bidService.getBidsByAuction(auctionId)
    return bids.length
  } catch {
    return null
  }
}

// demo lots so the page isn't empty without backend (shown as "Preview")
function getDemoLots(): HomeLiveLot[] {
  const hour = 60 * 60 * 1000
  const now = Date.now()

  const demo = [
    { brand: 'Stone Island', title: 'Ghost Jacket', price: 184, endsIn: 0.7 * hour, imageUrl: heroConfig.poster },
    { brand: 'Balenciaga', title: 'Football Logo Zip Hoodie', price: 420, endsIn: 2.4 * hour, imageUrl: 'https://media-photos.depop.com/b1/51377749/3251202528_7ca0ed8a90b8496c9b8793bd28d3de17/P0.jpg' },
    { brand: 'Maison Margiela', title: 'Replica Sneakers', price: 240, endsIn: 5 * hour, imageUrl: 'https://media-photos.depop.com/b1/45498419/3517277733_6056232abec546e987925ec83630f810/P0.jpg' },
    { brand: 'Vetements', title: 'Campaign Logo T-Shirt', price: 180, endsIn: 9 * hour, imageUrl: 'https://media-photos.depop.com/b1/36719830/3498593736_9131744e9156480d90e996a6dcb6ec67/P0.jpg' },
    { brand: 'Prada', title: 'Knit Zip Jacket', price: 390, endsIn: 20 * hour, imageUrl: 'https://media-photos.depop.com/b1/448068812/3474488748_5e0e9243711c4cbcb6dce7afdff37d6d/P0.jpg' },
    { brand: 'Enfants Riches Déprimés', title: 'Graphic T-Shirt', price: 310, endsIn: 30 * hour, imageUrl: 'https://media-photos.depop.com/b1/51416371/3553513106_b896da1472a94103a15b55b05dd159f1/P0.jpg' },
    { brand: 'Maison Margiela', title: 'Numbers Logo T-Shirt', price: 150, endsIn: 52 * hour, imageUrl: 'https://images.thebestshops.com/product_images/original/SL12226-044_01-339d21.jpg' },
  ]

  return demo.map((item, index) => ({
    lotId: `demo-${index + 1}`,
    auctionId: null,
    title: item.title,
    brand: item.brand,
    imageUrl: item.imageUrl,
    sizeName: 'M',
    conditionName: 'Excellent',
    currentPrice: item.price,
    currency: 'EUR',
    endsAt: new Date(now + item.endsIn).toISOString(),
    bidCount: null,
    isDemo: true,
  }))
}
