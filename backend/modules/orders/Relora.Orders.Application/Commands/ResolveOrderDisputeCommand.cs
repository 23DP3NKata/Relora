using Relora.Orders.Domain.Enums;

using MediatR;

namespace Relora.Orders.Application.Commands;

public sealed record ResolveOrderDisputeCommand(
    Guid DisputeId,
    Guid AdminId,
    OrderDisputeDecision Decision,
    string Reason) : IRequest;
