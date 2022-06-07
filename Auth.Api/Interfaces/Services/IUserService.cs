using Auth.Api.Models.Requests;
using Auth.Api.Models.Responses;
using Shared.Models.Models;

namespace Auth.Api.Interfaces.Services;

public interface IUserService
{
    public Task<Result<TokensResponse>> LoginAsync(string email, string password);

    public Task<Result<Guid>> RegisterAsync(RegistrationRequest registrationRequest);

    public Task<Result<TokensResponse>> RefreshTokensAsync(string refreshToken);

    public Task<Result<Guid>> ConfirmEmailAsync(Guid userId, string confirmationCode);

    public Task<Result> ResetPasswordAsync(string email);

    public Task<Result> ConfirmPasswordResetAsync(string email, string token, string newPassword);
}