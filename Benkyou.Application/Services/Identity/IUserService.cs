﻿using Benkyou.Domain.Models;
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

    public Task<Result<UserResponse>> UpdateUserInfo(Guid userId, UpdateUserInfoRequest updateRequest);

    public Task<Result<UserResponse>> GetUserInfo(Guid userId);

    public Task<Result> IsEmailAvailable(string email);

    public Task<Result> IsUserNameAvailable(string nickName);

    public Task<Result<Guid>> GetUserGuidFromEmail(string email);
}