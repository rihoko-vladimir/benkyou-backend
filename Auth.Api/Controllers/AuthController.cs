using Auth.Api.Extensions.ControllerExtensions;
using Auth.Api.Interfaces.Services;
using Auth.Api.Models.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Models.Configurations;

namespace Auth.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly IValidator<ResetPasswordConfirmationRequest> _passwordConfirmationValidator;

    private readonly IValidator<RegistrationRequest> _registrationValidator;

    //TODO Fix results, so they'll also send error codes back
    private readonly IUserService _userService;

    public AuthController(IUserService userService,
        IValidator<RegistrationRequest> registrationValidator,
        IValidator<ResetPasswordConfirmationRequest> passwordConfirmationValidator,
        JwtConfiguration jwtConfiguration
    )
    {
        _userService = userService;
        _registrationValidator = registrationValidator;
        _passwordConfirmationValidator = passwordConfirmationValidator;
        _jwtConfiguration = jwtConfiguration;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var result = await _userService.LoginAsync(loginRequest.Email, loginRequest.Password);

        if (!result.IsSuccess) return BadRequest(result.Message);

        this.SetAccessAndRefreshCookie(result.Value!.AccessToken, result.Value!.RefreshToken, _jwtConfiguration);

        return Ok();
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] RegistrationRequest registrationRequest)
    {
        var validationResult = await _registrationValidator.ValidateAsync(registrationRequest);

        if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

        var result = await _userService.RegisterAsync(registrationRequest);

        if (!result.IsSuccess) return BadRequest(result.Message);

        return Ok(result.Value);
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<ActionResult> Refresh()
    {
        var refreshToken = this.GetRefreshTokenFromCookie();
        var result = await _userService.RefreshTokensAsync(refreshToken);

        if (!result.IsSuccess) return BadRequest(result.Message);

        this.SetAccessAndRefreshCookie(result.Value!.AccessToken, result.Value!.RefreshToken, _jwtConfiguration);

        return Ok();
    }

    [HttpPost]
    [Route("confirm-email")]
    public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest confirmEmailRequest)
    {
        var result = await _userService.ConfirmEmailAsync(confirmEmailRequest.UserId, confirmEmailRequest.EmailCode);

        if (!result.IsSuccess) return BadRequest(result.Message);

        return Ok(result.Value);
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<ActionResult> ResetPassword([FromQuery] string email)
    {
        var result = await _userService.ResetPasswordAsync(email);

        if (!result.IsSuccess) return BadRequest(result.Message);

        return Ok();
    }

    [HttpPost]
    [Route("confirm-reset-password")]
    public async Task<ActionResult> ResetPasswordConform([FromQuery] string email, string token,
        [FromBody] ResetPasswordConfirmationRequest confirmationRequest)
    {
        var validationResult = await _passwordConfirmationValidator.ValidateAsync(confirmationRequest);

        if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));

        var result = await _userService.ConfirmPasswordResetAsync(email, token, confirmationRequest.Password);

        if (!result.IsSuccess) return BadRequest(result.Message);

        return Ok(result);
    }
}