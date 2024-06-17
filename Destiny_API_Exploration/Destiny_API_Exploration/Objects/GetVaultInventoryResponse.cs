namespace Destiny_API_Exploration.Objects;

/// <summary>
/// custom classes to deserialize httpResponses from the API
/// </summary>
public class GetVaultInventoryResponse
{
    public required ProfileInventory Response { get; set; }
}

public class ProfileInventory
{
    public required ProfileInventoryData profileInventory { get; set; }
}

public class ProfileInventoryData
{
    public required ItemsArray data { get; set; }
}