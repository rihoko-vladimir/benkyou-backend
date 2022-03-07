namespace Benkyou.Domain.Models;

public class TokensResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}