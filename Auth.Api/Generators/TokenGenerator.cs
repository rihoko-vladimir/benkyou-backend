using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Api.Interfaces.Generators;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Api.Generators;

public class TokenGenerator : ITokenGenerator
{
    public string GenerateToken(string secret, string issuer, string audience,
        int expiresInMinutes, ICollection<Claim>? claims = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(issuer, audience, claims, DateTime.Now,
            DateTime.Now.AddMinutes(expiresInMinutes), credentials);
        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}