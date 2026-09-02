using MediatR;

namespace Relora.Items.Application.Commands;

/// <summary>
/// Represents the delete lot command record.
/// </summary>
public record DeleteLotCommand (Guid lotId, Guid sellerId) : IRequest;