using Relora.Admin.Domain.Models;

using MediatR;

namespace Relora.Admin.Application.Queries;

public sealed record GetListOfPendingLots(Guid adminId) : IRequest<IReadOnlyList<PendingLotPreviewDto>>;
