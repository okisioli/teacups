namespace Okisioli.Teacups.Auth.Cookies;

/// <summary>Represents a user authenticated by an application's credential validator.</summary>
public sealed record AuthenticationUser(string Id, string Email);
