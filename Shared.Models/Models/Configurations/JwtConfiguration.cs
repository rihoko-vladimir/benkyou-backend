namespace Shared.Models.Models.Configurations;

public class JwtConfiguration
{
    public const string Key = "JWTConfiguration";
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public string AccessSecret { get; set; }
    public string RefreshSecret { get; set; }
    public string ResetSecret { get; set; }
    public int AccessExpiresIn { get; set; }
    public int RefreshExpiresIn { get; set; }
    public int ResetExpiresIn { get; set; }
}