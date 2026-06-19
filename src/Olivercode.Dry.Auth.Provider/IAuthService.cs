namespace Olivercode.Dry.Auth.Provider;

public interface IAuthService
{
    string? UserId { get; }

    Task<bool> IsLoggedInAsync();

    Task<AuthResult> LogInAsync(string email, string password, bool rememberMe = false);

    Task<AuthResult> SignUpAsync(string email, string password, bool rememberMe = false);

    Task<AuthResult> SendPasswordResetAsync(string email);

    Task SignOutAsync();
}
