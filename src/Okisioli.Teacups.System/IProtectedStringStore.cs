namespace Okisioli.Teacups.System;

public interface IProtectedStringStore
{
    string? Load();

    void Save(string value);

    void Clear();
}
