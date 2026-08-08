using System.IO;
using System.Security.Cryptography;
using System.Text;
using Okisioli.Teacups.System;

namespace Okisioli.Teacups.System.Windows;

/// <summary>
/// Stores a single string value protected with Windows Data Protection API (DPAPI).
/// </summary>
public sealed class WindowsProtectedStringStore : IProtectedStringStore
{
    private readonly byte[] entropy;
    private readonly string filePath;

    public WindowsProtectedStringStore(string filePath, string entropy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(entropy);

        this.filePath = filePath;
        this.entropy = Encoding.UTF8.GetBytes(entropy);
    }

    /// <inheritdoc />
    public string? Load()
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
            Clear();
            return null;
        }
    }

    /// <inheritdoc />
    public void Save(string value)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var data = Encoding.UTF8.GetBytes(value);
        var protectedData = ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(filePath, protectedData);
    }

    /// <inheritdoc />
    public void Clear()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
