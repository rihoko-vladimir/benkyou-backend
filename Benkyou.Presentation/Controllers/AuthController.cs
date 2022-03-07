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
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
    {
        var tokensResponse = await _userService.LoginAsync(loginModel);
        if (tokensResponse.AccessToken==string.Empty || tokensResponse.RefreshToken==string.Empty)
        {
            return NotFound();
        }

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
}