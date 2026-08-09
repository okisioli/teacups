using Okisioli.Teacups.Auth;

namespace Okisioli.Teacups.Auth.Supabase;

public sealed class SupabaseAuthService(
    SupabaseClientProvider clientProvider,
    string? passwordResetRedirectUrl = null) : IAuthService
{
    public string? UserId => clientProvider.Client.Auth.CurrentUser?.Id;

    public async Task<bool> IsLoggedInAsync()
    {
        await clientProvider.InitializeAsync();
        return clientProvider.Client.Auth.CurrentUser is not null;
    }

    public async Task<AuthResult> LogInAsync(
        string email,
        string password,
        bool rememberMe = false)
    {
        await clientProvider.InitializeAsync();
        clientProvider.SetSessionPersistence(rememberMe);

        try
        {
            var session = await clientProvider.Client.Auth.SignIn(email, password);
            return session?.User is null
                ? AuthResult.Failure(AuthErrorCodes.InvalidEmailOrPassword)
                : AuthResult.Success;
        }
        catch (Exception exception)
        {
            return AuthResult.Failure(MapError(exception.Message, isSignUp: false));
        }
    }

    public async Task<AuthResult> SignUpAsync(
        string email,
        string password,
        bool rememberMe = false)
    {
        await clientProvider.InitializeAsync();
        clientProvider.SetSessionPersistence(rememberMe);

        try
        {
            var signUpSession = await clientProvider.Client.Auth.SignUp(email, password);
            if (signUpSession?.User is null)
            {
                return AuthResult.Failure(AuthErrorCodes.SignUpFailed);
            }

            var authenticatedSession = await clientProvider.Client.Auth.SignIn(email, password);
            return authenticatedSession?.User is null
                ? AuthResult.Failure(AuthErrorCodes.SignUpFailed)
                : AuthResult.Success;
        }
        catch (Exception exception)
        {
            return AuthResult.Failure(MapError(exception.Message, isSignUp: true));
        }
    }

    public async Task<AuthResult> SendPasswordResetAsync(string email)
    {
        await clientProvider.InitializeAsync();

        try
        {
            if (string.IsNullOrWhiteSpace(passwordResetRedirectUrl))
            {
                await clientProvider.Client.Auth.ResetPasswordForEmail(email);
            }
            else
            {
                await clientProvider.Client.Auth.ResetPasswordForEmail(
                    new global::Supabase.Gotrue.ResetPasswordForEmailOptions(email)
                    {
                        RedirectTo = passwordResetRedirectUrl
                    });
            }

            return AuthResult.Success;
        }
        catch
        {
            return AuthResult.Failure(AuthErrorCodes.PasswordResetFailed);
        }
    }

    public async Task SignOutAsync()
    {
        clientProvider.SetSessionPersistence(enabled: false);
        await clientProvider.Client.Auth.SignOut();
    }

    private static int MapError(string message, bool isSignUp)
    {
        var normalized = message.ToLowerInvariant();

        if (isSignUp && (normalized.Contains("already registered") || normalized.Contains("already exists")))
        {
            return AuthErrorCodes.EmailAlreadyInUse;
        }

        if (normalized.Contains("password")
            && (normalized.Contains("short") || normalized.Contains("least") || normalized.Contains("weak")))
        {
            return AuthErrorCodes.InvalidPassword;
        }

        if (normalized.Contains("invalid login") || normalized.Contains("invalid credentials"))
        {
            return AuthErrorCodes.InvalidEmailOrPassword;
        }

        if (normalized.Contains("email") && normalized.Contains("invalid"))
        {
            return AuthErrorCodes.InvalidEmail;
        }

        return AuthErrorCodes.Unexpected;
    }
}
