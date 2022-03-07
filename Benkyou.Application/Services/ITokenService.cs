using Benkyou.Domain.Entities;

namespace Benkyou.Application.Services;

public interface ITokenService
{
    public string GetToken(User user);
}