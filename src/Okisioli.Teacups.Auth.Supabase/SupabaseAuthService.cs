using Okisioli.Teacups.Auth;
using Supabase.Gotrue.Exceptions;

namespace Okisioli.Teacups.Auth.Supabase;

public sealed partial class SupabaseAuthService(
    SupabaseClientProvider clientProvider,
    string? passwordResetRedirectUrl = null) : IExternalAuthService
{
    public string? UserId => clientProvider.Client.Auth.CurrentUser?.Id;

    /// <inheritdoc />
    public string? DisplayName
    {
        get
        {
            var metadata = clientProvider.Client.Auth.CurrentUser?.UserMetadata;
            if (metadata is null) return null;
            foreach (var key in new[] { "given_name", "full_name", "name" })
            {
                if (metadata.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value?.ToString()))
                    return value.ToString();
            }
            return null;
        }
    }

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

    public async Task SignOutAsync()
    {
        await clientProvider.InitializeAsync().ConfigureAwait(false);
        clientProvider.SetSessionPersistence(enabled: false);
        try
        {
            await clientProvider.Client.Auth.SignOut().ConfigureAwait(false);
        }
        finally
        {
            ((global::Supabase.Gotrue.Client)clientProvider.Client.Auth).DestroySession();
        }
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
