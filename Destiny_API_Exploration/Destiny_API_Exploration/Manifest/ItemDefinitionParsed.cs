namespace Destiny_API_Exploration.Manifest;


/// <summary>
/// custom classes to deserialize httpResponses from the API
/// </summary>
public class ItemProperties
{
    public required DisplayProperties displayProperties { get; set; }
}

public class DisplayProperties
{
    public required string name { get; set; }
}