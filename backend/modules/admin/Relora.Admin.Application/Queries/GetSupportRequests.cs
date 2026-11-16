using Relora.Admin.Domain.Models;

using MediatR;

namespace Relora.Admin.Application.Queries;

public sealed record GetSupportRequests(Guid AdminId) : IRequest<IReadOnlyList<AdminSupportRequestDto>>;
