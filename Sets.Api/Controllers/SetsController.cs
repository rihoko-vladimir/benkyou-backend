using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Sets.Api.Extensions.ControllerExtensions;
using Sets.Api.Interfaces.Services;
using Sets.Api.Models.Entities;
using Sets.Api.Models.Requests;

namespace Sets.Api.Controllers;


[ApiController]
[Route("/api/v{version:apiVersion}/sets")]
[Authorize]
public class SetsController : ControllerBase
{
    private readonly ISetsService _setsService;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IValidator<SetRequest> _setRequestValidator;
    private readonly IMapper _mapper;

    public SetsController(ISetsService setsService, 
        IAccessTokenService accessTokenService,
        IValidator<SetRequest> setRequestValidator,
        IMapper mapper)
    {
        _setsService = setsService;
        _accessTokenService = accessTokenService;
        _setRequestValidator = setRequestValidator;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> CreateNewSetAsync([FromBody] SetRequest setRequest)
    {
        var validationResult = await _setRequestValidator.ValidateAsync(setRequest);

        var token = await this.GetAccessTokenFromCookieAsync();
        _accessTokenService.GetGuidFromAccessToken(token, out var userId);
        
        if (!validationResult.IsValid)
        {
            Log.Warning("Set info validation failed while creating new one, user : {UserId}", userId);

            return BadRequest(validationResult.ToString("~"));
        }

        var result = await _setsService.CreateSetAsync(userId, setRequest);

        if (!result.IsSuccess) return BadRequest(result.Message);

        return Ok(result.Value);
    }

    [HttpPatch]
    [Route("modify")]
    public async Task<ActionResult> PatchSetAsync(
        JsonPatchDocument<SetRequest> patchDocument,
        [FromQuery] Guid setId)
    {
        var setResult = await _setsService.GetSet(setId);

        if (!setResult.IsSuccess)
        {
            Log.Warning("No set with id : {Id} was found", setId);
            
            return BadRequest(setResult.Message);
        }
        
        var setValue = setResult.Value!;
        var setRequestDto = _mapper.Map<SetRequest>(setValue);
        try
        {
            patchDocument.ApplyTo(setRequestDto);
        }
        catch (Exception e)
        {
            Log.Error("An error occured, because there were malformed patch request. {Exception}, {Message}",
                e.GetType().FullName, e.Message);
            return BadRequest(e.Message);
        }

        var validationResult = await _setRequestValidator.ValidateAsync(setRequestDto);

        var token = await this.GetAccessTokenFromCookieAsync();
        _accessTokenService.GetGuidFromAccessToken(token, out var userId);
        
        if (!validationResult.IsValid)
        {
            Log.Warning("Set info validation failed while attempting to patch, set: {SetId}, user : {UserId}", setId, userId);
            
            return BadRequest(validationResult.ToString("~"));
        }

        setValue.Name = setRequestDto.Name;
        setValue.Description = setRequestDto.Description;
        setValue.KanjiList = _mapper.Map<List<Kanji>>(setRequestDto.KanjiList);

        var patchResult = await _setsService.PatchSetAsync(userId, setId, setValue);

        if (!patchResult.IsSuccess) return BadRequest(patchResult.Message);

        return Ok(patchResult.Value);
    }

    [HttpDelete]
    [Route("remove")]
    public async Task<ActionResult> RemoveSetAsync([FromQuery] Guid setId)
    {
        var token = await this.GetAccessTokenFromCookieAsync();
        _accessTokenService.GetGuidFromAccessToken(token, out var userId);
        var removeResult = await _setsService.RemoveSetAsync(userId, setId);

        if (!removeResult.IsSuccess) return BadRequest(removeResult.Message);

        return Ok();
    }

    [HttpGet]
    [Route("my-sets")]
    public async Task<ActionResult> GetMySetsAsync([FromQuery] int pageNumber, int pageSize)
    {
        var token = await this.GetAccessTokenFromCookieAsync();
        _accessTokenService.GetGuidFromAccessToken(token, out var userId);

        var setResult = await _setsService.GetUserSetsAsync(userId, pageNumber, pageSize);

        if (!setResult.IsSuccess) return BadRequest(setResult.Message);

        return Ok(setResult.Value);
    }
    
    [HttpGet]
    [Route("all-sets")]
    public async Task<ActionResult> GetAllSetsAsync([FromQuery] int pageNumber, int pageSize, string? searchQuery)
    {
        var token = await this.GetAccessTokenFromCookieAsync();
        _accessTokenService.GetGuidFromAccessToken(token, out var userId);

        var setResult = await _setsService.GetAllSetsAsync(userId, pageNumber, pageSize, searchQuery ?? string.Empty);

        if (!setResult.IsSuccess) return BadRequest(setResult.Message);

        return Ok(setResult.Value);
    }
}