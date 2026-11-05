using Okisioli.Teacups.Store;

namespace Okisioli.Teacups.Auth.Supabase;

public sealed class SupabaseClientProvider : IDisposable
{
    /// <summary>The default key used to persist a Supabase session.</summary>
    public const string DefaultSessionStorageKey = "Okisioli.Teacups.Auth.Supabase.Session";

    private readonly SupabaseSessionHandler? sessionHandler;
    private readonly object initializationGate = new();
    private Task? initialization;
    private bool disposed;

    public SupabaseClientProvider(
        string url,
        string key,
        ISecureKeyValueStore? sessionStore = null,
        string sessionStorageKey = DefaultSessionStorageKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionStorageKey);

        sessionHandler = sessionStore is null
            ? null
            : new SupabaseSessionHandler(sessionStore, sessionStorageKey);
        Client = new global::Supabase.Client(url, key, new global::Supabase.SupabaseOptions
        {
            AutoConnectRealtime = false,
            AutoRefreshToken = true,
            SessionHandler = sessionHandler is not null
                ? sessionHandler
                : new global::Supabase.DefaultSupabaseSessionHandler()
        });
    }

    public global::Supabase.Client Client { get; }

    public Task InitializeAsync()
    {
        lock (initializationGate)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            return initialization ??= InitializeClientAsync();
        }
    }

    /// <summary>Stops the refresh timer when a host scope ends without signing the user out.</summary>
    public void Dispose()
    {
        lock (initializationGate)
        {
            if (disposed) return;
            disposed = true;
            Client.Auth.Shutdown();
        }
    }

    internal void SetSessionPersistence(bool enabled) => sessionHandler?.SetPersistence(enabled);

    private async Task InitializeClientAsync()
    {
        Client.Auth.LoadSession();
        await Client.InitializeAsync();
    }
}
