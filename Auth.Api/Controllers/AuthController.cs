using System.Net;
using Auth.Api.Extensions.ControllerExtensions;
using Auth.Api.Interfaces.Services;
using Auth.Api.Models.DbContext;
using Auth.Api.Models.Exceptions;
using Auth.Api.Models.Requests;
using Auth.Api.Models.Responses;
using Fido2NetLib;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Constants;
using Shared.Models.Models.Configurations;

namespace Auth.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly ApplicationContext _context;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly IValidator<ResetPasswordConfirmationRequest> _passwordConfirmationValidator;

    private readonly IValidator<RegistrationRequest> _registrationValidator;

    //TODO Fix results, so they'll also send error codes back
    private readonly IUserService _userService;

    public AuthController(IUserService userService,
        IValidator<RegistrationRequest> registrationValidator,
        IValidator<ResetPasswordConfirmationRequest> passwordConfirmationValidator,
        JwtConfiguration jwtConfiguration, ApplicationContext context, IAccessTokenService accessTokenService
    )
    {
        _userService = userService;
        _registrationValidator = registrationValidator;
        _passwordConfirmationValidator = passwordConfirmationValidator;
        _jwtConfiguration = jwtConfiguration;
        _context = context;
        _accessTokenService = accessTokenService;
    }
    
    [HttpGet]
    [Route("credential-options")]
    public async Task<ActionResult<CredentialCreateOptions>> GetCredentialOptions()
    {
        var userId = _accessTokenService.GetUserId(await this.GetAccessTokenAsync());

        var result = await _userService.CreateCredentialOptionsAsync(userId);
        
        if (!result.IsSuccess) return BadRequest(result.Message);
        
        HttpContext.Response.Cookies.Append("fido2.attestationOptions", result.Value!.ToJson());;

        return Ok(result.Value);
    }

    [HttpPost]
    [Route("create-passkey")]
    public async Task<ActionResult<AssertionOptions>> CreatePasskey([FromBody] AuthenticatorAttestationRawResponse attestationResponse)
    {
        var userId = _accessTokenService.GetUserId(await this.GetAccessTokenAsync());
        var jsonOptions = HttpContext.Request.Cookies["fido2.attestationOptions"];
        HttpContext.Session.Remove("fido2.attestationOptions");
        
        var options = CredentialCreateOptions.FromJson(jsonOptions);
        
        var result = await _userService.CreatePasskeyAsync(userId, attestationResponse, options);
        
        if (!result.IsSuccess) return BadRequest(result.Message);
        
        return Ok(result.Value);
    }

    [HttpGet]
    [Route("assertion-options")]
    public async Task<ActionResult<AssertionOptions>> GetAssertionOptions()
    {
        var userId = _accessTokenService.GetUserId(await this.GetAccessTokenAsync());

        var optionsResult = await _userService.GetAssertionOptionsAsync(userId);
        
        if (!optionsResult.IsSuccess) return BadRequest(optionsResult.Message);
        
        HttpContext.Response.Cookies.Append("fido2.assertionOptions", optionsResult.Value!.ToJson());
        
        return Ok(optionsResult.Value);
    }

    [HttpPost]
    [Route("passkey-login")]
    public async Task<IActionResult> PasskeyLogin([FromBody] AuthenticatorAssertionRawResponse clientResponse)
    {
        var userId = _accessTokenService.GetUserId(await this.GetAccessTokenAsync());
        var jsonOptions = HttpContext.Request.Cookies["fido2.assertionOptions"];
        var options = AssertionOptions.FromJson(jsonOptions);

        var result = await _userService.LoginPasskeyAsync(userId, clientResponse, options);
        
        return !result.IsSuccess ? StatusCode(500, result.Message) : Ok(result.Value);
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var result = await _userService.LoginAsync(loginRequest.Email, loginRequest.Password);

        if (!result.IsSuccess)
        {
            if (result.Exception is EmailNotConfirmedException ex)
                return StatusCode((int)HttpStatusCode.Forbidden, new EmailNotConfirmedResponse(ex.UserId));

            return BadRequest(result.Message);
        }

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

    [HttpPost]
    [Route("change-lock")]
    public async Task<IActionResult> ChangeUserLockedStatus([FromQuery] Guid userId)
    {
        var user = await _context.UserCredentials.FirstOrDefaultAsync(credential => credential.Id == userId);

        var role = _accessTokenService.GetRole(await this.GetAccessTokenAsync());

        if (user is not null && role == Role.Admin) user.IsAccountLocked = !user.IsAccountLocked;

        return Ok();
    }
}