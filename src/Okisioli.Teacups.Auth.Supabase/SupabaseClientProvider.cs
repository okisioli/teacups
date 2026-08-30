using Okisioli.Teacups.System;

namespace Okisioli.Teacups.Auth.Supabase;

/// <summary>Creates and initializes a configured Supabase client.</summary>
public sealed class SupabaseClientProvider
{
    private readonly SupabaseSessionHandler? _sessionHandler;
    private Task? _initialization;

    /// <summary>Initializes a provider for a Supabase project.</summary>
    /// <param name="url">The Supabase project URL.</param>
    /// <param name="key">The Supabase anonymous or publishable key.</param>
    /// <param name="sessionStore">Optional protected storage for persisted sessions.</param>
    public SupabaseClientProvider(string url, string key, IProtectedStringStore? sessionStore = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        _sessionHandler = sessionStore is null ? null : new SupabaseSessionHandler(sessionStore);
        Client = new global::Supabase.Client(url, key, new global::Supabase.SupabaseOptions
        {
            AutoConnectRealtime = false,
            AutoRefreshToken = true,
            SessionHandler = _sessionHandler is not null
                ? _sessionHandler
                : new global::Supabase.DefaultSupabaseSessionHandler()
        });
    }

    /// <summary>Gets the configured Supabase client.</summary>
    public global::Supabase.Client Client { get; }

    /// <summary>Initializes the client and restores any persisted session.</summary>
    public Task InitializeAsync() => _initialization ??= InitializeClientAsync();

    internal void SetSessionPersistence(bool enabled) => _sessionHandler?.SetPersistence(enabled);

    private async Task InitializeClientAsync()
    {
        Client.Auth.LoadSession();
        await Client.InitializeAsync();
    }
}
