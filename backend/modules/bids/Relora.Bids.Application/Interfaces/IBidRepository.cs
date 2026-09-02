using Relora.Bids.Domain;

namespace Relora.Bids.Application.Interfaces;

/// <summary>
/// Represents the i bid repository interface.
/// </summary>
public interface IBidRepository
{
    Task<Bid?> GetBidById(Guid bidId, CancellationToken cancellationToken);
    Task AddBidAsync(Bid bid, CancellationToken cancellationToken);
    Task SaveBidAsync(Bid bid, CancellationToken cancellationToken);
}
