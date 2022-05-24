namespace Auth.Api.Interfaces.Services;

public interface IAccessTokenService : ITokenService
{
    public Guid GetGuidFromAccessTokenAsync(string accessToken);
}