using System;
using System.Threading.Tasks;
using Benkyou.Application.Services.Identity;
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
        try
        {
            var tokens = await _userService.LoginAsync(loginModel);
            return Ok(tokens);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] RegisterModel registerModel)
    {
        try
        {
            var userId = await _userService.RegisterAsync(registerModel);
            return Ok(userId);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        try
        {
            var userId =
                await _tokenValidationService.GetUserIdIfRefreshTokenValidAsync(refreshTokenRequest.RefreshToken);
            var tokensResponse = await _userService.GetNewTokensAsync(userId);
            return Ok(tokensResponse);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Route("verify-email")]
    public async Task<ActionResult> VerifyEmailAddress([FromBody] VerifyEmailCodeRequest emailCodeRequest)
    {
        try
        {
            var isCorrect =
                await _userService.ValidateEmailCodeAsync(emailCodeRequest.UserId, emailCodeRequest.EmailCode);
            return isCorrect ? Ok() : BadRequest("Email code is incorrect");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
    {
        try
        {
            await _userService.ResetPasswordAsync(resetPasswordRequest.EmailAddress);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
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
        try
        {
            await _userService.SetNewUserPasswordAsync(email, confirmationRequest.Password, token);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}