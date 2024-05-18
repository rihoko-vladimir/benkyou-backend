namespace Shared.Models.Models.Configurations;

public class JwtConfiguration
{
    public const string Key = "JWTConfiguration";
    public string Audience { get; init; }
    public string Issuer { get; init; }
    public string AccessSecret { get; init; }
    public string RefreshSecret { get; init; }
    public string ResetSecret { get; init; }
    public int AccessExpiresIn { get; init; }
    public int RefreshExpiresIn { get; init; }
    public int ResetExpiresIn { get; init; }
}