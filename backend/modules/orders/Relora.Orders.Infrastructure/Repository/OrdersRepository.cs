using Relora.Orders.Application.Interfaces;
using Relora.Orders.Domain;
using Relora.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Relora.Orders.Infrastructure.Repository;

/// <summary>
/// Represents the orders repository class.
/// </summary>
public sealed class OrdersRepository (ReloraDbContext reloraDbContext) : IOrderRepository
{
    private readonly ReloraDbContext _reloraDbContext = reloraDbContext;

    /// <summary>
    /// Adds order async.
    /// </summary>
    /// <param name="order">Order.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddOrderAsync(Order order, CancellationToken cancellationToken)
    {
        _reloraDbContext.Orders.Add(order);
        await _reloraDbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Gets orders by buyer id.
    /// </summary>
    /// <param name="buyerId">Identifier of buyer.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<IReadOnlyList<Order>> GetOrdersByBuyerId(Guid buyerId, CancellationToken cancellationToken)
    {
        return await _reloraDbContext.Orders.Where(o => o.BuyerId == buyerId).ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// Gets orders by seller id.
    /// </summary>
    /// <param name="sellerId">Identifier of seller.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<IReadOnlyList<Order>> GetOrdersBySellerId(Guid sellerId, CancellationToken cancellationToken)
    {
        return await _reloraDbContext.Orders.Where(o => o.SellerId == sellerId).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets order by id async.
    /// </summary>
    /// <param name="orderId">Identifier of order.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await _reloraDbContext.Orders.FindAsync(new object[] { orderId }, cancellationToken);
    }

    /// <summary>
    /// Updates order async.
    /// </summary>
    /// <param name="order">Order.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        _reloraDbContext.Orders.Update(order);
        await _reloraDbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes order async.
    /// </summary>
    /// <param name="orderId">Identifier of order.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await GetOrderByIdAsync(orderId, cancellationToken);

        if (order != null)
        {
            _reloraDbContext.Orders.Remove(order);
            await _reloraDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Performs the exists for auction async operation.
    /// </summary>
    /// <param name="auctionId">Identifier of auction.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<bool> ExistsForAuctionAsync(Guid auctionId, CancellationToken cancellationToken)
    {
        return await _reloraDbContext.Orders.AnyAsync(o => o.AuctionId == auctionId, cancellationToken);
    }

    /// <summary>
    /// Gets order details by id.
    /// </summary>
    /// <param name="orderId">Identifier of order.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task that returns the operation result.</returns>
    public async Task<Order> GetOrderDetailsById(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _reloraDbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");
        }

        return order;
    }
}
