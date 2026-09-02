using Relora.Admin.Domain.Models;

using MediatR;

namespace Relora.Admin.Application.Queries;

public sealed record GetOverdueShipmentOrders(Guid AdminId) : IRequest<IReadOnlyList<OverdueShipmentOrderDto>>;
