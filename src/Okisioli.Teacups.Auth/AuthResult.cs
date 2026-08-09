namespace Okisioli.Teacups.Auth;

public sealed record AuthResult(
    int ErrorCode,
    string? ProviderErrorMessage = null,
    Exception? ErrorException = null)
{
    public static AuthResult Success { get; } = new(AuthErrorCodes.None);

    public static AuthResult Failure(int errorCode,
        string? providerErrorMessage = null, 
        Exception? errorException = null)
    {
        return errorCode == AuthErrorCodes.None
            ? throw new InvalidOperationException(
                "Cannot create a failure result with errorCode == AuthErrorCodes.None")
            : new AuthResult(errorCode, providerErrorMessage, errorException);
    }

    public bool IsSuccess => ErrorCode != AuthErrorCodes.None;  
}
