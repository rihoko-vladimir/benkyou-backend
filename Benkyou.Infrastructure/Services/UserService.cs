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
    private readonly IMapper _mapper;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager, IMapper mapper, IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _mapper = mapper;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Guid> RegisterAsync(RegisterModel registerModel)
    {
        var user = _mapper.Map<RegisterModel, User>(registerModel);
        user.Role = Roles.Administrator;
        var result = await _userManager.CreateAsync(user, registerModel.Password);
        if (!result.Succeeded) throw new UserRegistrationException("User already exists or there were error while creating him");
        var token = await _userManager.GenerateUserTokenAsync(user, Domain.Enums.TokenProviders.EmailCodeTokenProviderName, UserManager<User>.ConfirmEmailTokenPurpose);
        Console.WriteLine(token);
        return user.Id;
    }

    public async Task<TokensResponse> LoginAsync(LoginModel loginModel)
    {
        var user = await _userManager.FindByNameAsync(loginModel.Login);
        if (user == null)
            return new TokensResponse
            {
                AccessToken = string.Empty,
                RefreshToken = string.Empty
            };
        if (!await _userManager.CheckPasswordAsync(user, loginModel.Password))
            return new TokensResponse
            {
                AccessToken = string.Empty,
                RefreshToken = string.Empty
            };
        var accessToken = _accessTokenService.GetToken(user);
        var refreshToken = _refreshTokenService.GetToken(user);
        user.RefreshToken = refreshToken;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return new TokensResponse
            {
                RefreshToken = string.Empty,
                AccessToken = string.Empty
            };
        return new TokensResponse
        {
            RefreshToken = refreshToken,
            AccessToken = accessToken
        };
    }

    public async Task<bool> ValidateEmailCodeAsync(Guid userId, string emailCode)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var result = await _userManager.VerifyUserTokenAsync(user, Domain.Enums.TokenProviders.EmailCodeTokenProviderName,
            UserManager<User>.ConfirmEmailTokenPurpose, emailCode);
        return result;
    }

    public async Task<TokensResponse> GetNewTokens(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var accessToken = _accessTokenService.GetToken(user);
        var refreshToken = _refreshTokenService.GetToken(user);
        user.RefreshToken = refreshToken;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return new TokensResponse
            {
                AccessToken = string.Empty,
                RefreshToken = string.Empty
            };
        return new TokensResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}