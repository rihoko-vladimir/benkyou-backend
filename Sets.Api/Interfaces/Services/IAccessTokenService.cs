namespace Sets.Api.Interfaces.Services;

public interface IAccessTokenService
{
    public bool GetGuidFromAccessToken(string accessToken, out Guid userId);
}