using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Relora.Shared.Infrastructure.Media;

/// <summary>
/// Represents the media dependency injection class.
/// </summary>
public static class MediaDependencyInjection
{
    /// <summary>
    /// Adds media.
    /// </summary>
    /// <param name="services">Services.</param>
    /// <param name="configuration">Configuration.</param>
    /// <returns>The operation result.</returns>
    public static IServiceCollection AddMedia(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MediaOptions>(configuration.GetSection(MediaOptions.SectionName));
        services.AddScoped<IMediaUploader, MediaUploader>();
        return services;
    }
}
