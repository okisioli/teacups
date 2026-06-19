using Newtonsoft.Json;
using Olivercode.Dry.Auth.Provider;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace Olivercode.Dry.Auth.Provider.Supabase;

internal sealed class SupabaseSessionHandler(IAuthSessionStore store) : IGotrueSessionPersistence<Session>
{
    private bool _persistenceEnabled = true;

    public void SaveSession(Session session)
    {
        if (_persistenceEnabled)
        {
            store.Save(JsonConvert.SerializeObject(session));
        }
        else
        {
            store.Clear();
        }
    }

    public void DestroySession() => store.Clear();

    public Session? LoadSession()
    {
        if (!_persistenceEnabled)
        {
            return null;
        }

        var serializedSession = store.Load();
        if (string.IsNullOrWhiteSpace(serializedSession))
        {
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<Session>(serializedSession);
        }
        catch (JsonException)
        {
            store.Clear();
            return null;
        }
    }

    public void SetPersistence(bool enabled)
    {
        _persistenceEnabled = enabled;
        if (!enabled)
        {
            store.Clear();
        }
    }
}
