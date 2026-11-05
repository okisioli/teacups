namespace Okisioli.Teacups.Auth;

/// <summary>Starts browser-based sign-in and completes its callback without depending on a UI framework.</summary>
public interface IExternalAuthService : IAuthService
{
    /// <summary>Creates a Google sign-in request. The host opens AuthorizationUri and privately retains the request.</summary>
    Task<ExternalAuthRequest> BeginGoogleSignInAsync(Uri redirectUri, bool rememberMe = false);

    /// <summary>Validates the callback against the original request and exchanges its code for a session.</summary>
    Task<AuthResult> CompleteSignInAsync(ExternalAuthRequest request, Uri callbackUri);
}
