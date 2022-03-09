using System.Security.Claims;

namespace Benkyou.Application.Services.Identity;

public interface ITokenValidationService
{
    public ClaimsPrincipal GetClaimsPrincipalFromRefreshToken(string refreshToken);

    public Task<bool> IsRefreshTokenValidAsync(string refreshToken);

    public Task<Guid> GetUserIdIfRefreshTokenValidAsync(string refreshToken);
}