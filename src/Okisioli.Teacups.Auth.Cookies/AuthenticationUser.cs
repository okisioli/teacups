namespace Okisioli.Teacups.Auth.Cookies;

/// <summary>Represents a user authenticated by an application's credential validator.</summary>
/// <param name="Id">The application's stable user identifier.</param>
/// <param name="Email">The user's email address.</param>
public sealed record AuthenticationUser(string Id, string Email);
