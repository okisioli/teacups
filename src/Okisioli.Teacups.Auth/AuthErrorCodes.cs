namespace Okisioli.Teacups.Auth;

/// <summary>Defines provider-neutral error codes for authentication operations.</summary>
public static class AuthErrorCodes
{
    /// <summary>An account that is already registered during account creation.</summary>
    public const int EmailAlreadyInUse = 1;
    /// <summary>An authentication attempt blocked because the email address is not confirmed.</summary>
    public const int EmailNotConfirmed = 9;
    /// <summary>A refresh token rejected as expired by the authentication provider.</summary>
    public const int ExpiredRefreshToken = 14;
    /// <summary>A syntactically invalid email address supplied as authentication input, not an unregistered account.</summary>
    public const int InvalidEmail = 4;
    /// <summary>Failed sign-in identity or credential verification.</summary>
    public const int InvalidEmailOrPassword = 3;
    /// <summary>A password rejected by provider validation in a password-based authentication flow.</summary>
    public const int InvalidPassword = 2;
    /// <summary>A refresh token rejected as invalid by the authentication provider.</summary>
    public const int InvalidRefreshToken = 13;
    /// <summary>Required authentication input that was omitted.</summary>
    public const int MissingInformation = 11;
    /// <summary>An operation that requires an active session when none is available.</summary>
    public const int NoSession = 16;
    /// <summary>No authentication error occurred.</summary>
    public const int None = 0;
    /// <summary>An authentication operation that cannot reach its provider because it is offline.</summary>
    public const int Offline = 8;
    /// <summary>An otherwise-unclassified password reset failure.</summary>
    public const int PasswordResetFailed = 6;
    /// <summary>An otherwise-unclassified account creation failure.</summary>
    public const int SignUpFailed = 5;
    /// <summary>An authentication provider rate limit being exceeded.</summary>
    public const int TooManyRequests = 12;
    /// <summary>A fallback for an unknown or otherwise-unclassified authentication failure.</summary>
    public const int Unexpected = 7;

    /// <summary>An operation requiring an administrator token.</summary>
    public const int AdminTokenRequired = 15;
    /// <summary>A malformed or otherwise invalid authentication session URL.</summary>
    public const int BadSessionUrl = 17;
    /// <summary>An unsupported authentication flow type.</summary>
    public const int InvalidFlowType = 18;
    /// <summary>A syntactically invalid phone number supplied as authentication input.</summary>
    public const int InvalidPhone = 10;
    /// <summary>An MFA challenge that has not been verified.</summary>
    public const int MfaChallengeUnverified = 21;
    /// <summary>An SSO configuration referencing an unavailable domain.</summary>
    public const int SsoDomainNotFound = 19;
    /// <summary>An SSO configuration referencing an unavailable provider.</summary>
    public const int SsoProviderNotFound = 20;
}
