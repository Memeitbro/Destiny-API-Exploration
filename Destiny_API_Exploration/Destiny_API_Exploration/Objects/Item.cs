namespace Destiny_API_Exploration.Objects;

public class Item
{
    public long itemHash { get; set; }
    public string? itemInstanceId { get; set; }
    public int location { get; set; }
    public long bucketHash { get; set; }
    public int transferStatus { get; set; }
    public int quantity { get; set; }
    
    public string currentlyIn { get; set; }


    public override string ToString()
    {
        return itemHash + " : " + itemInstanceId;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        Item it = obj as Item;
        if (itemInstanceId != null)
        {
            if (it.itemInstanceId == null)
            {
                return false;
            }

            return this.itemHash == it.itemHash && itemInstanceId == it.itemInstanceId;
        }

        return itemHash == it.itemHash;
    }
}