using System.Buffers.Text;
using System.Security.Cryptography;

namespace Okisioli.Teacups.Sharing;

/// <summary>Generates short, URL-safe public identifiers.</summary>
public static class PublicId
{
    /// <summary>
    /// Generates an 11-character, case-sensitive ID from eight cryptographically random bytes.
    /// </summary>
    /// <returns>The ID encoded as unpadded Base64url.</returns>
    /// <remarks>Callers that require uniqueness must enforce it when storing the ID.</remarks>
    public static string New() => Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(8));
}
