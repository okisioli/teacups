namespace Okisioli.Teacups.Store;

/// <summary>
/// Provides persistent secure storage for named string values.
/// </summary>
public interface ISecureKeyValueStore
{
    /// <summary>
    /// Gets the value stored for the specified key, or <see langword="null"/> when no value is stored.
    /// </summary>
    /// <param name="key">The key that identifies the value.</param>
    string? Get(string key);

    /// <summary>
    /// Securely stores the specified value under the specified key.
    /// </summary>
    /// <param name="key">The key that identifies the value.</param>
    /// <param name="value">The value to store.</param>
    void Set(string key, string value);

    /// <summary>
    /// Removes the value stored for the specified key.
    /// </summary>
    /// <param name="key">The key that identifies the value to remove.</param>
    void Remove(string key);

    /// <summary>
    /// Removes all values stored by this store.
    /// </summary>
    void Clear();
}
