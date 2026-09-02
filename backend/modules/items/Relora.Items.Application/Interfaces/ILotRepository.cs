using Relora.Items.Domain;

namespace Relora.Items.Application.Interfaces;

/// <summary>
/// Represents the i lot repository interface.
/// </summary>
public interface ILotRepository
{
    Task<Lot?> GetLotById(Guid? id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Lot>> GetLotsBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken);
    Task SaveLotAsync(Lot lot, CancellationToken cancellationToken);
    Task AddLotAsync(Lot lot, CancellationToken cancellationToken);
    Task DeleteLotAsync(Lot lot, CancellationToken cancellationToken);
    IQueryable<Lot> GetQueryable();
}
