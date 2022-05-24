using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Api.Extensions.ConfigurationExtensions;
using Auth.Api.Interfaces.Generators;
using Auth.Api.Interfaces.Services;
using Auth.Api.Models.Application;
using Auth.Api.Models.Constants;
using ClaimTypes = Auth.Api.Models.Constants.ClaimTypes;

namespace Auth.Api.Services;

public class AccessTokenService : IAccessTokenService
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ITokenGenerator _tokenGenerator;

    public AccessTokenService(ITokenGenerator tokenGenerator, IConfiguration configuration)
    {
        _tokenGenerator = tokenGenerator;
        _jwtConfiguration = configuration.GetJwtConfiguration();
    }

    public string GetToken(Guid id)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Id, id.ToString()),
            new(ClaimTypes.Role, Role.User)
        };
        var token = _tokenGenerator.GenerateToken(_jwtConfiguration.AccessSecret,
            _jwtConfiguration.Issuer,
            _jwtConfiguration.Audience,
            _jwtConfiguration.AccessExpiresIn,
            claims);
        return token;
    }

    public Guid GetGuidFromAccessTokenAsync(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);
        var userId = token.Claims.First(claim => claim.Type == ClaimTypes.Id).Value;
        return Guid.Parse(userId);
    }
}