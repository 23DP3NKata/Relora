namespace Relora.Shared.Application.Payments;

public interface IPaymentRefundService
{
    Task RefundOrderAsync(Guid orderId, CancellationToken cancellationToken);
}
