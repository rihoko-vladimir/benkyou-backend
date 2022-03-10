using System;
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

    [HttpPost]
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
}