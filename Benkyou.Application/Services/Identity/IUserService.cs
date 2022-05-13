using Benkyou.Domain.Models;
using Benkyou.Domain.Models.Requests;
using Benkyou.Domain.Models.Responses;

namespace Benkyou.Application.Services.Identity;

public interface IUserService
{
    public Task<Result<Guid>> RegisterAsync(RegisterModel registerModel);
    public Task<Result<TokensResponse>> LoginAsync(LoginModel loginModel);

    public Task<Result> ConfirmUserEmailAsync(Guid userId, string emailCode);

    public Task<Result<TokensResponse>> GetNewTokensAsync(Guid userId);

    public Guid GetUserGuidFromAccessToken(string accessToken);

    public Task<Result> IsEmailConfirmedAsync(Guid userId);

    public Task<Result> ResetPasswordAsync(string emailAddress);

    public Task<Result> SetNewUserForgottenPasswordAsync(string email, string newPassword, string token);

    public Task<Result<UserResponse>> UpdateUserInfoAsync(Guid userId, UpdateUserInfoRequest updateRequest);

    public Task<Result> ChangeVisibilityAsync(Guid userId, bool isVisible);

    public Task<Result<UserResponse>> GetUserInfoAsync(Guid userId);

    public Task<Result> IsEmailAvailableAsync(string email);

    public Task<Result> IsUserNameAvailableAsync(string nickName);

    public Task<Result<Guid>> GetUserGuidFromEmailAsync(string email);

    public Task<Result<List<UserResponse>>> GetAllUsersAsync();

    public Task<Result> RemoveUserAsync(Guid userId);
}