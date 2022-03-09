using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Benkyou.Application.Common;
using Benkyou.Application.Services.Common;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Enums;
using Benkyou.Domain.Properties;

namespace Benkyou.Infrastructure.Services;

public class JwtAccessTokenService : IAccessTokenService
{
    private readonly JwtProperties _jwtProperties;
    private readonly ITokenGenerator _tokenGenerator;

    public JwtAccessTokenService(ITokenGenerator tokenGenerator, JwtProperties jwtProperties)
    {
        _tokenGenerator = tokenGenerator;
        _jwtProperties = jwtProperties;
    }

    public string GetToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ApplicationClaimTypes.Role, user.Role),
            new(ApplicationClaimTypes.Name, user.FirstName),
            new(ApplicationClaimTypes.Email, user.Email),
            new(ApplicationClaimTypes.Uid, user.Id.ToString())
        };
        var token = _tokenGenerator.GenerateToken(_jwtProperties.AccessSecret, _jwtProperties.Issuer,
            _jwtProperties.Audience, _jwtProperties.AccessTokenExpirationTime, claims);
        return token;
    }

    public Guid GetGuidFromAccessToken(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);
        var userId = token.Claims.First(claim => claim.Type == ApplicationClaimTypes.Uid).Value;
        return Guid.Parse(userId);
    }
}