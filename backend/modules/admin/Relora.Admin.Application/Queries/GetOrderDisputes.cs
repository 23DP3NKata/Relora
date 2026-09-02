using Relora.Admin.Domain.Models;

using MediatR;

namespace Relora.Admin.Application.Queries;

public sealed record GetOrderDisputes(Guid AdminId) : IRequest<IReadOnlyList<AdminOrderDisputeDto>>;
