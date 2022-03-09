using Benkyou.Domain.Models;

namespace Benkyou.Application.Services.Identity;

public interface IUserService
{
    public Task<Guid> RegisterAsync(RegisterModel registerModel);
    public Task<TokensResponse> LoginAsync(LoginModel loginModel);

    public Task<bool> ValidateEmailCodeAsync(Guid userId, string emailCode);

    public Task<TokensResponse> GetNewTokensAsync(Guid userId);

    public Guid GetUserGuidFromAccessToken(string accessToken);

    public Task<bool> IsEmailConfirmedAsync(Guid userId);

    public Task ResetPasswordAsync(string emailAddress);

    public Task SetNewUserPasswordAsync(string email, string newPassword, string token);
}