using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Benkyou.Domain.Extensions;

public static class ControllerExtensions
{
    public static async Task<string> GetTokenAsync(this ControllerBase controllerBase)
    {
        return (await controllerBase.HttpContext.GetTokenAsync("access_token"))!;
    }
}