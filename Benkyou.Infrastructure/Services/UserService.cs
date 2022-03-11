using AutoMapper;
using Benkyou.Application.Services.Common;
using Benkyou.Application.Services.Identity;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Enums;
using Benkyou.Domain.Exceptions;
using Benkyou.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Benkyou.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IMapper _mapper;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager, IMapper mapper, IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService, IEmailSenderService emailSenderService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _emailSenderService = emailSenderService;
    }

    public async Task<Result<Guid>> RegisterAsync(RegisterModel registerModel)
    {
        var user = _mapper.Map<RegisterModel, User>(registerModel);
        user.Role = Roles.Administrator;
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
        var user = await _userManager.FindByNameAsync(loginModel.Login);
        if (user == null)
            return Result.Error<TokensResponse>(new LoginException("User not found"));
        if (!await _userManager.CheckPasswordAsync(user, loginModel.Password))
            return Result.Error<TokensResponse>(new LoginException("Incorrect password"));
        if (!await _userManager.IsEmailConfirmedAsync(user))
            return Result.Error<TokensResponse>(new LoginException("Email is not verified"));
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

    public async Task<Result> ValidateEmailCodeAsync(Guid userId, string emailCode)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var result = await _userManager.VerifyUserTokenAsync(user,
            Domain.Enums.TokenProviders.EmailCodeTokenProviderName,
            UserManager<User>.ConfirmEmailTokenPurpose, emailCode);
        return !result ? Result.Error(new EmailVerificationCodeException("Email code is incorrect")) : Result.Success();
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
        return user == null
            ? Result.Error(new UserNotFoundExceptions("User with specified GUID wasn't found"))
            : Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string emailAddress)
    {
        var user = await _userManager.FindByEmailAsync(emailAddress);
        if (user == null) return Result.Error(new UserNotFoundExceptions("User with specified email wasn't found"));
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailSenderService.SendEmailResetLinkAsync(user.Email, resetToken, user.FirstName);
        return Result.Success();
    }

    public async Task<Result> SetNewUserPasswordAsync(string email, string newPassword, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return Result.Error(new UserNotFoundExceptions("User wasn't found"));
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return !result.Succeeded
            ? Result.Error(new InvalidTokenException("Password change token is expired or incorrect"))
            : Result.Success();
    }
}