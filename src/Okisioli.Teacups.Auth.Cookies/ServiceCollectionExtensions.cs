using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Okisioli.Teacups.Auth.Cookies;

/// <summary>Registers and configures cookie authentication services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds cookie-based <see cref="IAuthService"/> authentication backed by an application's credential validator.</summary>
    public static IServiceCollection AddCookieAuth<TCredentialValidator>(
        this IServiceCollection services,
        Action<CookieAuthenticationOptions>? configure = null)
        where TCredentialValidator : class, ICredentialValidator
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHttpContextAccessor();
        services.AddScoped<ICredentialValidator, TCredentialValidator>();
        services.AddScoped<IAuthService, CookieAuthService>();

        var authentication = services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
        if (configure is null)
        {
            authentication.AddCookie();
        }
        else
        {
            authentication.AddCookie(configure);
        }

        services.AddAuthorization();
        return services;
    }

    /// <summary>Adds cookie authentication and authorization middleware to the request pipeline.</summary>
    public static IApplicationBuilder UseCookieAuth(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
