using Auth.Api.Models;
using Auth.Api.Models.Requests;
using Auth.Api.Models.Responses;

namespace Auth.Api.Interfaces.Services;

public interface IUserService
{
    public Task<Result<TokensResponse>> LoginAsync(string email, string password);

    public Task<Result<Guid>> RegisterAsync(RegistrationRequest registrationRequest);

    public Task<Result<TokensResponse>> RefreshTokensAsync(Guid userId, string refreshToken);

    public Task<Result<Guid>> ConfirmEmailAsync(Guid userId, string confirmationCode);

    public Task<Result> ResetPasswordAsync(Guid userId);

    public Task<Result> ConfirmPasswordResetAsync(Guid userId, string token, string newPassword, string newPasswordConfirmation);
}