namespace Destiny_API_Exploration.Objects;


/// <summary>
/// custom classes to deserialize httpResponses from the API
/// </summary>
public class ResponseToBungieAccount
{
    public DMemberShips Response { get; set; }
}

public class DMemberShips
{
    public DestinyMemberShip[] destinyMemberships { get; set; }
}

public class DestinyMemberShip
{
    public int membershipType { get; set; }
    public string membershipId { get; set; }
    public string displayName { get; set; }
    public string bungieGlobalDisplayName { get; set; }
    public int bungieGlobalDisplayNameCode { get; set; }
}