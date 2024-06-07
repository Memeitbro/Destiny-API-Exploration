namespace Destiny_API_Exploration.Objects;


/// <summary>
/// custom classes to deserialize httpResponses from the API
/// </summary>
public class Auth
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public string refresh_token { get; set; }
    public int refresh_expires_in { get; set; }
    public string membership_id { get; set; }
}