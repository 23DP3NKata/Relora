using MediatR;

namespace Relora.Orders.Application.Commands;

public sealed record ConfirmOrderReceivedCommand(Guid OrderId, Guid BuyerId) : IRequest;
