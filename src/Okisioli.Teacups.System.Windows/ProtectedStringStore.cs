using Okisioli.Teacups.System;

namespace Okisioli.Teacups.System.Windows;

/// <summary>
/// Stores a single string value protected with Windows Data Protection API (DPAPI).
/// </summary>
public sealed class ProtectedStringStore : IProtectedStringStore
{
    private readonly ProtectedFileStore store;

    /// <summary>
    /// Initializes a protected string store.
    /// </summary>
    /// <param name="filePath">
    /// The file to store data in. A file name uses the current directory; prefer an application-specific path below
    /// local application data. Parent directories are created automatically.
    /// </param>
    /// <param name="entropy">
    /// A stable, application-specific, non-secret value used by DPAPI. For example,
    /// <c>Contoso.MyApp.ProtectedStore.v1</c>. It can be hard-coded, but must not change for existing data.
    /// </param>
    public ProtectedStringStore(string filePath, string entropy)
    {
        store = new ProtectedFileStore(filePath, entropy);
    }

    /// <inheritdoc />
    public string? Load() => store.LoadValue(store.StoragePath);

    /// <inheritdoc />
    public void Save(string value) => store.SaveValue(store.StoragePath, value);

    /// <inheritdoc />
    public void Clear() => store.DeleteValue(store.StoragePath);
}
