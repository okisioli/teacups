using Microsoft.Extensions.DependencyInjection;
using Okisioli.Teacups.Auth;
using Okisioli.Teacups.System;

namespace Okisioli.Teacups.Auth.Supabase;

public static class ServiceCollectionExtensions
{
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
