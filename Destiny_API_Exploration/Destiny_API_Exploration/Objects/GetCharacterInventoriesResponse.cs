namespace Destiny_API_Exploration.Objects;

/// <summary>
/// custom classes to deserialize httpResponses from the API
/// </summary>
public class GetCharacterInventoriesResponse
{
    public required CharacterInventories Response { get; set; }
}

public class CharacterInventoriesData
{
    public required Dictionary<string, ItemsArray> data { get; set; }
}

public class CharacterInventories
{
    public required CharacterInventoriesData characterInventories { get; set; }
}
