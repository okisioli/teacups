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
}
