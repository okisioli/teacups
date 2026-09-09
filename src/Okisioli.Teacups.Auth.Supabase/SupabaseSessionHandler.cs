using Newtonsoft.Json;
using Okisioli.Teacups.Store;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace Okisioli.Teacups.Auth.Supabase;

internal sealed class SupabaseSessionHandler(
    ISecureKeyValueStore store,
    string storageKey) : IGotrueSessionPersistence<Session>
{
    private bool persistenceEnabled = true;

    public void SaveSession(Session session)
    {
        if (persistenceEnabled)
        {
            store.Set(storageKey, JsonConvert.SerializeObject(session));
        }
        else
        {
            store.Remove(storageKey);
        }
    }

    public void DestroySession() => store.Remove(storageKey);

    public Session? LoadSession()
    {
        if (!persistenceEnabled)
        {
            return null;
        }

        var serializedSession = store.Get(storageKey);
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
            store.Remove(storageKey);
            return null;
        }
    }

    public void SetPersistence(bool enabled)
    {
        persistenceEnabled = enabled;
        if (!enabled)
        {
            store.Remove(storageKey);
        }
    }
}
