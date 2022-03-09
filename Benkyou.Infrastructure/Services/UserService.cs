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

    public async Task<Guid> RegisterAsync(RegisterModel registerModel)
    {
        var user = _mapper.Map<RegisterModel, User>(registerModel);
        user.Role = Roles.Administrator;
        var result = await _userManager.CreateAsync(user, registerModel.Password);
        if (!result.Succeeded)
            throw new UserRegistrationException("User already exists");
        var token = await _userManager.GenerateUserTokenAsync(user,
            Domain.Enums.TokenProviders.EmailCodeTokenProviderName, UserManager<User>.ConfirmEmailTokenPurpose);
        await _emailSenderService.SendEmailConfirmationCodeAsync(token, user.Email);
        return user.Id;
    }

    public async Task<TokensResponse> LoginAsync(LoginModel loginModel)
    {
        var user = await _userManager.FindByNameAsync(loginModel.Login);
        if (user == null)
            throw new LoginException("User not found");
        if (!await _userManager.CheckPasswordAsync(user, loginModel.Password))
            throw new LoginException("Incorrect password");
        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new LoginException("Email is not verified");
        var accessToken = _accessTokenService.GetToken(user);
        var refreshToken = _refreshTokenService.GetToken(user);
        user.RefreshToken = refreshToken;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new LoginException("Something went wrong");
        return new TokensResponse
        {
            RefreshToken = refreshToken,
            AccessToken = accessToken
        };
    }

    public async Task<bool> ValidateEmailCodeAsync(Guid userId, string emailCode)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var result = await _userManager.VerifyUserTokenAsync(user,
            Domain.Enums.TokenProviders.EmailCodeTokenProviderName,
            UserManager<User>.ConfirmEmailTokenPurpose, emailCode);
        return !result ? throw new EmailVerificationCodeException("Email code is incorrect"): true;
    }

    public async Task<TokensResponse> GetNewTokensAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new RefreshTokenException("Incorrect user id provided");
        var accessToken = _accessTokenService.GetToken(user);
        var refreshToken = _refreshTokenService.GetToken(user);
        user.RefreshToken = refreshToken;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) throw new RefreshTokenException("An error occured validating your request");
        return new TokensResponse
        {
            RefreshToken = refreshToken,
            AccessToken = accessToken
        };
    }

    public Guid GetUserGuidFromAccessToken(string accessToken)
    {
        var userId = _accessTokenService.GetGuidFromAccessToken(accessToken);
        return userId;
    }

    public async Task<bool> IsEmailConfirmedAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) throw new UserNotFoundExceptions("User with specified GUID wasn't found");
        return user.EmailConfirmed;
    }

    public async Task ResetPasswordAsync(string emailAddress)
    {
        var user = await _userManager.FindByEmailAsync(emailAddress);
        if (user == null) throw new UserNotFoundExceptions("User with specified email wasn't found");
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailSenderService.SendEmailResetLinkAsync(user.Email, resetToken);
    }

    public async Task SetNewUserPasswordAsync(string email, string newPassword, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new UserNotFoundExceptions("User wasn't found");
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded) throw new InvalidTokenException("Password change token is expired or incorrect");
    }
}