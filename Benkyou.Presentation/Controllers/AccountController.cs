using System.Threading.Tasks;
using Benkyou.Application.Services.Identity;
using Benkyou.Domain.Exceptions;
using Benkyou.Domain.Extensions;
using Benkyou.Domain.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Benkyou_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("")]
    public async Task<ActionResult> GetUserInfo()
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _userService.GetUserInfoAsync(userId);
        if (result.IsSuccess) return Ok(result.Value!);
        var exception = result.Exception!;
        return exception switch
        {
            UserNotFoundException => NotFound(new { errorMessage = exception.Message }),
            _ => StatusCode(500)
        };
    }

    [HttpPut]
    [Route("modify")]
    public async Task<ActionResult> UpdateUserInfo([FromBody] UpdateUserInfoRequest updateUserInfoRequest)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _userService.UpdateUserInfoAsync(userId, updateUserInfoRequest);
        if (result.IsSuccess) return Ok(result.Value);
        var exception = result.Exception!;
        return exception switch
        {
            UserNotFoundException => NotFound(new { errorMessage = exception.Message }),
            PasswordChangeException => BadRequest(new { errorMessage = exception.Message }),
            _ => StatusCode(500)
        };
    }

    [HttpPatch]
    [Route("change-visibility")]
    public async Task<ActionResult> ChangeAccountVisibility([FromQuery] bool isPublic)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _userService.ChangeVisibilityAsync(userId, isPublic);
        if (result.IsSuccess) return Ok();
        var exception = result.Exception!;
        return exception switch
        {
            UserNotFoundException => NotFound(new { errorMessage = exception.Message }),
            _ => StatusCode(500)
        };
    }
}