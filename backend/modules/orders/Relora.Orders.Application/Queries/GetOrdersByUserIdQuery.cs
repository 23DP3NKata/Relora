using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Relora.Orders.Application.Models;

using MediatR;

namespace Relora.Orders.Application.Queries;

/// <summary>
/// Represents the get orders by user id query record.
/// </summary>
public sealed record GetOrdersByUserIdQuery (Guid userId) : IRequest<IReadOnlyList<OrderListItemDto>>;

