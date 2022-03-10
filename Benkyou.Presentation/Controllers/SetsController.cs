using System;
using System.Linq;
using System.Threading.Tasks;
using Benkyou.Application.Services.Identity;
using Benkyou.Domain.Extensions;
using Benkyou.Domain.Models;
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
        try
        {
            var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
            var mySets = await _unitOfWork.SetsRepository.GetAllSetsAsync(userId);
            return Ok(mySets.ToArray());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    [Route("create-set")]
    public async Task<ActionResult> CreateNewSet([FromBody] CreateSetRequest setRequest)
    {
        try
        {
            var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
            var cardId = await _unitOfWork.SetsRepository.CreateSetAsync(setRequest, userId);
            await _unitOfWork.SetsRepository.SaveChangesAsync();
            return Ok(new
            {
                cardId
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete]
    [Route("delete-set")]
    public async Task<ActionResult> DeleteSet([FromBody] DeleteSetRequest deleteSetRequest)
    {
        try
        {
            var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
            await _unitOfWork.SetsRepository.RemoveSetAsync(Guid.Parse(deleteSetRequest.SetId),userId);
            await _unitOfWork.SetsRepository.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPatch]
    [Route("update-name")]
    public async Task<ActionResult> UpdateSetName([FromBody] UpdateSetNameRequest updateSetNameRequest)
    {
        try
        {
            var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
            await _unitOfWork.SetsRepository.ModifySetNameAsync(Guid.Parse(updateSetNameRequest.SetId),
                updateSetNameRequest.NewName, userId);
            await _unitOfWork.SetsRepository.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPatch]
    [Route("update-description")]
    public async Task<ActionResult> UpdateSetDescription([FromBody] UpdateSetDescriptionRequest updateSetDescriptionRequest)
    {
        try
        {
            var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
            await _unitOfWork.SetsRepository.ModifySetDescriptionAsync(Guid.Parse(updateSetDescriptionRequest.SetId),
                updateSetDescriptionRequest.NewName, userId);
            await _unitOfWork.SetsRepository.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPatch]
    [Route("update-kanji")]
    public async Task<ActionResult> UpdateSetKanji([FromBody] UpdateSetKanjiListRequest updateSetKanjiListRequest)
    {
        try
        {
            var userId = _userService.GetUserGuidFromAccessToken(await this.GetTokenAsync());
            await _unitOfWork.SetsRepository.ModifySetKanjiListAsync(Guid.Parse(updateSetKanjiListRequest.SetId),
                updateSetKanjiListRequest.NewKanjiList.ToList(), userId);
            await _unitOfWork.SetsRepository.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}