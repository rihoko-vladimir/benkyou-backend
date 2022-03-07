using Benkyou.Application.Common;
using Benkyou.Application.Services;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Models;

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
        var token = _tokenGenerator.GenerateToken(_jwtProperties.RefreshSecret, _jwtProperties.Issuer,
            _jwtProperties.Audience, _jwtProperties.RefreshTokenExpirationTime);
        return token;
    }
}