using Auth.Api.Interfaces.Generators;
using Auth.Api.Interfaces.Services;
using Auth.Api.Models;
using Auth.Api.Models.DbContext;
using Auth.Api.Models.Entities;
using Auth.Api.Models.Requests;
using Auth.Api.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Bcrypt = BCrypt.Net.BCrypt;

namespace Auth.Api.Services;

public class UserService : IUserService
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly ApplicationContext _applicationContext;
    private readonly IEmailCodeGenerator _emailCodeGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IResetTokenService _resetTokenService;
    private readonly ISenderService _senderService;

    public UserService(ApplicationContext applicationContext, IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService, IResetTokenService resetTokenService,
        IEmailCodeGenerator emailCodeGenerator,
        ISenderService senderService)
    {
        _applicationContext = applicationContext;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _resetTokenService = resetTokenService;
        _emailCodeGenerator = emailCodeGenerator;
        _senderService = senderService;
    }

    public async Task<Result<TokensResponse>> LoginAsync(string email, string password)
    {
        if (!await IsUserExistsByEmailAsync(email))
        {
            Log.Warning("Login attempt with incorrect email address. Email: {Email}, Password: {Password}", email,
                password);

            return Result.Error<TokensResponse>("User not found");
        }

        var user = (await _applicationContext.UserCredentials
            .Include(user => user.Tokens)
            .FirstOrDefaultAsync(user => user.Email == email))!;

        if (!user.IsEmailConfirmed)
        {
            Log.Warning("User tried to log in when email is not confirmed. Email: {Email}", email);

            return Result.Error<TokensResponse>("Email is not confirmed");
        }

        var isSuccess = Bcrypt.Verify(password, user.PasswordHash);
        if (!isSuccess)
        {
            Log.Warning("Sign in attempt with incorrect password. Email: {Email}", email);

            return Result.Error<TokensResponse>("Password is incorrect");
        }

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

        _applicationContext.UserCredentials.Update(user);
        await _applicationContext.SaveChangesAsync();

        return Result.Success(new TokensResponse(access, refresh));
    }

    public async Task<Result<Guid>> RegisterAsync(RegistrationRequest registrationRequest)
    {
        var isUserRegistered =
            await _applicationContext.UserCredentials.FirstOrDefaultAsync(user =>
                    user.Email == registrationRequest.Email) is not
                null;

        if (isUserRegistered)
        {
            Log.Warning("Registration attempt with already existing email address. Email: {Email}",
                registrationRequest.Email);

            return Result.Error<Guid>("User with specified email is already registered");
        }

        var hashedPassword = Bcrypt.HashPassword(registrationRequest.Password);

        var emailCode = _emailCodeGenerator.GenerateCode();

        var user = new UserCredential
        {
            Email = registrationRequest.Email,
            PasswordHash = hashedPassword,
            EmailConfirmationCode = emailCode,
            IsEmailConfirmed = false
        };

        await _applicationContext.UserCredentials.AddAsync(user);
        await _applicationContext.SaveChangesAsync();

        var result = await _senderService.SendEmailCodeAsync(emailCode, registrationRequest.Email);

        return !result.IsSuccess ? Result.Error<Guid>("Message broker error") : Result.Success(user.Id);
    }

    public async Task<Result<TokensResponse>> RefreshTokensAsync(string refreshToken)
    {
        if (!_refreshTokenService.GetGuidFromRefreshToken(refreshToken, out var userId))
            return Result.Error<TokensResponse>("Token is invalid");

        if (!await IsUserExistsByIdAsync(userId))
        {
            Log.Warning("Someone tried to refresh token of not existing user, Token: {Token}", refreshToken);

            return Result.Error<TokensResponse>("User not found");
        }

        var user = await _applicationContext.UserCredentials
            .Include(user1 => user1.Tokens)
            .FirstAsync(user1 => user1.Id == userId);

        var token = user.Tokens.FirstOrDefault(token1 => token1.RefreshToken == refreshToken);

        if (token is null)
        {
            Log.Warning("Someone tried to refresh token that doesn't exist as registered session. Token: {Token}",
                refreshToken);

            return Result.Error<TokensResponse>(
                "You can not refresh tokens, that are not registered as trusted sessions");
        }

        user.Tokens.Remove(token);

        var isRefreshCorrect = _refreshTokenService.VerifyToken(userId, refreshToken);

        if (!isRefreshCorrect)
        {
            Log.Warning("Attempt to refresh an expired token. Token: {Token}", refreshToken);

            return Result.Error<TokensResponse>("Invalid refresh token provided or token has expired");
        }

        var access = _accessTokenService.GetToken(userId);
        var refresh = _refreshTokenService.GetToken(userId);

        var newToken = new Token
        {
            RefreshToken = refresh,
            UserId = userId
        };

        user.Tokens.Add(newToken);
        _applicationContext.UserCredentials.Update(user);
        await _applicationContext.SaveChangesAsync();

        return Result.Success(new TokensResponse(access, refresh));
    }

    public async Task<Result<Guid>> ConfirmEmailAsync(Guid userId, string confirmationCode)
    {
        if (!await IsUserExistsByIdAsync(userId))
        {
            Log.Warning("Someone tried to confirm email of not existing user, User: {User}", userId);

            return Result.Error<Guid>("User not found");
        }

        var user = (await _applicationContext.UserCredentials
            .Include(user => user.Tokens)
            .FirstOrDefaultAsync(user => user.Id == userId))!;

        var isCodeCorrect = _emailCodeGenerator.VerifyCode(confirmationCode, user);
        if (!isCodeCorrect)
        {
            Log.Warning("Provided confirmation code is incorrect. User : {User}, EmailCode: {Code}", userId,
                confirmationCode);

            return Result.Error<Guid>("Confirmation code is incorrect");
        }

        user.EmailConfirmationCode = null;
        user.IsEmailConfirmed = true;

        _applicationContext.UserCredentials.Update(user);
        await _applicationContext.SaveChangesAsync();

        return Result.Success(user.Id);
    }

    public async Task<Result> ResetPasswordAsync(string email)
    {
        if (!await IsUserExistsByEmailAsync(email))
        {
            Log.Warning("Someone tried to reset password of not existing user, Email: {Email}", email);

            return Result.Error("User not found");
        }

        var user = (await _applicationContext.UserCredentials
            .Include(user => user.Tokens)
            .FirstOrDefaultAsync(user => user.Email == email))!;

        var resetCode = _resetTokenService.GetToken(user.Id);

        var result = await _senderService.SendResetPasswordAsync(resetCode, email);

        return !result.IsSuccess ? Result.Error("Message broker error") : Result.Success();
    }

    public async Task<Result> ConfirmPasswordResetAsync(string email, string token, string newPassword)
    {
        if (!await IsUserExistsByEmailAsync(email))
        {
            Log.Warning(
                "Someone tried to confirm password resetting of not existing user, Email: {Email}. Token: {Token}",
                email, token);

            return Result.Error("User not found");
        }

        var user = await _applicationContext.UserCredentials.FirstAsync(user1 => user1.Email == email);

        var isCorrect = _resetTokenService.VerifyToken(user.Id, token);
        if (!isCorrect)
        {
            Log.Warning("Incorrect reset token provided. Email: {Email}, Token: {Token}", email, token);

            return Result.Error("Reset token is incorrect");
        }

        var newPasswordHash = Bcrypt.HashPassword(newPassword);
        user.PasswordHash = newPasswordHash;

        _applicationContext.UserCredentials.Update(user);
        await _applicationContext.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<bool> IsUserExistsByIdAsync(Guid userId)
    {
        var user = await _applicationContext.UserCredentials
            .FirstOrDefaultAsync(user => user.Id == userId);

        return user is not null;
    }

    private async Task<bool> IsUserExistsByEmailAsync(string email)
    {
        var user = await _applicationContext.UserCredentials
            .FirstOrDefaultAsync(user => user.Email == email);

        return user is not null;
    }
}