using Relora.Orders.Domain.Enums;

using MediatR;

namespace Relora.Orders.Application.Commands;

public sealed record OpenOrderDisputeCommand(
    Guid OrderId,
    Guid BuyerId,
    OrderDisputeReason Reason,
    string Description,
    IReadOnlyList<string>? EvidenceKeys) : IRequest<Guid>;
