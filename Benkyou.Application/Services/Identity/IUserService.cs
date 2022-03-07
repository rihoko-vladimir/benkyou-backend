using Benkyou.Domain.Models;

namespace Benkyou.Application.Services.Identity;

public interface IUserService
{
    public Task<bool> Register(RegisterModel registerModel);
    public TokensResponse Login(LoginModel loginModel);
}