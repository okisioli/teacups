namespace Okisioli.Teacups.Auth;

/// <summary>Provides authentication operations for an application.</summary>
public interface IAuthService
{
    /// <summary>Gets the identifier of the signed-in user, if one is available.</summary>
    string? UserId { get; }

    /// <summary>Determines whether a user is currently signed in.</summary>
    Task<bool> IsLoggedInAsync();

    /// <summary>Signs in with the supplied credentials.</summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="rememberMe">Whether the resulting session should persist.</param>
    Task<AuthResult> LogInAsync(string email, string password, bool rememberMe = false);

    /// <summary>Creates an account with the supplied credentials.</summary>
    /// <param name="email">The new user's email address.</param>
    /// <param name="password">The new user's password.</param>
    /// <param name="rememberMe">Whether the resulting session should persist.</param>
    Task<AuthResult> SignUpAsync(string email, string password, bool rememberMe = false);

    /// <summary>Sends a password-reset request for the supplied email address.</summary>
    /// <param name="email">The email address for the account to reset.</param>
    Task<AuthResult> SendPasswordResetAsync(string email);

    /// <summary>Signs out the current user.</summary>
    Task SignOutAsync();
}
