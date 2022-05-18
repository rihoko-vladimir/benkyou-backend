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
    public async Task<ActionResult> GetAllSets([FromQuery] int page, [FromQuery] int size)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        if (page <= 0 || size <= 0)
            return BadRequest(new
            {
                errorMessage = "Invalid page or page size provided"
            });
        var pageCountResult = await _unitOfWork.SetsRepository.GetAllSetsPageCountAsync(userId, size);
        var result = await _unitOfWork.SetsRepository.GetAllSetsByPageAsync(userId, page, size);
        if (result.IsSuccess && pageCountResult.IsSuccess)
            return Ok(new
            {
                pages = pageCountResult.Value,
                page,
                size,
                sets = result.Value
            });
        return BadRequest(new
        {
            errorMessage = result.Value
        });
    }

    [HttpPost]
    [Route("report")]
    public async Task<ActionResult> ReportUserSet([FromBody] ReportRequest reportRequest)
    {
        var setIdResult = Guid.TryParse(reportRequest.SetId, out var setIdGuid);
        var userIdResult = Guid.TryParse(reportRequest.UserId, out var userIdGuid);
        if (!setIdResult || !userIdResult) return BadRequest();
        var result = await _unitOfWork.SetsRepository.ReportSetAsync(userIdGuid, setIdGuid, reportRequest.ReportReason);
        if (!result.IsSuccess) return BadRequest(new {errorMessage = result.Exception!.Message});
        await _unitOfWork.SetsRepository.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    [Route("all_search")]
    public async Task<ActionResult> GetAllSetsByQuery([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string name)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        if (page <= 0 || size <= 0)
            return BadRequest(new
            {
                errorMessage = "Invalid page or page size provided"
            });

        var pageCountResult =
            await _unitOfWork.SetsRepository.GetAllSetsByQueryPageCountAsync(userId, page, size, name);

        if (!pageCountResult.IsSuccess)
            return BadRequest(new
            {
                errorMessage = "You've typed incorrect page or page size"
            });
        var result = await _unitOfWork.SetsRepository.GetSetsByQueryAsync(userId, name, page, size);
        if (result.IsSuccess && pageCountResult.IsSuccess)
            return Ok(new
            {
                search = name,
                pages = pageCountResult.Value,
                page,
                size,
                sets = result.Value
            });
        return BadRequest(new
        {
            errorMessage = result.Value
        });
    }
}