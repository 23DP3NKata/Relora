using Relora.Items.Application.Models;
using MediatR;

namespace Relora.Items.Application.Queries;

public sealed record GetLotsByUserIdQuery(Guid UserId) : IRequest<IReadOnlyList<MyLotListItemDto>>;
