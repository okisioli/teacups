using Olivercode.Dry.Auth.Provider;

namespace Olivercode.Dry.Auth.Provider.Supabase;

public sealed class SupabaseClientProvider
{
    private readonly SupabaseSessionHandler? _sessionHandler;
    private Task? _initialization;

    public SupabaseClientProvider(string url, string key, IAuthSessionStore? sessionStore = null)
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

    public global::Supabase.Client Client { get; }

    public Task InitializeAsync() => _initialization ??= InitializeClientAsync();

    internal void SetSessionPersistence(bool enabled) => _sessionHandler?.SetPersistence(enabled);

    private async Task InitializeClientAsync()
    {
        Client.Auth.LoadSession();
        await Client.InitializeAsync();
    }
}
