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

    return results
      .filter((result) => result.status === 'fulfilled')
      .map((result) => (result as PromiseFulfilledResult<HomeLiveLot>).value)
  } catch {
    // backend is down, page shows the empty state
    return []
  }
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

// showcase lot for the hero while there are no real auctions (shown as "Preview")
export function getShowcaseLot(): HomeLiveLot {
  return {
    lotId: 'showcase',
    auctionId: null,
    title: 'Ghost Jacket',
    brand: 'Stone Island',
    imageUrl: heroConfig.poster,
    sizeName: 'M',
    conditionName: 'Excellent',
    currentPrice: 184,
    currency: 'EUR',
    endsAt: new Date(Date.now() + 42 * 60 * 1000).toISOString(),
    bidCount: null,
    isDemo: true,
  }
}
