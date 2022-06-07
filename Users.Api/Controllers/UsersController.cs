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

    public UsersController(IUserInformationService userInformationService, IAccessTokenService accessTokenService)
    {
        _userInformationService = userInformationService;
        _accessTokenService = accessTokenService;
    }

    [HttpPatch]
    [Route("update_info")]
    public async Task<ActionResult> PatchUserInfo([FromBody] JsonPatchDocument<UpdateUserInfoRequest> updateRequest)
    {
        var token = await this.GetAccessTokenAsync();
        _accessTokenService.GetGuidFromAccessToken(token, out var userId);
        var result = await _userInformationService.UpdateUserInfoAsync(updateRequest, userId);
        
        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpPut]
    [Route("upload_avatar")]
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
    [Route("")]
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