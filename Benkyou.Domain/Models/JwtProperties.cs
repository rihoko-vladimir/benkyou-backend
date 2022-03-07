namespace Benkyou.Domain.Models;

public record JwtProperties
{
    public string Audience { get; init; }
    public string Issuer { get; init; }
    public string AccessSecret { get; init; }

    public string RefreshSecret { get; set; }
    public int AccessTokenExpirationTime { get; init; }
    public int RefreshTokenExpirationTime { get; init; }
}