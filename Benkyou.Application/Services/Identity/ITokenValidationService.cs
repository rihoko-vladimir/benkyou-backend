using System.Security.Claims;
using Benkyou.Domain.Models;

namespace Benkyou.Application.Services.Identity;

public interface ITokenValidationService
{
    public ClaimsPrincipal GetClaimsPrincipalFromRefreshToken(string refreshToken);

    public Task<bool> IsRefreshTokenValidAsync(string refreshToken);

    public Task<Result<Guid>> GetUserIdIfRefreshTokenValidAsync(string refreshToken);
}