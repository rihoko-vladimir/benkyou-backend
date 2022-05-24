using Auth.Api.Models;

namespace Auth.Api.Interfaces.Services;

public interface ITokenService
{
    public string GetToken(User user);
}