using Okisioli.Teacups.Auth;
using Supabase.Gotrue.Exceptions;

namespace Okisioli.Teacups.Auth.Supabase;

/// <summary>Implements <see cref="IAuthService"/> using Supabase authentication.</summary>
/// <param name="clientProvider">Provides the configured Supabase client.</param>
/// <param name="passwordResetRedirectUrl">Optional URL to which password-reset emails redirect.</param>
public sealed class SupabaseAuthService(
    SupabaseClientProvider clientProvider,
    string? passwordResetRedirectUrl = null) : IAuthService
{
    /// <inheritdoc />
    public string? UserId => clientProvider.Client.Auth.CurrentUser?.Id;

    /// <inheritdoc />
    public async Task<bool> IsLoggedInAsync()
    {
        await clientProvider.InitializeAsync();
        return clientProvider.Client.Auth.CurrentUser is not null;
    }

    /// <inheritdoc />
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
        catch (GotrueException exception)
        {
            return CreateFailure(exception);
        }
        catch (Exception exception)
        {
            return AuthResult.Failure(
                AuthErrorCodes.Unexpected,
                exception.Message,
                exception);
        }
    }

    /// <inheritdoc />
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
        catch (GotrueException exception)
        {
            return CreateFailure(exception);
        }
        catch (Exception exception)
        {
            return AuthResult.Failure(
                AuthErrorCodes.Unexpected,
                exception.Message,
                exception);
        }
    }

    /// <inheritdoc />
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
                exception.Message,
                exception);
        }
    }

    /// <inheritdoc />
    public async Task SignOutAsync()
    {
        clientProvider.SetSessionPersistence(enabled: false);
        await clientProvider.Client.Auth.SignOut();
    }

    private static AuthResult CreateFailure(GotrueException exception)
    {
        var errorCode = MapError(exception.Reason);
        return AuthResult.Failure(errorCode, exception.Message, exception);
    }

    private static int MapError(FailureHint.Reason reason) => reason switch
    {
        FailureHint.Reason.UserAlreadyRegistered => AuthErrorCodes.EmailAlreadyInUse,
        FailureHint.Reason.UserEmailNotConfirmed => AuthErrorCodes.EmailNotConfirmed,
        FailureHint.Reason.ExpiredRefreshToken => AuthErrorCodes.ExpiredRefreshToken,
        FailureHint.Reason.UserBadEmailAddress => AuthErrorCodes.InvalidEmail,
        FailureHint.Reason.UserBadLogin => AuthErrorCodes.InvalidEmailOrPassword,
        FailureHint.Reason.UserBadMultiple => AuthErrorCodes.InvalidEmailOrPassword,
        FailureHint.Reason.UserBadPassword => AuthErrorCodes.InvalidPassword,
        FailureHint.Reason.InvalidRefreshToken => AuthErrorCodes.InvalidRefreshToken,
        FailureHint.Reason.UserMissingInformation => AuthErrorCodes.MissingInformation,
        FailureHint.Reason.NoSessionFound => AuthErrorCodes.NoSession,
        FailureHint.Reason.Offline => AuthErrorCodes.Offline,
        FailureHint.Reason.UserTooManyRequests => AuthErrorCodes.TooManyRequests,
        FailureHint.Reason.Unknown => AuthErrorCodes.Unexpected,

        FailureHint.Reason.AdminTokenRequired => AuthErrorCodes.AdminTokenRequired,
        FailureHint.Reason.BadSessionUrl => AuthErrorCodes.BadSessionUrl,
        FailureHint.Reason.InvalidFlowType => AuthErrorCodes.InvalidFlowType,
        FailureHint.Reason.UserBadPhoneNumber => AuthErrorCodes.InvalidPhone,
        FailureHint.Reason.MfaChallengeUnverified => AuthErrorCodes.MfaChallengeUnverified,
        FailureHint.Reason.SsoDomainNotFound => AuthErrorCodes.SsoDomainNotFound,
        FailureHint.Reason.SsoProviderNotFound => AuthErrorCodes.SsoProviderNotFound,
        _ => AuthErrorCodes.Unexpected
    };

}
