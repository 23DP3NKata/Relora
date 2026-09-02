using Relora.Auctions.Application.Interfaces;
using Relora.Auctions.Domain;
using Relora.Persistance;

using Microsoft.EntityFrameworkCore;

namespace Relora.Auctions.Infrastructure.Repository;

/// <summary>
/// Represents the auction repository class.
/// </summary>
public class AuctionRepository : IAuctionRepository
{
    private readonly ReloraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionRepository"/> class.
    /// </summary>
    /// <param name="context">Context.</param>
    public AuctionRepository(ReloraDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets auction by id.
    /// </summary>
    /// <param name="id">Entity identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<Auction?> GetAuctionById(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Auctions
            .Include(a => a.Bids)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Auction?> GetAuctionByLotId(Guid lotId, CancellationToken cancellationToken)
    {
        return await _context.Auctions
            .FirstOrDefaultAsync(a => a.LotId == lotId, cancellationToken);
    }

    /// <summary>
    /// Adds auction async.
    /// </summary>
    /// <param name="auction">Auction.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddAuctionAsync(Auction auction, CancellationToken cancellationToken)
    {
        await _context.Auctions.AddAsync(auction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Performs the save auction async operation.
    /// </summary>
    /// <param name="auction">Auction.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SaveAuctionAsync(Auction auction, CancellationToken cancellationToken)
    {
        if (_context.Entry(auction).State == EntityState.Detached)
        {
            _context.Auctions.Attach(auction);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
