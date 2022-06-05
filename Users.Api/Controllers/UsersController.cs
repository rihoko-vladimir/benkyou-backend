using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Users.Api.Extensions.ControllerExtensions;
using Users.Api.Interfaces.Services;
using Users.Api.Models.Requests;

namespace Users.Api.Controllers;

[Authorize]
[ApiController]
[Route("/api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly IUserInformationService _userInformationService;

    public UsersController(IUserInformationService userInformationService)
    {
        _userInformationService = userInformationService;
    }

    [HttpPatch]
    [Route("update_info")]
    public async Task<ActionResult> PatchUserInfo([FromBody] JsonPatchDocument<UpdateUserInfoRequest> updateRequest)
    {
        var userId = await this.GetAccessTokenAsync();
        var result = await _userInformationService.UpdateUserInfo(updateRequest, Guid.Parse(userId));
        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest();
    }
}