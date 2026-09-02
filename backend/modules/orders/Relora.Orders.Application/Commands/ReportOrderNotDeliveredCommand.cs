using MediatR;

namespace Relora.Orders.Application.Commands;

public sealed record ReportOrderNotDeliveredCommand(
    Guid OrderId,
    Guid BuyerId,
    string? Reason) : IRequest;
