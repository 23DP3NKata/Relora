using MediatR;

namespace Relora.Items.Application.Commands;

public sealed record AcceptLotCommand(Guid LotId) : IRequest<Guid>;
