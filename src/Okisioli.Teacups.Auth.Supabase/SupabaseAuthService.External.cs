using System.Security.Cryptography;
using System.Text;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using static Supabase.Gotrue.Constants;

namespace Okisioli.Teacups.Auth.Supabase;

public sealed partial class SupabaseAuthService
{
    private const string StateParameter = "teacups_state";

    /// <inheritdoc />
    public async Task<ExternalAuthRequest> BeginGoogleSignInAsync(Uri redirectUri, bool rememberMe = false)
    {
        ArgumentNullException.ThrowIfNull(redirectUri);
        if (!redirectUri.IsAbsoluteUri || redirectUri.IsFile || redirectUri.UserInfo.Length != 0
            || redirectUri.Fragment.Length != 0 || string.IsNullOrWhiteSpace(redirectUri.Host)
            || redirectUri.Scheme is "javascript" or "data"
            || redirectUri.Scheme == Uri.UriSchemeHttp && !redirectUri.IsLoopback)
        {
            throw new ArgumentException("Use an HTTPS, loopback HTTP, or application deep-link callback.", nameof(redirectUri));
        }

        var query = ReadQuery(redirectUri);
        if (query is null || query.ContainsKey(StateParameter) || query.ContainsKey("code") || query.ContainsKey("error"))
        {
            throw new ArgumentException("The callback contains duplicate or reserved query parameters.", nameof(redirectUri));
        }

        await clientProvider.InitializeAsync().ConfigureAwait(false);
        var state = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var callback = new UriBuilder(redirectUri)
        {
            Query = string.Join("&", query.Select(pair =>
                $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}")
                .Append($"{StateParameter}={state}"))
        };

        // Generating a URL through the API URL builder preserves an existing session if the user cancels.
        var api = new Api(clientProvider.Client.Auth.Options.Url, clientProvider.Client.Auth.Options.Headers);
        var provider = api.GetUriForProvider(Provider.Google,
            new SignInOptions { FlowType = OAuthFlowType.PKCE, RedirectTo = callback.Uri.AbsoluteUri });
        if (string.IsNullOrWhiteSpace(provider.PKCEVerifier))
        {
            throw new InvalidOperationException("Supabase did not generate a PKCE verifier.");
        }

        return new ExternalAuthRequest(provider.Uri, redirectUri, provider.PKCEVerifier,
            state, DateTimeOffset.UtcNow.AddMinutes(5), rememberMe);
    }

    /// <inheritdoc />
    public async Task<AuthResult> CompleteSignInAsync(ExternalAuthRequest request, Uri callbackUri)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(callbackUri);
        if (request.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            return AuthResult.Failure(AuthErrorCodes.AuthRequestExpired);
        }

        var expected = request.RedirectUri;
        if (!callbackUri.IsAbsoluteUri || !expected.IsAbsoluteUri
            || !string.Equals(expected.Scheme, callbackUri.Scheme, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(expected.IdnHost, callbackUri.IdnHost, StringComparison.OrdinalIgnoreCase)
            || expected.Port != callbackUri.Port || expected.AbsolutePath != callbackUri.AbsolutePath
            || callbackUri.UserInfo.Length != 0 || callbackUri.Fragment.Length != 0)
        {
            return AuthResult.Failure(AuthErrorCodes.InvalidAuthCallback);
        }

        var query = ReadQuery(callbackUri);
        var expectedQuery = ReadQuery(expected);
        if (query is null || expectedQuery is null
            || !query.TryGetValue(StateParameter, out var state)
            || !CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(request.State), Encoding.UTF8.GetBytes(state))
            || expectedQuery.Any(pair => !query.TryGetValue(pair.Key, out var value) || value != pair.Value))
        {
            return AuthResult.Failure(AuthErrorCodes.InvalidAuthCallback);
        }

        if (query.ContainsKey("error"))
        {
            return AuthResult.Failure(AuthErrorCodes.ExternalSignInFailed);
        }

        if (!query.TryGetValue("code", out var code) || string.IsNullOrWhiteSpace(code))
        {
            return AuthResult.Failure(AuthErrorCodes.InvalidAuthCallback);
        }

        try
        {
            await clientProvider.InitializeAsync().ConfigureAwait(false);
            clientProvider.SetSessionPersistence(request.RememberMe);
            var session = await clientProvider.Client.Auth.ExchangeCodeForSession(request.CodeVerifier, code)
                .ConfigureAwait(false);
            return session?.User is null
                ? AuthResult.Failure(AuthErrorCodes.ExternalSignInFailed)
                : AuthResult.Success;
        }
        catch (GotrueException exception)
        {
            return CreateFailure(exception);
        }
        catch (Exception exception)
        {
            return AuthResult.Failure(AuthErrorCodes.Unexpected, exception.Message, exception);
        }
    }

    private static Dictionary<string, string>? ReadQuery(Uri uri)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var part in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var pair = part.Split('=', 2);
            var key = Uri.UnescapeDataString(pair[0].Replace('+', ' '));
            var value = pair.Length == 2 ? Uri.UnescapeDataString(pair[1].Replace('+', ' ')) : "";
            if (!result.TryAdd(key, value)) return null;
        }
        return result;
    }
}
