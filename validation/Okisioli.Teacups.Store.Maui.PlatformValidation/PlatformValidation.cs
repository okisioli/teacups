using Okisioli.Teacups.Store.Maui;

namespace Okisioli.Teacups.Store.Maui.PlatformValidation;

public static class PlatformValidation
{
    public static async Task ExerciseSecureStorageAsync()
    {
        var store = new MauiSecureKeyValueStore();
        await store.SetAsync("validation", "value");
        _ = await store.GetAsync("validation");
        await store.RemoveAsync("validation");
        await store.ClearAsync();
    }
}
