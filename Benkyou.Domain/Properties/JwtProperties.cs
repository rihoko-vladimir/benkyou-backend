namespace Benkyou.Domain.Properties;

public record JwtProperties
{
    public string Audience { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string AccessSecret { get; init; } = null!;

    public string RefreshSecret { get; init; } = null!;
    public int AccessTokenExpirationTime { get; init; }
    public int RefreshTokenExpirationTime { get; init; }
}