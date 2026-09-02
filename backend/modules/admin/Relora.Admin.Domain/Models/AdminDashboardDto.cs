namespace Relora.Admin.Domain.Models;

public sealed class AdminDashboardDto
{
    public int UsersCount { get; set; }
    public int ActiveAuctionsCount { get; set; }
    public int PendingLotsCount { get; set; }
}
