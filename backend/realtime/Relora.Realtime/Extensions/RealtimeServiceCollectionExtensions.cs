using Relora.Auctions.Application.Interfaces;
using Relora.Realtime.Notifier;
using Microsoft.Extensions.DependencyInjection;

namespace Relora.Realtime.Extensions;

/// <summary>
/// Represents the realtime service collection extensions class.
/// </summary>
public static class RealtimeServiceCollectionExtensions
{
    /// <summary>
    /// Adds relora realtime.
    /// </summary>
    /// <param name="services">Services.</param>
    /// <returns>The operation result.</returns>
    public static IServiceCollection AddReloraRealtime(this IServiceCollection services)
    {
        services.AddSignalR();

        services.AddScoped<
            IAuctionRealtimeNotifier,
            SignalRAuctionRealtimeNotifier>();

        return services;
    }
}
