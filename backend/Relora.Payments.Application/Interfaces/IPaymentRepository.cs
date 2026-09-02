using Relora.Payments.Domain;

namespace Relora.Payments.Application.Interfaces;

public interface IPaymentRepository
{
    Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken);
    Task<Payment?> GetPaymentByIdAsync(Guid paymentId, CancellationToken cancellationToken);
    Task<Payment?> GetByStripeSessionIdAsync(string stripeSessionId, CancellationToken cancellationToken);
    Task<Payment?> GetLatestByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task UpdatePaymentAsync(Payment payment, CancellationToken cancellationToken);
    Task DeletePaymentAsync(Guid paymentId, CancellationToken cancellationToken);
}
