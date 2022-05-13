using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Benkyou.Domain.Extensions;

public static class ControllerExtensions
{
    public static async Task<string> GetTokenAsync(this ControllerBase controllerBase)
    {
        return (await controllerBase.HttpContext.GetTokenAsync("access_token"))!;
    }

    public static void SetAccessAndRefreshCookie(this ControllerBase controllerBase, string accessToken,
        string refreshToken)
    {
        var refreshCookieKey = "refresh";
        var accessCookieKey = "access";
        var accessCookieOptions = new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Unspecified
        };
        var refreshCookieOptions = new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Unspecified
        };
        controllerBase.Response.Cookies.Append(accessCookieKey, accessToken, accessCookieOptions);
        controllerBase.Response.Cookies.Append(refreshCookieKey, refreshToken, refreshCookieOptions);
    }

    public static string GetRefreshTokenFromCookie(this ControllerBase controllerBase)
    {
        return controllerBase.Request.Cookies["refresh"] ?? "";
    }
}