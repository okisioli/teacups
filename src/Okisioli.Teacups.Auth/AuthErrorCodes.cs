namespace Okisioli.Teacups.Auth;

public static class AuthErrorCodes
{
    /// <summary>Shared code for an account that is already registered during account creation.</summary>
    public const int EmailAlreadyInUse = 1;
    /// <summary>Shared code for an authentication attempt blocked because the email address is not confirmed.</summary>
    public const int EmailNotConfirmed = 9;
    /// <summary>Shared code for a refresh token rejected as expired by the authentication provider.</summary>
    public const int ExpiredRefreshToken = 14;
    /// <summary>Shared code for a syntactically invalid email address supplied as authentication input, not an unregistered account.</summary>
    public const int InvalidEmail = 4;
    /// <summary>Shared code for failed sign-in identity or credential verification.</summary>
    public const int InvalidEmailOrPassword = 3;
    /// <summary>Shared code for a password rejected by provider validation in a password-based authentication flow.</summary>
    public const int InvalidPassword = 2;
    /// <summary>Shared code for a refresh token rejected as invalid by the authentication provider.</summary>
    public const int InvalidRefreshToken = 13;
    /// <summary>Shared code for required authentication input that was omitted.</summary>
    public const int MissingInformation = 11;
    /// <summary>Shared code for an operation that requires an active session when none is available.</summary>
    public const int NoSession = 16;
    /// <summary>Value indicating that no authentication error occurred.</summary>
    public const int None = 0;
    /// <summary>Shared code for an authentication operation that cannot reach its provider because it is offline.</summary>
    public const int Offline = 8;
    /// <summary>Shared code for an otherwise-unclassified password reset failure.</summary>
    public const int PasswordResetFailed = 6;
    /// <summary>Shared code for an otherwise-unclassified account creation failure.</summary>
    public const int SignUpFailed = 5;
    /// <summary>Shared code for an authentication provider rate limit being exceeded.</summary>
    public const int TooManyRequests = 12;
    /// <summary>Shared fallback code for an unknown or otherwise-unclassified authentication failure.</summary>
    public const int Unexpected = 7;

    /// <summary>Shared code for an operation requiring an administrator token.</summary>
    public const int AdminTokenRequired = 15;
    /// <summary>Shared code for a malformed or otherwise invalid authentication session URL.</summary>
    public const int BadSessionUrl = 17;
    /// <summary>Shared code for an unsupported authentication flow type.</summary>
    public const int InvalidFlowType = 18;
    /// <summary>Shared code for a syntactically invalid phone number supplied as authentication input.</summary>
    public const int InvalidPhone = 10;
    /// <summary>Shared code for an MFA challenge that has not been verified.</summary>
    public const int MfaChallengeUnverified = 21;
    /// <summary>Shared code for an SSO configuration referencing an unavailable domain.</summary>
    public const int SsoDomainNotFound = 19;
    /// <summary>Shared code for an SSO configuration referencing an unavailable provider.</summary>
    public const int SsoProviderNotFound = 20;
}
