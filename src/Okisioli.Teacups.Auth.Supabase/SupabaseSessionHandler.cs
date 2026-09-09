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
            store.SetAsync(storageKey, JsonConvert.SerializeObject(session)).GetAwaiter().GetResult();
        }
        else
        {
            store.RemoveAsync(storageKey).GetAwaiter().GetResult();
        }
    }

    public void DestroySession() => store.RemoveAsync(storageKey).GetAwaiter().GetResult();

    public Session? LoadSession()
    {
        if (!persistenceEnabled)
        {
            return null;
        }

        var serializedSession = store.GetAsync(storageKey).GetAwaiter().GetResult();
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
            store.RemoveAsync(storageKey).GetAwaiter().GetResult();
            return null;
        }
    }

    public void SetPersistence(bool enabled)
    {
        persistenceEnabled = enabled;
        if (!enabled)
        {
            store.RemoveAsync(storageKey).GetAwaiter().GetResult();
        }
    }
}
