using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Api.Interfaces.Generators;
using Auth.Api.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Shared.Models.Models.Configurations;
using ClaimTypes = Shared.Models.Constants.ClaimTypes;

namespace Auth.Api.Services;

public class ResetTokenService : IResetTokenService
{
    private readonly JwtConfiguration _jwtConfiguration;

    private readonly ITokenGenerator _tokenGenerator;

    public ResetTokenService(ITokenGenerator tokenGenerator, JwtConfiguration configuration)
    {
        _tokenGenerator = tokenGenerator;
        _jwtConfiguration = configuration;
    }

    public string GetToken(Guid id)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Id, id.ToString())
        };

        var token = _tokenGenerator.GenerateToken(_jwtConfiguration.ResetSecret,
            _jwtConfiguration.Issuer,
            _jwtConfiguration.Audience,
            _jwtConfiguration.ResetExpiresIn,
            claims);

        Log.Information("Generated Password reset token {Token} for User {UserId}", token, id);

        return token;
    }

    public bool VerifyToken(Guid userId, string resetToken)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.ResetSecret));
        var verificationProperties = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            IssuerSigningKey = signingKey,
            ValidIssuer = _jwtConfiguration.Issuer,
            ValidAudience = _jwtConfiguration.Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(resetToken, verificationProperties, out _);
            var id = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Id)?.Value;

            return id == userId.ToString();
        }
        catch (Exception)
        {
            Log.Warning("Verification failed: Password reset token is invalid. User: {UserId}, Token: {Token}", userId,
                resetToken);

            return false;
        }
    }
}