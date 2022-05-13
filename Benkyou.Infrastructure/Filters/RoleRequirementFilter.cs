using Benkyou.Application.Services.Common;
using Benkyou.Application.Services.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Benkyou.Infrastructure.Filters;

public class RoleRequirementFilter : IAsyncAuthorizationFilter
{
    private readonly string _role;

    public RoleRequirementFilter(string role)
    {
        _role = role;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var accessTokenService = context.HttpContext.RequestServices.GetService<IAccessTokenService>()!;
        var userService = context.HttpContext.RequestServices.GetService<IUserService>()!;
        var token = (await context.HttpContext.GetTokenAsync("access_token"))!;
        if (accessTokenService.GetRoleFromAccessToken(token) != _role)
        {
            context.Result = new ForbidResult();
            return;
        }

        var userId = accessTokenService.GetGuidFromAccessToken(token);
        if (!(await userService.GetUserInfoAsync(userId)).IsSuccess)
        {
            context.Result = new NotFoundResult();
            return;
        }

        if ((await userService.GetUserInfoAsync(userId)).Value!.Role != _role) context.Result = new ForbidResult();
    }
}