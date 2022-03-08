using Benkyou.Domain.Entities;
using Benkyou.Domain.Models;

namespace Benkyou.Application.Services.Identity;

public interface IUserService
{
    public Task<Guid> RegisterAsync(RegisterModel registerModel);
    public Task<TokensResponse> LoginAsync(LoginModel loginModel);

    public Task<bool> ValidateEmailCodeAsync(Guid userId, string emailCode);
    
    public Task<TokensResponse> GetNewTokens(Guid userId);
}