namespace Okisioli.Teacups.Auth;

public static class AuthErrorCodes
{
    public const int None = 0;

    public const int EmailAlreadyInUse = 1;

    public const int InvalidPassword = 2;

    public const int InvalidEmailOrPassword = 3;

    public const int InvalidEmail = 4;

    public const int SignUpFailed = 5;

    public const int PasswordResetFailed = 6;

    public const int Unexpected = 7;

    public const int Offline = 8;

    public const int EmailNotConfirmed = 9;

    public const int InvalidPhone = 10;

    public const int MissingInformation = 11;

    public const int TooManyRequests = 12;

    public const int InvalidRefreshToken = 13;

    public const int ExpiredRefreshToken = 14;

    public const int AdminTokenRequired = 15;

    public const int NoSession = 16;

    public const int BadSessionUrl = 17;

    public const int InvalidFlowType = 18;

    public const int SsoDomainNotFound = 19;

    public const int SsoProviderNotFound = 20;

    public const int MfaChallengeUnverified = 21;
}
