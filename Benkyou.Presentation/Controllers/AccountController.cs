using System;
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
        var result = await _userService.GetUserInfo(userId);
        if (result.IsSuccess) return Ok(result.Value!);
        var exception = result.Exception!;
        return exception switch
        {
            UserNotFoundException => NotFound(new {errorMessage = exception.Message}),
            _ => StatusCode(500)
        };
    }

    [HttpPatch]
    [Route("update-firstname")]
    public async Task<ActionResult> UpdateUserFirstName([FromBody] UpdateUserFirstNameRequest updateRequest)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _userService.SetNewUserFirstName(userId, updateRequest.FirstName);
        if (result.IsSuccess) return Ok();
        var exception = result.Exception!;
        return exception switch
        {
            UserNotFoundException => NotFound(new {errorMessage = exception.Message}),
            _ => StatusCode(500)
        };
    }

    [HttpPatch]
    [Route("update-lastname")]
    public async Task<ActionResult> UpdateUserLastName([FromBody] UpdateUserLastNameRequest updateRequest)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _userService.SetNewUserLastName(userId, updateRequest.LastName);
        if (result.IsSuccess) return Ok();
        var exception = result.Exception!;
        return exception switch
        {
            UserNotFoundException => NotFound(new {errorMessage = exception.Message}),
            _ => StatusCode(500)
        };
    }

    [HttpPatch]
    [Route("update-birthday")]
    public async Task<ActionResult> UpdateUserBirthday([FromBody] UpdateUserBirthdayRequest updateRequest)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var newBirthdayInDatetimeFormat = DateTime.Parse(updateRequest.Birthday);
        var result = await _userService.SetNewUserBirthday(userId, newBirthdayInDatetimeFormat);
        if (result.IsSuccess) return Ok();
        var exception = result.Exception!;
        return exception switch
        {
            UserNotFoundException => NotFound(new {errorMessage = exception.Message}),
            _ => StatusCode(500)
        };
    }

    [HttpPatch]
    [Route("update-about")]
    public async Task<ActionResult> UpdateUserAbout([FromBody] UpdateUserAboutRequest updateRequest)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _userService.SetNewUserAbout(userId, updateRequest.About);
        if (result.IsSuccess) return Ok();
        var exception = result.Exception!;
        return exception switch
        {
            UserNotFoundException => NotFound(new {errorMessage = exception.Message}),
            _ => StatusCode(500)
        };
    }
}