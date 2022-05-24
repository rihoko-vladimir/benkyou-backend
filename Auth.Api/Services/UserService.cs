using Auth.Api.Interfaces.Generators;
using Auth.Api.Interfaces.Services;
using Auth.Api.Models;
using Auth.Api.Models.DbContext;
using Auth.Api.Models.Entities;
using Auth.Api.Models.Requests;
using Auth.Api.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Bcrypt = BCrypt.Net.BCrypt;

namespace Auth.Api.Services;

public class UserService : IUserService
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly ApplicationContext _applicationContext;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IEmailCodeGenerator _emailCodeGenerator;

    public UserService(ApplicationContext applicationContext, IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService, IEmailCodeGenerator emailCodeGenerator)
    {
        _applicationContext = applicationContext;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _emailCodeGenerator = emailCodeGenerator;
    }

    public async Task<Result<TokensResponse>> LoginAsync(string email, string password)
    {
        var user = await _applicationContext.Users
            .Include(user => user.Tokens)
            .FirstOrDefaultAsync(user => user.Email == email);
        if (user is null) return Result.Error<TokensResponse>("User not found");
        if (!user.IsEmailConfirmed) return Result.Error<TokensResponse>("Email is not confirmed");
        var isSuccess = Bcrypt.Verify(password, user.PasswordHash);
        if (!isSuccess) return Result.Error<TokensResponse>("Password is incorrect");
        var access = _accessTokenService.GetToken(user.Id);
        var refresh = _refreshTokenService.GetToken(user.Id);
        if (user.Tokens.Count == 3)
        {
            var oldestSession = user.Tokens.OrderBy(token => token.IssuedDateTime).First();
            user.Tokens.Remove(oldestSession);
        }
        var token = new Token
        {
            RefreshToken = refresh,
            UserId = user.Id
        };
        user.Tokens.Add(token);
        _applicationContext.Users.Update(user);
        await _applicationContext.SaveChangesAsync();
        return Result.Success(new TokensResponse(access, refresh));
    }

    public async Task<Result<Guid>> RegisterAsync(RegistrationRequest registrationRequest)
    {
        var isUserRegistered =
            (await _applicationContext.Users.FirstOrDefaultAsync(user => user.Email == registrationRequest.Email)) is not null;
        if (isUserRegistered) return Result.Error<Guid>("User with specified email is already registered");
        var hashedPassword = Bcrypt.HashPassword(registrationRequest.Password);
        var emailCode = _emailCodeGenerator.GenerateCode();
        var user = new User
        {
            Email = registrationRequest.Email,
            PasswordHash = hashedPassword,
            EmailConfirmationCode = emailCode,
            IsEmailConfirmed = false
        };
        await _applicationContext.Users.AddAsync(user);
        await _applicationContext.SaveChangesAsync();
        //TODO Send email via Notification Api
        return Result.Success(user.Id);
    }

    public async Task<Result<TokensResponse>> RefreshTokensAsync(Guid userId, string refreshToken)
    {
        var user = await _applicationContext.Users
            .Include(user1 => user1.Tokens)
            .FirstAsync(user1 => user1.Id == userId);
        var token = user.Tokens.FirstOrDefault(token1 => token1.RefreshToken == refreshToken);
        if (token is null) return Result.Error<TokensResponse>("You can not refresh tokens, that are not registered as trusted sessions");
        user.Tokens.Remove(token);
        var isRefreshCorrect = _refreshTokenService.VerifyToken(userId, refreshToken);
        if (!isRefreshCorrect) return Result.Error<TokensResponse>("Invalid refresh token provided or token has expired");
        var access = _accessTokenService.GetToken(userId);
        var refresh = _refreshTokenService.GetToken(userId);
        var newToken = new Token
        {
            RefreshToken = refresh,
            UserId = userId
        };
        user.Tokens.Add(newToken);
        await _applicationContext.Users.AddAsync(user);
        await _applicationContext.SaveChangesAsync();
        return Result.Success(new TokensResponse(access, refresh));
    }

    public async Task<Result<Guid>> ConfirmEmailAsync(Guid userId, string confirmationCode)
    {
        var user = await _applicationContext.Users
            .Include(user => user.Tokens)
            .FirstOrDefaultAsync(user => user.Id == userId);
        if (user is null) return Result.Error<Guid>("User not found");
        if (user.EmailConfirmationCode != confirmationCode) return Result.Error<Guid>("Confirmation code is incorrect");
        user.EmailConfirmationCode = null;
        user.IsEmailConfirmed = true;
        _applicationContext.Users.Update(user);
        await _applicationContext.SaveChangesAsync();
        return Result.Success(user.Id);
    }

    public Task<Result> ResetPasswordAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ConfirmPasswordResetAsync(Guid userId, string token, string newPassword, string newPasswordConfirmation)
    {
        throw new NotImplementedException();
    }
}