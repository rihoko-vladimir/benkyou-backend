using Auth.Api.Models.Requests;
using Auth.Api.Models.Responses;
using Fido2NetLib;
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
    
    Task<Result<CredentialCreateOptions>> CreateCredentialOptionsAsync(Guid userId);

    Task<Result<AssertionOptions>> GetAssertionOptionsAsync(Guid userId);
    
    Task<Result<Fido2.CredentialMakeResult>> CreatePasskeyAsync(Guid userId, AuthenticatorAttestationRawResponse attestationResponse, CredentialCreateOptions options);
    Task<Result<TokensResponse>> LoginPasskeyAsync(Guid userId, AuthenticatorAssertionRawResponse clientResponse, AssertionOptions options);
}