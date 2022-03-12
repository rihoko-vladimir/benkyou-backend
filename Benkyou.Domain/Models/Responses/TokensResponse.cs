namespace Benkyou.Domain.Models.Responses;

public class TokensResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}