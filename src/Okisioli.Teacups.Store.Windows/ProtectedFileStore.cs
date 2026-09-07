using System.Security.Cryptography;
using System.Text;

namespace Okisioli.Teacups.Store.Windows;

internal sealed class ProtectedFileStore
{
    private readonly byte[] entropy;

    public ProtectedFileStore(string storagePath, string entropy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(storagePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(entropy);

        StoragePath = storagePath;
        this.entropy = Encoding.UTF8.GetBytes(entropy);
        StoreIdentifier = Convert.ToHexString(SHA256.HashData(this.entropy));
    }

    internal string StoragePath { get; }

    internal string StoreIdentifier { get; }

    internal string? LoadValue(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            var protectedData = File.ReadAllBytes(filePath);
            var data = ProtectedData.Unprotect(protectedData, entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(data);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or CryptographicException)
        {
            DeleteValue(filePath);
            return null;
        }
    }

    internal void SaveValue(string filePath, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var data = Encoding.UTF8.GetBytes(value);
        var protectedData = ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(filePath, protectedData);
    }

    internal static void DeleteValue(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
