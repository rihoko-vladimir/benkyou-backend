using System;
using System.Threading.Tasks;
using Benkyou.Application.Services.Common;
using Benkyou.Application.Services.Identity;
using Benkyou.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Benkyou_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenValidationService _tokenValidationService;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IUserService _userService;

    public AuthController(IUserService userService, ITokenValidationService tokenValidationService, IAccessTokenService accessTokenService)
    {
        _userService = userService;
        _tokenValidationService = tokenValidationService;
        _accessTokenService = accessTokenService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
    {
        var tokensResponse = await _userService.LoginAsync(loginModel);
        if (tokensResponse.AccessToken == string.Empty || tokensResponse.RefreshToken == string.Empty)
            return NotFound();

        return Ok(tokensResponse);
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
            var tokensResponse = await _userService.GetNewTokens(userId);
            return Ok(tokensResponse);
        }
        catch (Exception e)
        {
            return BadRequest("Refresh token is incorrect");
        }
    }

    [HttpPost]
    [Route("verify-email")]
    public async Task<ActionResult> VerifyEmailAddress([FromBody] VerifyEmailCodeRequest emailCodeRequest)
    { 
        var isCorrect = await _userService.ValidateEmailCodeAsync(emailCodeRequest.UserId,emailCodeRequest.EmailCode);
        if (isCorrect) return Ok("Email confirmed");
        return BadRequest("Email code is incorrect");
    }
}