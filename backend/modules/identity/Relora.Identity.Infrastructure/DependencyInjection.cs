using System.Text;

using Relora.Identity.Application.Interfaces;
using Relora.Identity.Infrastructure.Configuration;
using Relora.Identity.Infrastructure.Hasher;
using Relora.Identity.Infrastructure.Repository;
using Relora.Identity.Infrastructure.Token;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Relora.Identity.Infrastructure;

/// <summary>
/// Represents the dependency injection class.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds identity module.
    /// </summary>
    /// <param name="services">Services.</param>
    /// <param name="configuration">Configuration.</param>
    /// <returns>The operation result.</returns>
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();

        services
            .AddOptions<JwtOptions>()
            .Bind(jwtSection)
            .Validate(options => !string.IsNullOrWhiteSpace(options.Secret),
                "Jwt secret is required. Configure Jwt:Secret via UserSecrets or environment variable JWT__Secret")

            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "Jwt issuer is required")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "Jwt audience is required")
            .Validate(options => options.ExpirationInMinutes > 0, "Jwt expiration must be greater than 0")
            .ValidateOnStart();

        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["access_token"];

                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}
