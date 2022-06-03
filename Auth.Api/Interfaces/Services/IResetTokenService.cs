namespace Auth.Api.Interfaces.Services;

public interface IResetTokenService : ITokenService
{
    public bool VerifyToken(Guid userId, string resetToken);
}