using Auth.Api.Interfaces.Generators;
using Auth.Api.Interfaces.Repositories;
using Auth.Api.Interfaces.Services;
using Auth.Api.Models.Entities;
using Auth.Api.Models.Requests;
using Auth.Api.Models.Responses;
using Serilog;
using Shared.Models.Models;
using Bcrypt = BCrypt.Net.BCrypt;

namespace Auth.Api.Services;

public class UserService : IUserService
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly IEmailCodeGenerator _emailCodeGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IResetTokenService _resetTokenService;
    private readonly ISenderService _senderService;
    private readonly IUserCredentialsRepository _userCredentialsRepository;

    public UserService(IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService, IResetTokenService resetTokenService,
        IEmailCodeGenerator emailCodeGenerator,
        ISenderService senderService,
        IUserCredentialsRepository userCredentialsRepository)
    {
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _resetTokenService = resetTokenService;
        _emailCodeGenerator = emailCodeGenerator;
        _senderService = senderService;
        _userCredentialsRepository = userCredentialsRepository;
    }

    public async Task<Result<TokensResponse>> LoginAsync(string email, string password)
    {
        if (!await _userCredentialsRepository.IsUserExistsByEmailAsync(email))
        {
            Log.Warning("Login attempt with incorrect email address. Email: {Email}, Password: {Password}", email,
                password);

            return Result.Error<TokensResponse>("User not found");
        }

        var user = await _userCredentialsRepository.GetUserByEmailAsync(email);

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

        await _userCredentialsRepository.UpdateUserAsync(user);

        return Result.Success(new TokensResponse(access, refresh));
    }

    public async Task<Result<Guid>> RegisterAsync(RegistrationRequest registrationRequest)
    {
        var isUserRegistered =
            await _userCredentialsRepository.IsUserExistsByEmailAsync(registrationRequest.Email);

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

        await _userCredentialsRepository.CreateUserCredentialAsync(user);

        var result = await _senderService.SendEmailCodeMessageAsync(emailCode, registrationRequest.Email);

        var registrationCredResult =
            await _senderService.SendRegistrationMessageAsync(user.Id, registrationRequest.FirstName,
                registrationRequest.LastName, registrationRequest.UserName, registrationRequest.IsTermsAccepted);

        return !result.IsSuccess && !registrationCredResult.IsSuccess ? Result.Error<Guid>("Message broker error") : Result.Success(user.Id);
    }

    public async Task<Result<TokensResponse>> RefreshTokensAsync(string refreshToken)
    {
        if (!_refreshTokenService.GetGuidFromRefreshToken(refreshToken, out var userId))
            return Result.Error<TokensResponse>("Token is invalid");

        if (!await _userCredentialsRepository.IsUserExistsByIdAsync(userId))
        {
            Log.Warning("Someone tried to refresh token of not existing user, Token: {Token}", refreshToken);

            return Result.Error<TokensResponse>("User not found");
        }

        var user = await _userCredentialsRepository.GetUserByIdAsync(userId);

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
        await _userCredentialsRepository.UpdateUserAsync(user);

        return Result.Success(new TokensResponse(access, refresh));
    }

    public async Task<Result<Guid>> ConfirmEmailAsync(Guid userId, string confirmationCode)
    {
        if (!await _userCredentialsRepository.IsUserExistsByIdAsync(userId))
        {
            Log.Warning("Someone tried to confirm email of not existing user, User: {User}", userId);

            return Result.Error<Guid>("User not found");
        }

        var user = await _userCredentialsRepository.GetUserByIdAsync(userId);

        var isCodeCorrect = _emailCodeGenerator.VerifyCode(confirmationCode, user);
        
        if (!isCodeCorrect)
        {
            Log.Warning("Provided confirmation code is incorrect. User : {User}, EmailCode: {Code}", userId,
                confirmationCode);

            return Result.Error<Guid>("Confirmation code is incorrect");
        }

        user.EmailConfirmationCode = null;
        user.IsEmailConfirmed = true;

        await _userCredentialsRepository.UpdateUserAsync(user);

        return Result.Success(user.Id);
    }

    public async Task<Result> ResetPasswordAsync(string email)
    {
        if (!await _userCredentialsRepository.IsUserExistsByEmailAsync(email))
        {
            Log.Warning("Someone tried to reset password of not existing user, Email: {Email}", email);

            return Result.Error("User not found");
        }

        var user = await _userCredentialsRepository.GetUserByEmailAsync(email);

        var resetCode = _resetTokenService.GetToken(user.Id);

        var result = await _senderService.SendResetPasswordMessageAsync(resetCode, email);

        return !result.IsSuccess ? Result.Error("Message broker error") : Result.Success();
    }

    public async Task<Result> ConfirmPasswordResetAsync(string email, string token, string newPassword)
    {
        if (!await _userCredentialsRepository.IsUserExistsByEmailAsync(email))
        {
            Log.Warning(
                "Someone tried to confirm password resetting of not existing user, Email: {Email}. Token: {Token}",
                email, token);

            return Result.Error("User not found");
        }

        var user = await _userCredentialsRepository.GetUserByEmailAsync(email);

        var isCorrect = _resetTokenService.VerifyToken(user.Id, token);
        if (!isCorrect)
        {
            Log.Warning("Incorrect reset token provided. Email: {Email}, Token: {Token}", email, token);

            return Result.Error("Reset token is incorrect");
        }

        var newPasswordHash = Bcrypt.HashPassword(newPassword);
        user.PasswordHash = newPasswordHash;

        await _userCredentialsRepository.UpdateUserAsync(user);

        return Result.Success();
    }
}