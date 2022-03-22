using System;
using System.Threading.Tasks;
using Benkyou.Application.Services.Identity;
using Benkyou.Domain.Exceptions;
using Benkyou.Domain.Extensions;
using Benkyou.Domain.Models.Requests;
using Benkyou.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Benkyou_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SetsController : ControllerBase
{
    private readonly ApplicationUnitOfWork _unitOfWork;
    private readonly IUserService _userService;

    public SetsController(ApplicationUnitOfWork unitOfWork, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

    [HttpGet]
    [Route("my-collection")]
    public async Task<ActionResult> GetMySets()
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _unitOfWork.SetsRepository.GetUserSetsAsync(userId);
        var mySets = result.Value!;
        return Ok(mySets.ToArray());
    }

    [HttpPut]
    [Route("create")]
    public async Task<ActionResult> CreateNewSet([FromBody] CreateSetRequest setRequest)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _unitOfWork.SetsRepository.CreateSetAsync(setRequest, userId);
        if (!result.IsSuccess)
        {
            var exception = result.Exception!;
            return exception switch
            {
                UserNotFoundException => NotFound(new {errorMessage = exception.Message}),
                KanjiCountException => BadRequest(new {errorMessage = exception.Message}),
                _ => StatusCode(500)
            };
        }

        await _unitOfWork.SetsRepository.SaveChangesAsync();
        return Ok(result.Value);
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> DeleteSet([FromQuery] string setId)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _unitOfWork.SetsRepository.RemoveSetAsync(Guid.Parse(setId), userId);
        if (!result.IsSuccess)
        {
            var exception = result.Exception!;
            return exception switch
            {
                InvalidSetIdException => NotFound(new {errorMessage = exception.Message}),
                CardRemoveException => Unauthorized(new {errorMessage = exception.Message}),
                _ => StatusCode(500)
            };
        }

        await _unitOfWork.SetsRepository.SaveChangesAsync();
        return Ok();
    }

    [HttpPut]
    [Route("modify")]
    public async Task<ActionResult> ModifyUserSet([FromBody] ModifySetRequest modifySetRequest)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _unitOfWork.SetsRepository.ModifySetAsync(modifySetRequest, userId);
        if (!result.IsSuccess)
        {
            var exception = result.Exception!;
            return exception switch
            {
                UserNotFoundException => NotFound(new {errorMessage = exception.Message}),
                KanjiCountException => BadRequest(new {errorMessage = exception.Message}),
                SetUpdateException => NotFound(new {errorMessage = exception.Message}),
                _ => StatusCode(500)
            };
        }

        await _unitOfWork.SetsRepository.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    [Route("all")]
    public async Task<ActionResult> GetAllSets([FromQuery] int page)
    {
        var result = await _unitOfWork.SetsRepository.GetAllSetsByPageAsync(page);
        if (result.IsSuccess) return Ok(result.Value);
        return BadRequest(result.Value);
    }
}