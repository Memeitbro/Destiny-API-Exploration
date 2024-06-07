namespace Destiny_API_Exploration.Manifest;

/// <summary>
/// custom classes to deserialize httpResponses from the API
/// </summary>
public class ManifestResponse
{
    public required Manifest Response { get; set; }
}

public class Manifest
{
    public required ContentPaths jsonWorldComponentContentPaths { get; set; }
}

public class ContentPaths
{
    public required EnglishPaths en { get; set; }
}

public class EnglishPaths
{
    public required string DestinyInventoryItemLiteDefinition { get; set; } // the only path we need, for now
}

