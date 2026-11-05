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

    // GoTrue requires synchronous hooks. Keep async stores outside the host UI synchronization context.
    public void SaveSession(Session session)
    {
        if (persistenceEnabled)
        {
            Task.Run(() => store.SetAsync(storageKey, JsonConvert.SerializeObject(session))).GetAwaiter().GetResult();
        }
        else
        {
            Task.Run(() => store.RemoveAsync(storageKey)).GetAwaiter().GetResult();
        }
    }

    public void DestroySession() => Task.Run(() => store.RemoveAsync(storageKey)).GetAwaiter().GetResult();

    public Session? LoadSession()
    {
        if (!persistenceEnabled)
        {
            return null;
        }

        var serializedSession = Task.Run(() => store.GetAsync(storageKey)).GetAwaiter().GetResult();
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
            Task.Run(() => store.RemoveAsync(storageKey)).GetAwaiter().GetResult();
            return null;
        }
    }

    public void SetPersistence(bool enabled)
    {
        persistenceEnabled = enabled;
        if (!enabled)
        {
            Task.Run(() => store.RemoveAsync(storageKey)).GetAwaiter().GetResult();
        }
    }
}
