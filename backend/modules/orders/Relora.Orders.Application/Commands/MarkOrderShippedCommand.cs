using MediatR;

namespace Relora.Orders.Application.Commands;

public sealed record MarkOrderShippedCommand(
    Guid OrderId,
    Guid SellerId,
    string CarrierName,
    string TrackingNumber) : IRequest;
