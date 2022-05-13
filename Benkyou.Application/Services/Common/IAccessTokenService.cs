namespace Benkyou.Application.Services.Common;

public interface IAccessTokenService : ITokenService
{
    public Guid GetGuidFromAccessToken(string accessToken);

    public string GetRoleFromAccessToken(string accessToken);
}