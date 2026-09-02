using Relora.Payments.Application.Interfaces;
using Relora.Payments.Domain;
using Relora.Persistance;

using Microsoft.EntityFrameworkCore;

namespace Relora.Payments.Infrastructure.Repository;

public class PaymentRepository(ReloraDbContext reloraDbContext) : IPaymentRepository
{
    private readonly ReloraDbContext _reloraDbContext = reloraDbContext;

    public Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken)
    {
        _reloraDbContext.Payments.Add(payment);
        return _reloraDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task DeletePaymentAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        _reloraDbContext.Payments.Remove(new Payment { Id = paymentId });
        return _reloraDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Payment?> GetPaymentByIdAsync(Guid paymentId, CancellationToken cancellationToken)
    {
       return _reloraDbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
    }

    public Task<Payment?> GetByStripeSessionIdAsync(string stripeSessionId, CancellationToken cancellationToken)
    {
        return _reloraDbContext.Payments
            .FirstOrDefaultAsync(payment => payment.StripeSessionId == stripeSessionId, cancellationToken);
    }

    public Task<Payment?> GetLatestByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return _reloraDbContext.Payments
            .Where(payment => payment.OrderId == orderId)
            .OrderByDescending(payment => payment.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task UpdatePaymentAsync(Payment payment, CancellationToken cancellationToken)
    {
        _reloraDbContext.Update(payment);
        return _reloraDbContext.SaveChangesAsync(cancellationToken);
    }
}
