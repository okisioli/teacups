namespace Okisioli.Teacups.Auth;

public sealed record AuthResult(
    bool IsSuccess,
    int? ErrorCode = null,
    string? ErrorMessage = null)
{
    public static AuthResult Success { get; } = new(true);

    public static AuthResult Failure(int errorCode, string? errorMessage = null) =>
        new(false, errorCode, errorMessage);
}
