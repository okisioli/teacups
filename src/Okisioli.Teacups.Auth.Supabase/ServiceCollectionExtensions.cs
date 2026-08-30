using Microsoft.Extensions.DependencyInjection;
using Okisioli.Teacups.Auth;
using Okisioli.Teacups.System;

namespace Okisioli.Teacups.Auth.Supabase;

/// <summary>Registers Supabase-backed authentication services.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds a Supabase implementation of <see cref="IAuthService"/>.</summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="url">The Supabase project URL.</param>
    /// <param name="key">The Supabase anonymous or publishable key.</param>
    /// <param name="sessionStore">Optional protected storage for persisted sessions.</param>
    /// <param name="passwordResetRedirectUrl">Optional URL to which password-reset emails redirect.</param>
    public static IServiceCollection AddSupabaseAuth(
        this IServiceCollection services,
        string url,
        string key,
        IProtectedStringStore? sessionStore = null,
        string? passwordResetRedirectUrl = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var clientProvider = new SupabaseClientProvider(url, key, sessionStore);
        services.AddSingleton(clientProvider);
        services.AddSingleton<IAuthService>(_ => new SupabaseAuthService(
            clientProvider,
            passwordResetRedirectUrl));

        if (sessionStore is not null)
        {
            services.AddSingleton(sessionStore);
        }

        return services;
    }
}
