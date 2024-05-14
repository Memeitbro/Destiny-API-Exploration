namespace Destiny_API_Exploration.Objects;

public class ResponseToProfileGet
{
    public ProfileData Response { get; set; }
}

public class ProfileData
{
    public ActualProfileData profile { get; set; }
}

public class ActualProfileData
{
    public CharacterIds data { get; set; }
}

public class CharacterIds
{
    public string[] characterIds { get; set; }
}