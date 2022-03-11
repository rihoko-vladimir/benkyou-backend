using Benkyou.Domain.Models;

namespace Benkyou.Application.Services.Identity;

public interface IUserService
{
    public Task<Result<Guid>> RegisterAsync(RegisterModel registerModel);
    public Task<Result<TokensResponse>> LoginAsync(LoginModel loginModel);

    public Task<Result> ValidateEmailCodeAsync(Guid userId, string emailCode);

    public Task<Result<TokensResponse>> GetNewTokensAsync(Guid userId);

    public Guid GetUserGuidFromAccessToken(string accessToken);

    public Task<Result> IsEmailConfirmedAsync(Guid userId);

    public Task<Result> ResetPasswordAsync(string emailAddress);

    public Task<Result> SetNewUserPasswordAsync(string email, string newPassword, string token);
}