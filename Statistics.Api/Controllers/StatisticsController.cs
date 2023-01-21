using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Statistics.Api.Extensions.ControllerExtensions;
using Statistics.Api.Interfaces.Services;

namespace Statistics.Api.Controllers;

[ApiController]
[Authorize]
[Route("/api/v{version:apiVersion}/statistics")]
public class StatisticsController : ControllerBase
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService, IAccessTokenService accessTokenService)
    {
        _statisticsService = statisticsService;
        _accessTokenService = accessTokenService;
    }

    [HttpGet]
    [Route("set-results")]
    public async Task<ActionResult> GetSetResults([FromQuery] string setId)
    {
        var token = await this.GetAccessTokenAsync();
        _accessTokenService.GetGuidFromAccessToken(token, out var userId);

        if (!Guid.TryParse(setId, out var id))
            return BadRequest("Incorrect id provided");

        var result = await _statisticsService.GetSetStudyResults(userId, id);

        if (!result.IsSuccess) return BadRequest(result.Message);

        return Ok(result.Value);
    }

    [HttpGet]
    [Route("user-stats")]
    public async Task<ActionResult> GetUserStats()
    {
        var token = await this.GetAccessTokenAsync();

        _accessTokenService.GetGuidFromAccessToken(token, out var userId);

        var result = await _statisticsService.GetGeneralStatistics(userId);

        if (!result.IsSuccess) return BadRequest(result.Message);

        return Ok(result.Value);
    }

    [HttpPost]
    [Route("last-online")]
    public async Task<ActionResult> SetLastOnlineStatus()
    {
        var token = await this.GetAccessTokenAsync();

        _accessTokenService.GetGuidFromAccessToken(token, out var userId);

        var result = await _statisticsService.SetLastOnlineStatus(userId);

        if (!result.IsSuccess) return BadRequest(result.Message);

        return Ok();
    }
}