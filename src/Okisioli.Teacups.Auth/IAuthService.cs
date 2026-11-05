namespace Okisioli.Teacups.Auth;

public interface IAuthService
{
    string? UserId { get; }

    /// <summary>A friendly name for display only, never an authorization decision.</summary>
    string? DisplayName => null;

    Task<bool> IsLoggedInAsync();

    Task<AuthResult> LogInAsync(string email, string password, bool rememberMe = false);

    Task<AuthResult> SignUpAsync(string email, string password, bool rememberMe = false);

    Task<AuthResult> SendPasswordResetAsync(string email);

    Task SignOutAsync();
}
