using Relora.Admin.Domain.Models;

using MediatR;

namespace Relora.Admin.Application.Queries;

public sealed record GetAdminDashboardData(Guid adminId) : IRequest<AdminDashboardDto>;
