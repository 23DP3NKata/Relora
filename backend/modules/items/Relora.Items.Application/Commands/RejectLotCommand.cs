using MediatR;

namespace Relora.Items.Application.Commands;

public sealed record RejectLotCommand(Guid LotId) : IRequest<Guid>;
