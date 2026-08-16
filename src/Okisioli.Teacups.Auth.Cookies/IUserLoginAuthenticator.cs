namespace Okisioli.Teacups.Auth.Cookies;

/// <summary>Authenticates a user's login against the application's account database.</summary>
public interface IUserLoginAuthenticator
{
    /// <summary>Returns the authenticated user, or <see langword="null"/> when the login details are invalid.</summary>
    Task<AuthenticationUser?> AuthenticateAsync(string email, string password);
}
