using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Users.Api.Extensions.ControllerExtensions;

public static class ControllerExtensions
{
    public static async Task<string> GetAccessTokenAsync(this ControllerBase controllerBase)
    {
        return (await controllerBase.HttpContext.GetTokenAsync("access_token"))!;
    }
}