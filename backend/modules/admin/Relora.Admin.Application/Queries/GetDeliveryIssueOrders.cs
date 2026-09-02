using Relora.Admin.Domain.Models;

using MediatR;

namespace Relora.Admin.Application.Queries;

public sealed record GetDeliveryIssueOrders(Guid AdminId) : IRequest<IReadOnlyList<DeliveryIssueOrderDto>>;
