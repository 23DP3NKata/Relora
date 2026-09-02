using Relora.Bids.Application.Interfaces;
using Relora.Persistance;
using Relora.Bids.Domain;

namespace Relora.Bids.Infrastructure.Repository;

/// <summary>
/// Represents the bid repository class.
/// </summary>
public sealed class BidRepository : IBidRepository
{
    private readonly ReloraDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="BidRepository"/> class.
    /// </summary>
    /// <param name="dbContext">Db context.</param>
    public BidRepository(ReloraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Adds bid async.
    /// </summary>
    /// <param name="bid">Identifier of b.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddBidAsync(Domain.Bid bid, CancellationToken cancellationToken)
    {
        await _dbContext.Bids.AddAsync(bid, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Gets bid by id.
    /// </summary>
    /// <param name="bidId">Identifier of bid.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<Domain.Bid?> GetBidById(Guid bidId, CancellationToken cancellationToken)
    {
        return await _dbContext.Bids.FindAsync(new object[] { bidId }, cancellationToken);
    }

    /// <summary>
    /// Performs the save bid async operation.
    /// </summary>
    /// <param name="bid">Identifier of b.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SaveBidAsync(Domain.Bid bid, CancellationToken cancellationToken)
    {
        _dbContext.Bids.Update(bid);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
