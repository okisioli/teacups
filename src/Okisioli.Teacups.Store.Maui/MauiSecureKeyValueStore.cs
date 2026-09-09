using Microsoft.Maui.Storage;

namespace Okisioli.Teacups.Store.Maui;

/// <summary>
/// Stores named string values using the platform implementation provided by .NET MAUI Essentials.
/// </summary>
public sealed class MauiSecureKeyValueStore : ISecureKeyValueStore
{
    private readonly ISecureStorage secureStorage;

    /// <summary>
    /// Initializes a secure key/value store using the default .NET MAUI secure storage implementation.
    /// </summary>
    public MauiSecureKeyValueStore()
        : this(SecureStorage.Default)
    {
    }

    /// <summary>
    /// Initializes a secure key/value store using the supplied .NET MAUI secure storage implementation.
    /// </summary>
    /// <param name="secureStorage">The secure storage implementation to wrap.</param>
    public MauiSecureKeyValueStore(ISecureStorage secureStorage)
    {
        ArgumentNullException.ThrowIfNull(secureStorage);
        this.secureStorage = secureStorage;
    }

    /// <inheritdoc />
    public Task<string?> GetAsync(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return secureStorage.GetAsync(key);
    }

    /// <inheritdoc />
    public Task SetAsync(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);
        return secureStorage.SetAsync(key, value);
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        secureStorage.Remove(key);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ClearAsync()
    {
        secureStorage.RemoveAll();
        return Task.CompletedTask;
    }
}
