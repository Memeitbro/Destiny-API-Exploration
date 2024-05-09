namespace Destiny_API_Exploration.Objects;

public class Item
{
    public long itemHash { get; set; }
    public string? itemInstanceId { get; set; }
    public int location { get; set; }
    public long bucketHash { get; set; }
    public int transferStatus { get; set; }
}