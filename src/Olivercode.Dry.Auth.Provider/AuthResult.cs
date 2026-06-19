namespace Olivercode.Dry.Auth.Provider;

public sealed record AuthResult(bool IsSuccess, string? ErrorMessage = null)
{
    public static AuthResult Success { get; } = new(true);

    public static AuthResult Failure(string errorMessage) => new(false, errorMessage);
}
