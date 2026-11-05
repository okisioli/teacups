using Microsoft.Extensions.DependencyInjection;
using Okisioli.Teacups.Auth;
using Okisioli.Teacups.Store;

namespace Okisioli.Teacups.Auth.Supabase;

public static class ServiceCollectionExtensions
{
    /// <summary>Registers per-scope authentication. Single-user native hosts can explicitly select Singleton.</summary>
    /// <remarks>Web hosts supply a per-user ISecureKeyValueStore through DI and retain the protected external sign-in request across redirects.</remarks>
    public static IServiceCollection AddSupabaseAuth(
        this IServiceCollection services,
        string url,
        string key,
        ISecureKeyValueStore? sessionStore = null,
        string? passwordResetRedirectUrl = null,
        string sessionStorageKey = SupabaseClientProvider.DefaultSessionStorageKey,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionStorageKey);
        if (lifetime is not ServiceLifetime.Scoped and not ServiceLifetime.Singleton)
            throw new ArgumentOutOfRangeException(nameof(lifetime), "Use Scoped for per-user hosts or Singleton for single-user native hosts.");
        if (sessionStore is not null && lifetime != ServiceLifetime.Singleton)
            throw new ArgumentException("A shared store instance is only valid for an explicitly single-user Singleton registration. Register a per-user store in DI for Scoped authentication.", nameof(sessionStore));

        services.Add(new ServiceDescriptor(typeof(SupabaseClientProvider), provider =>
            new SupabaseClientProvider(url, key, sessionStore ?? provider.GetService<ISecureKeyValueStore>(), sessionStorageKey), lifetime));
        services.Add(new ServiceDescriptor(typeof(SupabaseAuthService), provider =>
            new SupabaseAuthService(provider.GetRequiredService<SupabaseClientProvider>(), passwordResetRedirectUrl), lifetime));
        services.Add(new ServiceDescriptor(typeof(IAuthService), provider => provider.GetRequiredService<SupabaseAuthService>(), lifetime));
        services.Add(new ServiceDescriptor(typeof(IExternalAuthService), provider => provider.GetRequiredService<SupabaseAuthService>(), lifetime));
        return services;
    }
}
