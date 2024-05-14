namespace Destiny_API_Exploration.Objects;

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