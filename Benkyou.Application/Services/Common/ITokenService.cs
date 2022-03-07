using Benkyou.Domain.Entities;

namespace Benkyou.Application.Services.Common;

public interface ITokenService
{
    public string GetToken(User user);
}