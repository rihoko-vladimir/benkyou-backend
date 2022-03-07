using System.Security.Claims;
using Benkyou.Application.Common;
using Benkyou.Application.Services.Common;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Enums;
using Benkyou.Domain.Properties;

namespace Benkyou.Infrastructure.Services;

public class JwtRefreshTokenService : IRefreshTokenService
{
    private readonly JwtProperties _jwtProperties;
    private readonly ITokenGenerator _tokenGenerator;

    public JwtRefreshTokenService(ITokenGenerator tokenGenerator, JwtProperties jwtProperties)
    {
        _tokenGenerator = tokenGenerator;
        _jwtProperties = jwtProperties;
    }

    public string GetToken(User user)
    {
        var userClaims = new List<Claim>
        {
            new(ApplicationClaimTypes.Uid, user.Id.ToString())
        };
        var token = _tokenGenerator.GenerateToken(_jwtProperties.RefreshSecret, _jwtProperties.Issuer,
            _jwtProperties.Audience, _jwtProperties.RefreshTokenExpirationTime, userClaims);
        return token;
    }
}