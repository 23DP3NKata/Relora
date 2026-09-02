using Relora.Persistance;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;


/// <summary>
/// Represents the dependency injection class.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ReloraDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Postgres")
            ));

        return services;
    }
}
