namespace Destiny_API_Exploration.ErrorHandling;


/// <summary>
/// custom classes to deserialize httpResponses from the API
/// </summary>
public class ErrorResponse
{
    public int? Response { get; set; }
    public required string Message { get; set; } 
}