using System.Threading.Tasks;
using Benkyou.Application.Services.Identity;
using Benkyou.Domain.Exceptions;
using Benkyou.Domain.Extensions;
using Benkyou.Domain.Models.Requests;
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

    [HttpGet]
    [Route("check-email")]
    public async Task<ActionResult> IsEmailOccupied([FromQuery] string email)
    {
        var result = await _userService.IsEmailAvailableAsync(email);
        if (!result.IsSuccess) return Ok();
        return Conflict(new
        {
            errorMessage = "User with specified email already exists"
        });
    }

    [HttpGet]
    [Route("check-username")]
    public async Task<ActionResult> IsUserNameOccupied([FromQuery] string userName)
    {
        var result = await _userService.IsUserNameAvailableAsync(userName);
        if (!result.IsSuccess) return Ok();
        return Conflict(new
        {
            errorMessage = "User with specified username already exists"
        });
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
    {
        var result = await _userService.LoginAsync(loginModel);
        if (result.IsSuccess)
        {
            this.SetAccessAndRefreshCookie(result.Value!.AccessToken, result.Value!.RefreshToken);
            return Ok();
        }

        var exception = result.Exception!;
        return exception switch
        {
            LoginException => StatusCode(403, new
            {
                errorMessage = exception.Message,
                userId = (await _userService.GetUserGuidFromEmailAsync(loginModel.Email)).Value
            }),
            UserNotFoundException => NotFound(new {errorMessage = exception.Message}),
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
            UserRegistrationException => Conflict(new {errorMessage = exception.Message}),
            _ => BadRequest(new {errorMessage = exception.Message})
        };
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<ActionResult> RefreshToken()
    {
        var token = this.GetRefreshTokenFromCookie();
        var userIdResult = await _tokenValidationService.GetUserIdIfRefreshTokenValidAsync(token);
        if (!userIdResult.IsSuccess) return Unauthorized(new {errorMessage = userIdResult.Exception!.Message});
        var userId = userIdResult.Value;
        var result = await _userService.GetNewTokensAsync(userId);
        if (result.IsSuccess)
        {
            this.SetAccessAndRefreshCookie(result.Value!.AccessToken, result.Value!.RefreshToken);
            return Ok();
        }

        var exception = result.Exception!;
        return exception switch
        {
            RefreshTokenException => NotFound(new {errorMessage = exception.Message}),
            _ => BadRequest(new {errorMessage = exception.Message})
        };
    }

    [HttpPost]
    [Route("confirm-email")]
    public async Task<ActionResult> ConfirmEmailAddress([FromBody] ConfirmEmailRequest emailRequest)
    {
        var isConfirmedResult = await _userService.IsEmailConfirmedAsync(emailRequest.UserId);
        var exceptionConfirmed = isConfirmedResult.Exception;
        if (!isConfirmedResult.IsSuccess) return Conflict(new {errorMessage = exceptionConfirmed!.Message});
        var result = await _userService.ConfirmUserEmailAsync(emailRequest.UserId, emailRequest.EmailCode);
        if (result.IsSuccess) return Ok();
        var exception = result.Exception!;
        return exception switch
        {
            EmailConfirmationCodeException => BadRequest(new {errorMessage = exception.Message}),
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
            UserNotFoundException => NotFound(new {errorMessage = exception.Message}),
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
        var result = await _userService.SetNewUserForgottenPasswordAsync(email, confirmationRequest.Password, token);
        if (result.IsSuccess) return Ok();
        var exception = result.Exception!;
        return exception switch
        {
            UserNotFoundException => NotFound(new {errorMessage = exception.Message}),
            InvalidTokenException => BadRequest(new {errorMessage = exception.Message}),
            _ => StatusCode(500)
        };
    }
}