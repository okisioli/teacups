namespace Okisioli.Teacups.Auth;

/// <summary>Represents the outcome of an authentication operation.</summary>
public sealed record AuthResult
{
    private AuthResult(
        int errorCode,
        string? providerErrorMessage = null,
        Exception? errorException = null)
    {
        ErrorCode = errorCode;
        ProviderErrorMessage = providerErrorMessage;
        ErrorException = errorException;
    }

    /// <summary>Gets the shared authentication error code, or <see cref="AuthErrorCodes.None"/> on success.</summary>
    public int ErrorCode { get; }

    /// <summary>Gets the original provider error message when one is available.</summary>
    public string? ProviderErrorMessage { get; }

    /// <summary>Gets the original provider or operation exception when one is available.</summary>
    public Exception? ErrorException { get; }

    /// <summary>Gets the successful authentication result.</summary>
    public static AuthResult Success { get; } = new(AuthErrorCodes.None);

    /// <summary>Creates a failed authentication result with a non-zero error code.</summary>
    public static AuthResult Failure(
        int errorCode,
        string? providerErrorMessage = null,
        Exception? errorException = null) =>
        errorCode == AuthErrorCodes.None
            ? throw new InvalidOperationException(
                "Cannot create a failure result with errorCode == AuthErrorCodes.None")
            : new AuthResult(errorCode, providerErrorMessage, errorException);

    /// <summary>Gets a value indicating whether the authentication operation succeeded.</summary>
    public bool IsSuccess => ErrorCode == AuthErrorCodes.None;
}
