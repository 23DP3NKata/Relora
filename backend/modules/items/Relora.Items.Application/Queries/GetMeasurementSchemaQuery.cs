using Relora.Items.Application.Models;

using MediatR;

namespace Relora.Items.Application.Queries;

public sealed record GetMeasurementSchemaQuery(Guid CategoryId) : IRequest<IReadOnlyList<MeasurementDefinitionDto>>;
