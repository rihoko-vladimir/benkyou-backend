using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Users.Api.Extensions.ControllerExtensions;
using Users.Api.Interfaces.Services;
using Users.Api.Models.Requests;

namespace Users.Api.Controllers;

[Authorize]
[ApiController]
[Route("/api/v{version:apiVersion}/user")]
public class UsersController : ControllerBase
{
    private readonly IUserInformationService _userInformationService;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IValidator<UpdateUserInfoRequest> _userInfoValidator;
    private readonly IMapper _mapper;

    public UsersController(IUserInformationService userInformationService, 
        IAccessTokenService accessTokenService,
        IValidator<UpdateUserInfoRequest> userInfoValidator,
        IMapper mapper)
    {
        _userInformationService = userInformationService;
        _accessTokenService = accessTokenService;
        _userInfoValidator = userInfoValidator;
        _mapper = mapper;
    }

    [HttpPatch]
    [Route("update-info")]
    public async Task<ActionResult> PatchUserInfo([FromBody] JsonPatchDocument<UpdateUserInfoRequest> updateRequest)
    {
        var token = await this.GetAccessTokenAsync();
        _accessTokenService.GetGuidFromAccessToken(token, out var userId);

        var userResult = await _userInformationService.GetUserInformation(userId);
        if (!userResult.IsSuccess) return BadRequest(userResult.Message);
        
        var updateDto = _mapper.Map<UpdateUserInfoRequest>(userResult.Value);
        updateRequest.ApplyTo(updateDto);
        var validationResult = await _userInfoValidator.ValidateAsync(updateDto);
        
        if (!validationResult.IsValid) return BadRequest(validationResult.ToString("~"));
        
        var result = await _userInformationService.UpdateUserInfoAsync(updateRequest, userId);
        
        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(result.Message);
    }

    [HttpPut]
    [Route("upload-avatar")]
    public async Task<ActionResult> UploadUserAvatar(IFormFile formFile)
    {
        var token = await this.GetAccessTokenAsync();
        _accessTokenService.GetGuidFromAccessToken(token, out var userId);
        var result = await _userInformationService.UpdateUserAvatarAsync(formFile, userId);
        
        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpGet]
    [Route("get-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        var token = await this.GetAccessTokenAsync();
        _accessTokenService.GetGuidFromAccessToken(token, out var userId);
        var result = await _userInformationService.GetUserInformation(userId);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest();
    }
}