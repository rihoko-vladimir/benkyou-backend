using System;
using System.Linq;
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
    [Route("my-sets")]
    public async Task<ActionResult> GetMySets()
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _unitOfWork.SetsRepository.GetAllSetsAsync(userId);
        var mySets = result.Value!;
        return Ok(mySets.ToArray());
    }

    [HttpPut]
    [Route("create-set")]
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
        return Ok(new
        {
            cardId = result.Value
        });
    }

    [HttpDelete]
    [Route("delete-set")]
    public async Task<ActionResult> DeleteSet([FromBody] DeleteSetRequest deleteSetRequest)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _unitOfWork.SetsRepository.RemoveSetAsync(Guid.Parse(deleteSetRequest.SetId), userId);
        if (!result.IsSuccess)
        {
            var exception = result.Exception!;
            return exception switch
            {
                InvalidCardIdException => BadRequest(new {errorMessage = exception.Message}),
                CardRemoveException => Unauthorized(new {errorMessage = exception.Message}),
                _ => StatusCode(500)
            };
        }

        await _unitOfWork.SetsRepository.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch]
    [Route("update-name")]
    public async Task<ActionResult> UpdateSetName([FromBody] UpdateSetNameRequest updateSetNameRequest)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _unitOfWork.SetsRepository.ModifySetNameAsync(Guid.Parse(updateSetNameRequest.SetId),
            updateSetNameRequest.NewName, userId);
        if (!result.IsSuccess)
        {
            var exception = result.Exception!;
            return exception switch
            {
                InvalidCardIdException => NotFound(new {errorMessage = exception.Message}),
                CardUpdateException => Unauthorized(new {errorMessage = exception.Message}),
                _ => StatusCode(500)
            };
        }

        await _unitOfWork.SetsRepository.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch]
    [Route("update-description")]
    public async Task<ActionResult> UpdateSetDescription(
        [FromBody] UpdateSetDescriptionRequest updateSetDescriptionRequest)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _unitOfWork.SetsRepository.ModifySetDescriptionAsync(
            Guid.Parse(updateSetDescriptionRequest.SetId),
            updateSetDescriptionRequest.NewName, userId);
        if (!result.IsSuccess)
        {
            var exception = result.Exception!;
            return exception switch
            {
                InvalidCardIdException => NotFound(new {errorMessage = exception.Message}),
                CardUpdateException => Unauthorized(new {errorMessage = exception.Message}),
                _ => StatusCode(500)
            };
        }

        await _unitOfWork.SetsRepository.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch]
    [Route("update-kanji")]
    public async Task<ActionResult> UpdateSetKanji([FromBody] UpdateSetKanjiListRequest updateSetKanjiListRequest)
    {
        var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
        var result = await _unitOfWork.SetsRepository.ModifySetKanjiListAsync(
            Guid.Parse(updateSetKanjiListRequest.SetId),
            updateSetKanjiListRequest.NewKanjiList.ToList(), userId);
        if (!result.IsSuccess)
        {
            var exception = result.Exception!;
            return exception switch
            {
                InvalidCardIdException => NotFound(new {errorMessage = exception.Message}),
                CardUpdateException => Unauthorized(new {errorMessage = exception.Message}),
                _ => StatusCode(500)
            };
        }

        await _unitOfWork.SetsRepository.SaveChangesAsync();
        return Ok();
    }
}