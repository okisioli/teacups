using System.Security.Cryptography;
using System.Text;

namespace Okisioli.Teacups.Store.Windows;

/// <summary>
/// Stores named string values in separate files protected with Windows Data Protection API (DPAPI).
/// </summary>
public sealed class WindowsSecureKeyValueStore : ISecureKeyValueStore
{
    private readonly ProtectedFileStore store;
    private readonly string fileNamePrefix;

    /// <summary>
    /// Initializes a Windows secure key/value store.
    /// </summary>
    /// <param name="directoryPath">
    /// The directory in which to store protected values. Prefer a dedicated application-specific directory below local
    /// application data. The directory is created automatically, and <see cref="Clear"/> removes only this store's files.
    /// </param>
    /// <param name="entropy">
    /// A stable, application-specific, non-secret value used by DPAPI. For example,
    /// <c>Contoso.MyApp.SecureStore.v1</c>. It can be hard-coded, but must not change for existing data.
    /// </param>
    public WindowsSecureKeyValueStore(string directoryPath, string entropy)
    {
        store = new ProtectedFileStore(directoryPath, entropy);
        fileNamePrefix = $"protected-key-value-{store.StoreIdentifier}-";
    }

    /// <inheritdoc />
    public string? Get(string key) => store.LoadValue(GetFilePath(key));

    /// <inheritdoc />
    public void Set(string key, string value) => store.SaveValue(GetFilePath(key), value);

    /// <inheritdoc />
    public void Remove(string key) => ProtectedFileStore.DeleteValue(GetFilePath(key));

    /// <inheritdoc />
    public void Clear()
    {
        if (!Directory.Exists(store.StoragePath))
        {
            return;
        }

        foreach (var filePath in Directory.EnumerateFiles(
                     store.StoragePath,
                     $"{fileNamePrefix}*.dat",
                     SearchOption.TopDirectoryOnly))
        {
            File.Delete(filePath);
        }
    }

    private string GetFilePath(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var keyBytes = Encoding.UTF8.GetBytes(key);
        var keyHash = Convert.ToHexString(SHA256.HashData(keyBytes));
        return Path.Combine(store.StoragePath, $"{fileNamePrefix}{keyHash}.dat");
    }
}
