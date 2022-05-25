namespace Auth.Api.Interfaces.Services;

public interface IAccessTokenService : ITokenService
{
    public bool GetGuidFromAccessToken(string accessToken, out Guid userId);
}