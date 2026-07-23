using System.IO;
using System.Security.Cryptography;
using System.Text;
using Okisioli.Teacups.Auth.Provider;

namespace Okisioli.Teacups.Auth.Session.Wpf;

public sealed class WpfAuthSessionStore : IAuthSessionStore
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("Quitwit.AuthSession.v1");
    private readonly string _filePath;

    public WpfAuthSessionStore(string? filePath = null)
    {
        _filePath = filePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Quitwit",
            "auth-session.dat");
    }

    public string? Load()
    {
        if (!File.Exists(_filePath))
        {
            return null;
        }

        try
        {
            var protectedData = File.ReadAllBytes(_filePath);
            var data = ProtectedData.Unprotect(protectedData, Entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(data);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or CryptographicException)
        {
            Clear();
            return null;
        }
    }

    public void Save(string session)
    {
        var directory = Path.GetDirectoryName(_filePath)!;
        Directory.CreateDirectory(directory);

        var data = Encoding.UTF8.GetBytes(session);
        var protectedData = ProtectedData.Protect(data, Entropy, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(_filePath, protectedData);
    }

    public void Clear()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }
}
