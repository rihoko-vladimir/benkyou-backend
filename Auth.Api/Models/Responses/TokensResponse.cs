namespace Auth.Api.Models.Responses;

public class TokensResponse{
    public string AccessToken { get; }
    public string RefreshToken { get; }

    public TokensResponse(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
};