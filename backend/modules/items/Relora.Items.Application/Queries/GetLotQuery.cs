using Relora.Items.Application.Models;
using MediatR;

namespace Relora.Items.Application.Queries;

/// <summary>
/// Represents the get lot query record.
/// </summary>
public record GetLotQuery(
    Guid lotId,
    Guid? ViewerUserId = null,
    bool ViewerIsAdmin = false) : IRequest<LotDto>;
