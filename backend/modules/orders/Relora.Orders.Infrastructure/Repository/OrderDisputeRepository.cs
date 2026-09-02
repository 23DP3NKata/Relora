using Relora.Orders.Application.Interfaces;
using Relora.Orders.Domain;
using Relora.Orders.Domain.Enums;
using Relora.Persistance;

using Microsoft.EntityFrameworkCore;

namespace Relora.Orders.Infrastructure.Repository;

public sealed class OrderDisputeRepository(ReloraDbContext context) : IOrderDisputeRepository
{
    private readonly ReloraDbContext _context = context;

    public async Task AddAsync(OrderDispute dispute, CancellationToken cancellationToken)
    {
        await _context.OrderDisputes.AddAsync(dispute, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<OrderDispute?> GetByIdAsync(Guid disputeId, CancellationToken cancellationToken)
    {
        return _context.OrderDisputes
            .Include(x => x.Evidence)
            .FirstOrDefaultAsync(x => x.Id == disputeId, cancellationToken);
    }

    public Task<OrderDispute?> GetOpenByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return _context.OrderDisputes
            .Include(x => x.Evidence)
            .FirstOrDefaultAsync(
                x => x.OrderId == orderId && x.Status == OrderDisputeStatus.Open,
                cancellationToken);
    }

    public Task<OrderDispute?> GetLatestByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return _context.OrderDisputes
            .Include(x => x.Evidence)
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.OpenedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(OrderDispute dispute, CancellationToken cancellationToken)
    {
        _context.OrderDisputes.Update(dispute);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
