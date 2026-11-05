namespace Okisioli.Teacups.Auth;

/// <summary>A short-lived sign-in transaction. Keep it private; it contains a secret PKCE verifier.</summary>
/// <remarks>Web hosts must retain this in protected, browser-bound state across redirects, never in a URL or logs.</remarks>
public sealed class ExternalAuthRequest
{
    /// <summary>Creates or restores a transaction from trusted host storage.</summary>
    public ExternalAuthRequest(Uri authorizationUri, Uri redirectUri, string codeVerifier,
        string state, DateTimeOffset expiresAt, bool rememberMe)
    {
        ArgumentNullException.ThrowIfNull(authorizationUri);
        ArgumentNullException.ThrowIfNull(redirectUri);
        ArgumentException.ThrowIfNullOrWhiteSpace(codeVerifier);
        ArgumentException.ThrowIfNullOrWhiteSpace(state);
        AuthorizationUri = authorizationUri;
        RedirectUri = redirectUri;
        CodeVerifier = codeVerifier;
        State = state;
        ExpiresAt = expiresAt;
        RememberMe = rememberMe;
    }

    /// <summary>The address the host opens in the system browser or redirects the browser to.</summary>
    public Uri AuthorizationUri { get; }
    /// <summary>The application callback address supplied when sign-in started.</summary>
    public Uri RedirectUri { get; }
    /// <summary>The secret proof used only when completing sign-in.</summary>
    public string CodeVerifier { get; }
    /// <summary>The random correlation value required in the callback.</summary>
    public string State { get; }
    /// <summary>The deadline for completing this transaction.</summary>
    public DateTimeOffset ExpiresAt { get; }
    /// <summary>Whether the resulting session should survive an application restart.</summary>
    public bool RememberMe { get; }
}
