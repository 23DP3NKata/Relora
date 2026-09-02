using MediatR;

namespace Relora.Identity.Application.Commands;

public sealed record UserPreferencesCommand(string? preference) : IRequest;