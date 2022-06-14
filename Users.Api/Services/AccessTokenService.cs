using System.IdentityModel.Tokens.Jwt;
using Serilog;
using Shared.Models.Constants;
using Users.Api.Interfaces.Services;

namespace Users.Api.Services;

public class AccessTokenService : IAccessTokenService
{
    public bool GetGuidFromAccessToken(string accessToken, out Guid userId)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var id = token.Claims.First(claim => claim.Type == ClaimTypes.Id).Value;
            var guid = Guid.Parse(id);
            userId = guid;

            return true;
        }
        catch (Exception e)
        {
            Log.Warning("Invalid Access token provided. Exception: {Type}, Message: {Message}", e.GetType().FullName,
                e.Message);
            userId = Guid.Empty;

            return false;
        }
    }
}