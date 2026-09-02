using Relora.Items.Domain;

using MediatR;

namespace Relora.Admin.Application.Commands;

public sealed record AcceptLotCommand(Guid lotId, Guid adminId) : IRequest;
