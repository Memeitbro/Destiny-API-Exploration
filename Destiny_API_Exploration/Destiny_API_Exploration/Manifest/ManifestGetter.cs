using System.Net.Http;
using System.Text.Json;
using Destiny_API_Exploration.Objects;

namespace Destiny_API_Exploration.Manifest;

/// <summary>
/// Fetches the database of item definitions, which apparently changes very often so this is done on every log in.
/// Originally there was to be functions for fetching activity rotations as well, refer to "ShowRotations" doc from MainWindow
/// </summary>
public static class ManifestGetter
{
    public static async Task<Dictionary<string, ItemProperties>> GetItemManifest(HttpClient client)
    {
        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get,
            $"https://www.bungie.net/Platform/Destiny2/Manifest/");
        req.Headers.Add("X-API-Key", "f12e32517f1a4b72aa46e39c42e944a7");
        HttpResponseMessage res = await client.SendAsync(req);
        var responseToManifest =
            JsonSerializer.Deserialize<ManifestResponse>(await res.Content.ReadAsStringAsync());
        var down = new HttpRequestMessage(HttpMethod.Get,
            $"https://www.bungie.net{responseToManifest!.Response.jsonWorldComponentContentPaths.en.DestinyInventoryItemLiteDefinition}");
        down.Headers.Add("X-API-Key", "f12e32517f1a4b72aa46e39c42e944a7");
        HttpResponseMessage json = await client.SendAsync(down);
        var itemDefinitions =
            JsonSerializer.Deserialize<Dictionary<string, ItemProperties>>(await json.Content.ReadAsStringAsync());
        return itemDefinitions!;
    }
}