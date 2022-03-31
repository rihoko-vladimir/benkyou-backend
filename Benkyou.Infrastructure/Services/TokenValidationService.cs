using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Benkyou.Application.Services.Identity;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Enums;
using Benkyou.Domain.Exceptions;
using Benkyou.Domain.Models;
using Benkyou.Domain.Properties;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Benkyou.Infrastructure.Services;

public class TokenValidationService : ITokenValidationService
{
    private readonly JwtProperties _jwtProperties;
    private readonly UserManager<User> _userManager;

    public TokenValidationService(JwtProperties jwtProperties, UserManager<User> userManager)
    {
        _jwtProperties = jwtProperties;
        _userManager = userManager;
    }

    public ClaimsPrincipal GetClaimsPrincipalFromRefreshToken(string refreshToken)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtProperties.RefreshSecret));
        var verificationProperties = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            IssuerSigningKey = signingKey,
            ValidIssuer = _jwtProperties.Issuer,
            ValidAudience = _jwtProperties.Audience
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var claimsPrincipal = tokenHandler.ValidateToken(refreshToken, verificationProperties, out _);
        return claimsPrincipal;
    }

    public async Task<bool> IsRefreshTokenValidAsync(string refreshToken)
    {
        var claimsPrincipal = GetClaimsPrincipalFromRefreshToken(refreshToken);

        var userId = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ApplicationClaimTypes.Uid)?.Value;

        if (userId == null) return false;

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) return false;

        return user.RefreshToken == refreshToken;
    }

    public async Task<Result<Guid>> GetUserIdIfRefreshTokenValidAsync(string refreshToken)
    {
        var isTokenValid = await IsRefreshTokenValidAsync(refreshToken);
        if (!isTokenValid) Result.Error<Guid>(new InvalidTokenException("Refresh token is incorrect"));
        return Result.Success(Guid.Parse(GetClaimsPrincipalFromRefreshToken(refreshToken).Claims
            .First(claim => claim.Type == ApplicationClaimTypes.Uid).Value));
    }
}