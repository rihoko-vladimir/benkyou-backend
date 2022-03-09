using System.Security.Claims;

namespace Benkyou.Application.Common;

public interface ITokenGenerator
{
    public string GenerateToken(string secret, string issuer, string audience,
        int expiresInMinutes, ICollection<Claim>? claims = null);
}