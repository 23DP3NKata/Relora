using Relora.Orders.Domain;

namespace Relora.Orders.Application.Interfaces;

public interface IOrderDisputeRepository
{
    Task AddAsync(OrderDispute dispute, CancellationToken cancellationToken);
    Task<OrderDispute?> GetByIdAsync(Guid disputeId, CancellationToken cancellationToken);
    Task<OrderDispute?> GetOpenByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task<OrderDispute?> GetLatestByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task UpdateAsync(OrderDispute dispute, CancellationToken cancellationToken);
}
