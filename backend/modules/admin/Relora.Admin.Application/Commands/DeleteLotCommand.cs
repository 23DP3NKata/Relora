using MediatR;

namespace Relora.Admin.Application.Commands;

public sealed record DeleteLotCommand(Guid lotId, Guid adminId) : IRequest;
