using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Orders.Domain;

namespace Relora.Orders.Application.Interfaces;
/// <summary>
/// Represents the i order repository interface.
/// </summary>
public interface IOrderRepository
{
    Task AddOrderAsync(Order order, CancellationToken cancellationToken);
    Task <Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task UpdateOrderAsync(Order order, CancellationToken cancellationToken);
    Task DeleteOrderAsync(Guid orderId, CancellationToken cancellationToken);
    Task<bool> ExistsForAuctionAsync(Guid auctionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> GetOrdersByBuyerId(Guid buyerId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> GetOrdersBySellerId(Guid sellerId, CancellationToken cancellationToken);
    Task<Order> GetOrderDetailsById (Guid orderId, CancellationToken cancellationToken);
}
