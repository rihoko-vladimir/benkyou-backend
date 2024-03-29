﻿using AutoMapper;
using Benkyou.Application.Services.Common;
using Benkyou.Application.Services.Identity;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Exceptions;
using Benkyou.Domain.Models;
using Benkyou.Domain.Models.Requests;
using Benkyou.Domain.Models.Responses;
using Microsoft.AspNetCore.Identity;

namespace Benkyou.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly IAvatarUploadService _avatarUploadService;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IMapper _mapper;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager, IMapper mapper, IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService, IEmailSenderService emailSenderService,
        IAvatarUploadService avatarUploadService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _emailSenderService = emailSenderService;
        _avatarUploadService = avatarUploadService;
    }

    public async Task<Result<Guid>> RegisterAsync(RegisterModel registerModel)
    {
        var user = _mapper.Map<RegisterModel, User>(registerModel);
        user.Role = Role.User;
        var result = await _userManager.CreateAsync(user, registerModel.Password);
        if (!result.Succeeded)
            return Result.Error<Guid>(new UserRegistrationException("User already exists"));
        var token = await _userManager.GenerateUserTokenAsync(user,
            Domain.Enums.TokenProviders.EmailCodeTokenProviderName, UserManager<User>.ConfirmEmailTokenPurpose);
        await _emailSenderService.SendEmailConfirmationCodeAsync(token, user.Email);
        return Result.Success(user.Id);
    }

    public async Task<Result<TokensResponse>> LoginAsync(LoginModel loginModel)
    {
        var user = await _userManager.FindByEmailAsync(loginModel.Email);
        if (user == null)
            return Result.Error<TokensResponse>(new UserNotFoundException("User not found"));
        if (!await _userManager.CheckPasswordAsync(user, loginModel.Password))
            return Result.Error<TokensResponse>(new LoginException("Incorrect password"));
        if (!await _userManager.IsEmailConfirmedAsync(user))
            return Result.Error<TokensResponse>(new LoginException("Email is not confirmed"));
        var accessToken = _accessTokenService.GetToken(user);
        var refreshToken = _refreshTokenService.GetToken(user);
        user.RefreshToken = refreshToken;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result.Error<TokensResponse>(new LoginException("Something went wrong"));
        return Result.Success(new TokensResponse
        {
            RefreshToken = refreshToken,
            AccessToken = accessToken
        });
    }

    public async Task<Result> ConfirmUserEmailAsync(Guid userId, string emailCode)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var result = await _userManager.VerifyUserTokenAsync(user,
            Domain.Enums.TokenProviders.EmailCodeTokenProviderName,
            UserManager<User>.ConfirmEmailTokenPurpose, emailCode);
        return !result ? Result.Error(new EmailConfirmationCodeException("Email code is incorrect")) : Result.Success();
    }

    public async Task<Result<TokensResponse>> GetNewTokensAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result.Error<TokensResponse>(new RefreshTokenException("Incorrect user id provided"));
        var accessToken = _accessTokenService.GetToken(user);
        var refreshToken = _refreshTokenService.GetToken(user);
        user.RefreshToken = refreshToken;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result.Error<TokensResponse>(new RefreshTokenException("An error occured validating your request"));
        return Result.Success(new TokensResponse
        {
            RefreshToken = refreshToken,
            AccessToken = accessToken
        });
    }

    public Guid GetUserGuidFromAccessToken(string accessToken)
    {
        var userId = _accessTokenService.GetGuidFromAccessToken(accessToken);
        return userId;
    }

    public async Task<Result> IsEmailConfirmedAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result.Error(new UserNotFoundException("User with specified GUID wasn't found"));
        var isConfirmed = user.EmailConfirmed;
        return !isConfirmed
            ? Result.Success()
            : Result.Error(new EmailConfirmationCodeException("Email is already confirmed"));
    }

    public async Task<Result> ResetPasswordAsync(string emailAddress)
    {
        var user = await _userManager.FindByEmailAsync(emailAddress);
        if (user == null) return Result.Error(new UserNotFoundException("User with specified email wasn't found"));
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        Console.WriteLine(resetToken);
        await _emailSenderService.SendEmailResetLinkAsync(user.Email, resetToken, user.FirstName);
        return Result.Success();
    }

    public async Task<Result> SetNewUserForgottenPasswordAsync(string email, string newPassword, string token)
    {
        Console.WriteLine(token);
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Error(new UserNotFoundException("User wasn't found"));
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return !result.Succeeded
            ? Result.Error(new InvalidTokenException("Password change token is expired or incorrect"))
            : Result.Success();
    }

    public async Task<Result<UserResponse>> UpdateUserInfoAsync(Guid userId, UpdateUserInfoRequest updateRequest)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result.Error<UserResponse>(new UserNotFoundException("User wasn't found"));
        if (!string.IsNullOrEmpty(updateRequest.About)) user.About = updateRequest.About;
        if (updateRequest.Birthday != null) user.Birthday = updateRequest.Birthday;

        user.FirstName = updateRequest.FirstName;
        user.LastName = updateRequest.LastName;
        if (!string.IsNullOrEmpty(updateRequest.Avatar))
        {
            if (!string.IsNullOrEmpty(user.AvatarUrl)) await _avatarUploadService.DeleteFileAsync(user.AvatarUrl);
            var imageUrl =
                await _avatarUploadService.UploadFileAsync(
                    new MemoryStream(Convert.FromBase64String(updateRequest.Avatar)));
            Console.WriteLine(imageUrl);
            user.AvatarUrl = imageUrl;
        }

        var result = await _userManager.UpdateAsync(user);
        return !result.Succeeded
            ? Result.Error<UserResponse>(new Exception("Unknown database error"))
            : Result.Success(_mapper.Map<UserResponse>(user));
    }

    public async Task<Result> ChangeVisibilityAsync(Guid userId, bool isVisible)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result.Error(new UserNotFoundException("User wasn't found"));
        user.IsAccountPublic = isVisible;
        var result = await _userManager.UpdateAsync(user);
        return !result.Succeeded ? Result.Error(new Exception("Unknown database error")) : Result.Success();
    }

    public async Task<Result<UserResponse>> GetUserInfoAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user == null
            ? Result.Error<UserResponse>(new UserNotFoundException("User wasn't found"))
            : Result.Success(_mapper.Map<UserResponse>(user));
    }

    public async Task<Result> IsEmailAvailableAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null ? Result.Error(new UserNotFoundException("User wasn't found")) : Result.Success();
    }

    public async Task<Result> IsUserNameAvailableAsync(string nickName)
    {
        var user = await _userManager.FindByNameAsync(nickName);
        return user == null ? Result.Error(new UserNotFoundException("User wasn't found")) : Result.Success();
    }

    public async Task<Result<Guid>> GetUserGuidFromEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null
            ? Result.Error<Guid>(new UserNotFoundException("User wasn't found"))
            : Result.Success(user.Id);
    }

    public async Task<Result<List<UserResponse>>> GetAllUsersAsync()
    {
        return await Task.FromResult(Result.Success(_mapper.Map<List<UserResponse>>(_userManager.Users.ToList())));
    }

    public async Task<Result> RemoveUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result.Error(new UserNotFoundException("Specified user does not exist"));

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded) return Result.Error();
        if (!string.IsNullOrEmpty(user.AvatarUrl)) await _avatarUploadService.DeleteFileAsync(user.AvatarUrl);
        return Result.Success();
    }
}