using Relora.Orders.Application.Models;

using MediatR;

namespace Relora.Orders.Application.Queries;

/// <summary>
/// Represents the get order details query record.
/// </summary>
public sealed record GetOrderDetailsQuery(Guid orderId, Guid userId) : IRequest<OrderDetailsDto>;
