using System.Threading.Tasks;
using AutoMapper;
using Benkyou.Application.Services.Common;
using Benkyou.Domain.Entities;
using Benkyou.Domain.Enums;
using Benkyou.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Benkyou_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly IMapper _mapper;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly UserManager<User> _userManager;

    public AuthController(UserManager<User> userManager, IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService, IMapper mapper)
    {
        _userManager = userManager;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
    {
        return Ok();
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] RegisterModel registerModel)
    {
        var user = _mapper.Map<RegisterModel, User>(registerModel);
        //var result = await _userManager.CreateAsync(user, registerModel.Password);
        //if (!result.Succeeded) return BadRequest();
        user.FirstName = "vladimir";
        user.Role = Roles.Administrator;
        var accessToken = _accessTokenService.GetToken(user);
        var refreshToken = _refreshTokenService.GetToken(user);
        return Ok(new TokensResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
}