namespace Destiny_API_Exploration.Rotators;

/// <summary>
/// custom objects for deserializing rotators.
/// </summary>
public class RotatorObject
{
    public required RotatorInfo Rotator { get; set; }
}

public class RotatorInfo
{
    public required DateTime First_Reset { get; set; }
    public required int Count { get; set; }
    public required string[] Activities { get; set; }
}