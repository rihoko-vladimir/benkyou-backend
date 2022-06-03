namespace Auth.Api.Interfaces.Services;

public interface ITokenService
{
    public string GetToken(Guid id);
}