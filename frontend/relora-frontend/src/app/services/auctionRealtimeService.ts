import * as signalR from '@microsoft/signalr'

import { API_BASE_URL } from '@/api'

export type BidPlacedEvent = {
  auctionId: string
  bidderId: string
  amount: number
  currency: string
}

export type AuctionEndedEvent = {
  auctionId: string
  winnerId: string | null
  winningBidId: string | null
}

export type AuctionStartedEvent = {
  auctionId: string
  lotId: string | null
}

type RealtimeEventMap = {
  BidPlaced: BidPlacedEvent
  AuctionEnded: AuctionEndedEvent
  AuctionStarted: AuctionStartedEvent
}

type RealtimeEventName = keyof RealtimeEventMap
type RealtimeHandler<TEventName extends RealtimeEventName> =
  (payload: RealtimeEventMap[TEventName]) => void | Promise<void>

class AuctionRealtimeService {
  private connection: signalR.HubConnection | null = null
  private startPromise: Promise<void> | null = null
  private currentAuctionId: string | null = null
  private currentLotId: string | null = null
  private readonly handlers = new Map<RealtimeEventName, Set<RealtimeHandler<any>>>()

  on<TEventName extends RealtimeEventName>(
    eventName: TEventName,
    handler: RealtimeHandler<TEventName>,
  ): () => void {
    const handlers = this.handlers.get(eventName) ?? new Set<RealtimeHandler<any>>()

    handlers.add(handler)
    this.handlers.set(eventName, handlers)

    return () => {
      handlers.delete(handler)
    }
  }

  async joinAuction(auctionId: string): Promise<void> {
    if (!auctionId) return

    const connection = this.getConnection()
    await this.start()

    if (this.currentAuctionId && this.currentAuctionId !== auctionId) {
      await connection.invoke('LeaveAuction', this.currentAuctionId)
    }

    await connection.invoke('JoinAuction', auctionId)
    this.currentAuctionId = auctionId
  }

  async leaveAuction(auctionId = this.currentAuctionId): Promise<void> {
    if (!auctionId || !this.connection) return

    if (this.connection.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('LeaveAuction', auctionId)
    }

    if (this.currentAuctionId === auctionId) {
      this.currentAuctionId = null
    }
  }

  async joinLot(lotId: string): Promise<void> {
    if (!lotId) return

    const connection = this.getConnection()
    await this.start()

    if (this.currentLotId && this.currentLotId !== lotId) {
      await connection.invoke('LeaveLot', this.currentLotId)
    }

    await connection.invoke('JoinLot', lotId)
    this.currentLotId = lotId
  }

  async leaveLot(lotId = this.currentLotId): Promise<void> {
    if (!lotId || !this.connection) return

    if (this.connection.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('LeaveLot', lotId)
    }

    if (this.currentLotId === lotId) {
      this.currentLotId = null
    }
  }

  async stop(): Promise<void> {
    if (!this.connection) return

    const connection = this.connection

    try {
      await Promise.allSettled([
        this.leaveAuction(),
        this.leaveLot(),
      ])
      await connection.stop()
    } finally {
      this.currentAuctionId = null
      this.currentLotId = null
      this.connection = null
      this.startPromise = null
    }
  }

  private getConnection(): signalR.HubConnection {
    if (this.connection) {
      return this.connection
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_BASE_URL}/hubs/auction`, {
        withCredentials: true,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build()

    connection.on('BidPlaced', (payload: BidPlacedEvent) => {
      void this.emit('BidPlaced', payload)
    })

    connection.on('AuctionEnded', (payload: AuctionEndedEvent) => {
      void this.emit('AuctionEnded', payload)
    })

    connection.on('AuctionStarted', (payload: AuctionStartedEvent) => {
      void this.emit('AuctionStarted', payload)
    })

    connection.onreconnected(() => {
      if (this.currentAuctionId) {
        void connection.invoke('JoinAuction', this.currentAuctionId)
      }

      if (this.currentLotId) {
        void connection.invoke('JoinLot', this.currentLotId)
      }
    })

    this.connection = connection

    return connection
  }

  private async start(): Promise<void> {
    const connection = this.getConnection()

    if (connection.state === signalR.HubConnectionState.Connected) {
      return
    }

    if (!this.startPromise) {
      this.startPromise = connection
        .start()
        .finally(() => {
          this.startPromise = null
        })
    }

    await this.startPromise
  }

  private async emit<TEventName extends RealtimeEventName>(
    eventName: TEventName,
    payload: RealtimeEventMap[TEventName],
  ): Promise<void> {
    const handlers = this.handlers.get(eventName)

    if (!handlers?.size) return

    await Promise.all(
      [...handlers].map(async (handler) => {
        await handler(payload)
      }),
    )
  }
}

export const auctionRealtimeService = new AuctionRealtimeService()
