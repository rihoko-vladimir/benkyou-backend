using System;
using System.Threading.Tasks;
using Benkyou.Application.Services.Identity;
using Benkyou.Domain.Exceptions;
using Benkyou.Domain.Models;
using Benkyou.Infrastructure.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Benkyou_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Role(Role.Admin)]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("users")]
    public async Task<ActionResult> GetAllUsersInfo()
    {
        var result = await _userService.GetAllUsersAsync();
        if (result.IsSuccess) return Ok(result.Value);
        return StatusCode(500);
    }

    [HttpDelete]
    [Route("remove")]
    public async Task<ActionResult> RemoveUser([FromQuery] string userId)
    {
        Guid.TryParse(userId, out var guidId);
        var result = await _userService.RemoveUserAsync(guidId);
        if (result.IsSuccess) return Ok();
        var exception = result.Exception!;
        return exception switch
        {
            UserNotFoundException => NotFound(exception.Message),
            _ => StatusCode(500)
        };
    }
}