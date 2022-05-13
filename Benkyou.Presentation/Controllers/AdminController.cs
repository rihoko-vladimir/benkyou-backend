using System;
using System.Threading.Tasks;
using Benkyou.Application.Services.Identity;
using Benkyou.Domain.Exceptions;
using Benkyou.Domain.Models;
using Benkyou.Infrastructure.Attributes;
using Benkyou.Infrastructure.Repositories;
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
    private readonly ApplicationUnitOfWork _unitOfWork;

    public AdminController(IUserService userService, ApplicationUnitOfWork unitOfWork)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
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
    [Route("removeUser")]
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

    [HttpGet]
    [Route("getUserSets")]
    public async Task<ActionResult> GetUserSets([FromQuery] string userId)
    {
        Guid.TryParse(userId, out var guidId);
        var result = await _unitOfWork.SetsRepository.GetUserSetsAsync(guidId);
        return Ok(result.Value);
    }

    [HttpDelete]
    [Route("removeSet")]
    public async Task<ActionResult> RemoveSet([FromQuery] string setId)
    {
        Guid.TryParse(setId, out var setIdGuid);
        var setResult = await _unitOfWork.SetsRepository.GetSetAsync(setIdGuid);
        var userId = setResult.Value!.AuthorId;
        var result = await _unitOfWork.SetsRepository.RemoveSetAsync(setIdGuid, userId);
        if (result.IsSuccess) return Ok();
        var exception = result.Exception!;
        return exception switch
        {
            InvalidSetIdException => NotFound(exception.Message),
            _ => StatusCode(500)
        };
    }
}