using Okisioli.Teacups.Auth;
using Supabase.Gotrue.Exceptions;

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
                ? AuthResult.Failure(
                    AuthErrorCodes.InvalidEmailOrPassword,
                    "Invalid email or password.")
                : AuthResult.Success;
        }
        catch (GotrueException exception)
        {
            return CreateFailure(exception);
        }
        catch (Exception exception)
        {
            return AuthResult.Failure(
                AuthErrorCodes.Unexpected,
                "An unexpected authentication error occurred.",
                exception.Message);
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
                return AuthResult.Failure(
                    AuthErrorCodes.SignUpFailed,
                    "Sign-up did not create a session.");
            }

            var authenticatedSession = await clientProvider.Client.Auth.SignIn(email, password);
            return authenticatedSession?.User is null
                ? AuthResult.Failure(
                    AuthErrorCodes.SignUpFailed,
                    "Sign-up did not create a session.")
                : AuthResult.Success;
        }
        catch (GotrueException exception)
        {
            return CreateFailure(exception);
        }
        catch (Exception exception)
        {
            return AuthResult.Failure(
                AuthErrorCodes.Unexpected,
                "An unexpected authentication error occurred.",
                exception.Message);
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
        catch (GotrueException exception)
        {
            return CreateFailure(exception);
        }
        catch (Exception exception)
        {
            return AuthResult.Failure(
                AuthErrorCodes.PasswordResetFailed,
                "Unable to send the password reset email.",
                exception.Message);
        }
    }

    public async Task SignOutAsync()
    {
        clientProvider.SetSessionPersistence(enabled: false);
        await clientProvider.Client.Auth.SignOut();
    }

    private static AuthResult CreateFailure(GotrueException exception)
    {
        var errorCode = MapError(exception.Reason);
        return AuthResult.Failure(
            errorCode,
            GetErrorMessage(errorCode),
            exception.Message);
    }

    private static int MapError(FailureHint.Reason reason) => reason switch
    {
        FailureHint.Reason.Offline => AuthErrorCodes.Offline,
        FailureHint.Reason.UserEmailNotConfirmed => AuthErrorCodes.EmailNotConfirmed,
        FailureHint.Reason.UserBadMultiple => AuthErrorCodes.InvalidEmailOrPassword,
        FailureHint.Reason.UserBadPassword => AuthErrorCodes.InvalidPassword,
        FailureHint.Reason.UserBadLogin => AuthErrorCodes.InvalidEmailOrPassword,
        FailureHint.Reason.UserBadEmailAddress => AuthErrorCodes.InvalidEmail,
        FailureHint.Reason.UserBadPhoneNumber => AuthErrorCodes.InvalidPhone,
        FailureHint.Reason.UserMissingInformation => AuthErrorCodes.MissingInformation,
        FailureHint.Reason.UserAlreadyRegistered => AuthErrorCodes.EmailAlreadyInUse,
        FailureHint.Reason.UserTooManyRequests => AuthErrorCodes.TooManyRequests,
        FailureHint.Reason.InvalidRefreshToken => AuthErrorCodes.InvalidRefreshToken,
        FailureHint.Reason.ExpiredRefreshToken => AuthErrorCodes.ExpiredRefreshToken,
        FailureHint.Reason.AdminTokenRequired => AuthErrorCodes.AdminTokenRequired,
        FailureHint.Reason.NoSessionFound => AuthErrorCodes.NoSession,
        FailureHint.Reason.BadSessionUrl => AuthErrorCodes.BadSessionUrl,
        FailureHint.Reason.InvalidFlowType => AuthErrorCodes.InvalidFlowType,
        FailureHint.Reason.SsoDomainNotFound => AuthErrorCodes.SsoDomainNotFound,
        FailureHint.Reason.SsoProviderNotFound => AuthErrorCodes.SsoProviderNotFound,
        FailureHint.Reason.MfaChallengeUnverified => AuthErrorCodes.MfaChallengeUnverified,
        _ => AuthErrorCodes.Unexpected
    };

    private static string GetErrorMessage(int errorCode) => errorCode switch
    {
        AuthErrorCodes.Offline => "The authentication service is offline.",
        AuthErrorCodes.EmailNotConfirmed => "Confirm your email address before signing in.",
        AuthErrorCodes.InvalidEmailOrPassword => "Invalid email or password.",
        AuthErrorCodes.InvalidPassword => "Invalid password.",
        AuthErrorCodes.InvalidEmail => "Enter a valid email address.",
        AuthErrorCodes.InvalidPhone => "Enter a valid phone number.",
        AuthErrorCodes.MissingInformation => "Provide all required information.",
        AuthErrorCodes.EmailAlreadyInUse => "Email is already in use.",
        AuthErrorCodes.TooManyRequests => "Too many authentication attempts. Try again later.",
        AuthErrorCodes.InvalidRefreshToken => "The refresh token is invalid.",
        AuthErrorCodes.ExpiredRefreshToken => "The refresh token has expired.",
        AuthErrorCodes.AdminTokenRequired => "This operation requires an administrator token.",
        AuthErrorCodes.NoSession => "No valid authentication session was found.",
        AuthErrorCodes.BadSessionUrl => "The authentication session URL is invalid.",
        AuthErrorCodes.InvalidFlowType => "The authentication flow is invalid.",
        AuthErrorCodes.SsoDomainNotFound => "The SSO domain was not found.",
        AuthErrorCodes.SsoProviderNotFound => "The SSO provider was not found.",
        AuthErrorCodes.MfaChallengeUnverified => "The MFA challenge was not verified.",
        _ => "An unexpected authentication error occurred."
    };
}
