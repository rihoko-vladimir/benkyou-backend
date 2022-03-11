using System.Threading.Tasks;
using Benkyou.Application.Services.Identity;
using Benkyou.Domain.Exceptions;
using Benkyou.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Benkyou_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenValidationService _tokenValidationService;
    private readonly IUserService _userService;

    public AuthController(IUserService userService, ITokenValidationService tokenValidationService)
    {
        _userService = userService;
        _tokenValidationService = tokenValidationService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
    {
        var result = await _userService.LoginAsync(loginModel);
        if (result.IsSuccess) return Ok(result.Value);
        var exception = result.Exception!;
        return exception switch
        {
            LoginException => BadRequest(exception.Message),
            _ => StatusCode(500)
        };
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] RegisterModel registerModel)
    {
        var result = await _userService.RegisterAsync(registerModel);
        if (result.IsSuccess) return Ok(result.Value);
        var exception = result.Exception!;
        return exception switch
        {
            UserRegistrationException => Conflict(exception.Message),
            _ => BadRequest(exception.Message)
        };
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var userId =
            await _tokenValidationService.GetUserIdIfRefreshTokenValidAsync(refreshTokenRequest.RefreshToken);
        var result = await _userService.GetNewTokensAsync(userId);
        if (result.IsSuccess) return Ok(result.Value);
        var exception = result.Exception!;
        return exception switch
        {
            RefreshTokenException => NotFound(exception.Message),
            _ => BadRequest(exception.Message)
        };
    }

    [HttpPost]
    [Route("verify-email")]
    public async Task<ActionResult> VerifyEmailAddress([FromBody] VerifyEmailCodeRequest emailCodeRequest)
    {
        var result =
            await _userService.ValidateEmailCodeAsync(emailCodeRequest.UserId, emailCodeRequest.EmailCode);
        if (result.IsSuccess) return Ok();
        var exception = result.Exception!;
        return exception switch
        {
            EmailVerificationCodeException => BadRequest(exception.Message),
            _ => StatusCode(500)
        };
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
    {
        var result = await _userService.ResetPasswordAsync(resetPasswordRequest.EmailAddress);
        if (result.IsSuccess) return Ok();
        var exception = result.Exception!;
        return exception switch
        {
            UserNotFoundExceptions => NotFound(exception.Message),
            _ => BadRequest()
        };
    }

    [HttpGet]
    [Route("reset-password-confirm")]
    public ActionResult PasswordResetConfirmation()
    {
        return Ok();
    }

    [HttpPost]
    [Route("reset-password-confirm")]
    public async Task<ActionResult> PasswordResetConfirmation([FromQuery] string email, [FromQuery] string token,
        [FromBody] ResetPasswordConfirmationRequest confirmationRequest)
    {
        var result = await _userService.SetNewUserPasswordAsync(email, confirmationRequest.Password, token);
        if (result.IsSuccess) return Ok();
        var exception = result.Exception!;
        return exception switch
        {
            UserNotFoundExceptions => NotFound(exception.Message),
            InvalidTokenException => BadRequest(exception.Message),
            _ => StatusCode(500)
        };
    }
}