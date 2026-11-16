using Relora.Support.Domain;

using MediatR;

namespace Relora.Admin.Application.Commands;

public sealed record ChangeSupportRequestStatusCommand(Guid RequestId, Guid AdminId, SupportRequestStatus Status) : IRequest;
