using Relora.Identity.Application.Models;

using MediatR;

namespace Relora.Identity.Application.Queries;

public sealed record GetUserProfileQuery (string username) : IRequest<UserProfileDto>;
