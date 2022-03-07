using Benkyou.Domain.Models;

namespace Benkyou.Application.Services.Identity;

public interface IUserService
{
    public Task<bool> RegisterAsync(RegisterModel registerModel);
    public Task<TokensResponse> LoginAsync(LoginModel loginModel);

    public Task<TokensResponse> GetNewTokens(Guid userId);
}