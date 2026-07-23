using Microsoft.Extensions.DependencyInjection;
using Okisioli.Teacups.Auth.Provider;

namespace Okisioli.Teacups.Auth.Provider.Supabase;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSupabaseAuth(
        this IServiceCollection services,
        string url,
        string key,
        IAuthSessionStore? sessionStore = null,
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
