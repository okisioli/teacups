using System.Security.Cryptography;

namespace Okisioli.Teacups.Sharing;

/// <summary>Generates random codes designed for manual entry.</summary>
public static class ReadableCode
{
    private const string Alphabet = "23456789ABCDEFGHJKMNPQRSTUVWXYZ";

    /// <summary>
    /// Generates a code from cryptographically random uppercase letters and digits,
    /// excluding 0, 1, I, L, and O.
    /// </summary>
    /// <param name="length">The positive number of characters to generate.</param>
    /// <returns>A code of the requested length.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than one.</exception>
    /// <remarks>Callers that require uniqueness must enforce it when storing the code.</remarks>
    public static string New(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
        return RandomNumberGenerator.GetString(Alphabet, length);
    }
}
