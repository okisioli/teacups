using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Okisioli.Teacups.Auth.Cookies;

/// <summary>Implements <see cref="IAuthService"/> with ASP.NET Core cookie authentication.</summary>
/// <param name="httpContextAccessor">Provides the current HTTP context.</param>
/// <param name="userLoginAuthenticator">Authenticates supplied user credentials.</param>
public sealed class CookieAuthService(
    IHttpContextAccessor httpContextAccessor,
    IUserLoginAuthenticator userLoginAuthenticator) : IAuthService
{
    /// <inheritdoc />
    public string? UserId => GetCurrentUserId();

    /// <inheritdoc />
    public Task<bool> IsLoggedInAsync()
    {
        return Task.FromResult(httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated is true);
    }

    /// <inheritdoc />
    public async Task<AuthResult> LogInAsync(string email, string password, bool rememberMe = false)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return AuthResult.Failure(AuthErrorCodes.NoSession, "No HTTP session is available.");
        }

        try
        {
            var user = await userLoginAuthenticator.AuthenticateAsync(email, password);
            if (user is null)
            {
                return AuthResult.Failure(AuthErrorCodes.InvalidEmailOrPassword, "Invalid email or password.");
            }

            var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            ],
            CookieAuthenticationDefaults.AuthenticationScheme);

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = rememberMe });

            return AuthResult.Success;
        }
        catch (Exception exception)
        {
            return AuthResult.Failure(AuthErrorCodes.Unexpected, exception.Message, exception);
        }
    }

    /// <inheritdoc />
    public Task<AuthResult> SignUpAsync(string email, string password, bool rememberMe = false)
    {
        return Task.FromResult(AuthResult.Failure(
            AuthErrorCodes.SignUpFailed,
            "This authentication provider does not support signing up."));
    }

    /// <inheritdoc />
    public Task<AuthResult> SendPasswordResetAsync(string email)
    {
        return Task.FromResult(AuthResult.Failure(
            AuthErrorCodes.PasswordResetFailed,
            "This authentication provider does not support password resets."));
    }

    /// <inheritdoc />
    public Task SignOutAsync()
    {
        return httpContextAccessor.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)
            ?? Task.CompletedTask;
    }

    private string? GetCurrentUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        return user?.Identity?.IsAuthenticated is true
            ? user.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;
    }
}
