using Microsoft.Extensions.DependencyInjection;
using Okisioli.Teacups.Auth;
using Okisioli.Teacups.Store;

namespace Okisioli.Teacups.Auth.Supabase;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSupabaseAuth(
        this IServiceCollection services,
        string url,
        string key,
        ISecureKeyValueStore? sessionStore = null,
        string? passwordResetRedirectUrl = null,
        string sessionStorageKey = SupabaseClientProvider.DefaultSessionStorageKey)
    {
        ArgumentNullException.ThrowIfNull(services);

        var clientProvider = new SupabaseClientProvider(url, key, sessionStore, sessionStorageKey);
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
