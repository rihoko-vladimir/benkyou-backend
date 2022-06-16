using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Sets.Api.Extensions.ControllerExtensions;

public static class ControllerExtensions
{
    public static async Task<string> GetAccessTokenFromCookieAsync(this ControllerBase controllerBase)
    {
        return (await controllerBase.HttpContext.GetTokenAsync("access_token"))!;
    }

    public static string GetRefreshTokenFromCookie(this ControllerBase controllerBase)
    {
        return controllerBase.Request.Cookies["refresh"] ?? "";
    }
}