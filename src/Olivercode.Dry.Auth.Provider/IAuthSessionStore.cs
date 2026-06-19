namespace Olivercode.Dry.Auth.Provider;

public interface IAuthSessionStore
{
    string? Load();

    void Save(string session);

    void Clear();
}
