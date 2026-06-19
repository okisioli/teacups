using Olivercode.Dry.Auth.Provider;

namespace Olivercode.Dry.Auth.Provider.Supabase;

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
                ? AuthResult.Failure("Invalid email or password.")
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
                return AuthResult.Failure("Sign-up did not create a session.");
            }

            var authenticatedSession = await clientProvider.Client.Auth.SignIn(email, password);
            return authenticatedSession?.User is null
                ? AuthResult.Failure("Sign-up did not create a session.")
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
            return AuthResult.Failure("Unable to send the password reset email.");
        }
    }

    public async Task SignOutAsync()
    {
        clientProvider.SetSessionPersistence(enabled: false);
        await clientProvider.Client.Auth.SignOut();
    }

    private static string MapError(string message, bool isSignUp)
    {
        var normalized = message.ToLowerInvariant();

        if (isSignUp && (normalized.Contains("already registered") || normalized.Contains("already exists")))
        {
            return "Email is already in use.";
        }

        if (normalized.Contains("password")
            && (normalized.Contains("short") || normalized.Contains("least") || normalized.Contains("weak")))
        {
            return "Invalid password. Use at least 6 characters.";
        }

        if (normalized.Contains("invalid login") || normalized.Contains("invalid credentials"))
        {
            return "Invalid email or password.";
        }

        if (normalized.Contains("email") && normalized.Contains("invalid"))
        {
            return "Enter a valid email address.";
        }

        return isSignUp ? "Unable to sign up." : "Unable to log in.";
    }
}
