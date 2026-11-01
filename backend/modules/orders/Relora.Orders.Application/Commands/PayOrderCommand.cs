using Relora.Orders.Application.Requests;

using MediatR;

namespace Relora.Orders.Application.Commands;

// payment works in test mode, no real money is charged
public sealed record PayOrderCommand(
    Guid OrderId,
    Guid BuyerId,
    ShippingAddressRequest? ShippingAddress = null) : IRequest;
