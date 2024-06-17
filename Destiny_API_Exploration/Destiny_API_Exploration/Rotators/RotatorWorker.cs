using System.IO;
using System.Text.Json;

namespace Destiny_API_Exploration.Rotators;

public static class RotatorWorker
{
    /// <summary>
    /// Fetches the raid rotation.
    /// Originally not meant to be hard-coded. Refer to "ShowRotations" doc from MainWindow
    /// </summary>
    /// <returns></returns>
    public static async Task<string> RaidRotation()
    {
        var rotator = JsonSerializer.Deserialize<RotatorObject>(await File.ReadAllTextAsync("../../../Rotators/Raids.json"));
        var diff = DateTime.Now - rotator.Rotator.First_Reset;
        int weeks = diff.Days / 7;
        
        return rotator.Rotator.Activities[weeks % rotator.Rotator.Count];
    }

    /// <summary>
    /// Fetches the dungeon rotation.
    /// Originally not meant to be hard-coded. Refer to "ShowRotations" doc from MainWindow
    /// </summary>
    /// <returns></returns>
    public static async Task<string> DungeonRotation()
    {
        var rotator = JsonSerializer.Deserialize<RotatorObject>(await File.ReadAllTextAsync("../../../Rotators/Dungeons.json"));
        var diff = DateTime.Now - rotator.Rotator.First_Reset;
        int weeks = diff.Days / 7;
        
        return rotator.Rotator.Activities[weeks % rotator.Rotator.Count];
    }

}   