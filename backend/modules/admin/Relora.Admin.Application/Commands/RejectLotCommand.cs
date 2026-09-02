using MediatR;

namespace Relora.Admin.Application.Commands;

public sealed record RejectLotCommand(Guid lotId, Guid adminId, string? reason) : IRequest;

