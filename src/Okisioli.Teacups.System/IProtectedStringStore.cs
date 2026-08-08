namespace Okisioli.Teacups.System;

/// <summary>
/// Provides persistent storage for a single protected string value.
/// </summary>
public interface IProtectedStringStore
{
    /// <summary>
    /// Loads the stored value, or <see langword="null"/> when no value is stored.
    /// </summary>
    string? Load();

    /// <summary>
    /// Protects and stores the specified value.
    /// </summary>
    /// <param name="value">The value to store.</param>
    void Save(string value);

    /// <summary>
    /// Removes the stored value.
    /// </summary>
    void Clear();
}
