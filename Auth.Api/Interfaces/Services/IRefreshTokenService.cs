namespace Auth.Api.Interfaces.Services;

public interface IRefreshTokenService : ITokenService
{
    public bool VerifyToken(Guid userId, string refreshToken);
}