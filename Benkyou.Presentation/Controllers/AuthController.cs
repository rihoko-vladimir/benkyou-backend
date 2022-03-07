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
        var tokensResponse = await _userService.LoginAsync(loginModel);
        if (tokensResponse.AccessToken == string.Empty || tokensResponse.RefreshToken == string.Empty)
            return NotFound();

        return Ok(tokensResponse);
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] RegisterModel registerModel)
    {
        var isRegistrationSuccessful = await _userService.RegisterAsync(registerModel);
        if (!isRegistrationSuccessful) return Conflict();

        return Ok("Please, verify your email address");
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
}