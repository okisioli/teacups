namespace Okisioli.Teacups.Auth.Cookies;

/// <summary>Validates credentials against an application's user store.</summary>
public interface ICredentialValidator
{
    /// <summary>Returns the authenticated user, or <see langword="null"/> when the credentials are invalid.</summary>
    Task<AuthenticationUser?> ValidateAsync(string email, string password);
}
