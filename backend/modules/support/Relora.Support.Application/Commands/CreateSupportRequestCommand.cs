using MediatR;

namespace Relora.Support.Application.Commands;

public sealed record CreateSupportRequestCommand(string Email, string Category, string Subject, string Message) : IRequest<Guid>;
