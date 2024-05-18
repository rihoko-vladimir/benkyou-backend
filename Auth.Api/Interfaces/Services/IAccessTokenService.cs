namespace Auth.Api.Interfaces.Services;

public interface IAccessTokenService : ITokenService
{
    public string GetRole(string token);

    public Guid GetUserId(string accessToken);
}