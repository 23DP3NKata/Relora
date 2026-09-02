using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Relora.Realtime.Hubs;

namespace Relora.Realtime.Extensions;

/// <summary>
/// Represents the realtime endpoint extensions class.
/// </summary>
public static class RealtimeEndpointExtensions
{
    public static IEndpointRouteBuilder MapReloraRealtime(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<AuctionHub>("/hubs/auction");
        return endpoints;
    }
}
