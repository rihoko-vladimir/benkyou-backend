using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Benkyou.Domain.Extensions;

public static class ControllerExtensions
{
    public static async Task<string> GetTokenAsync(this ControllerBase controllerBase)
    {
        return (await controllerBase.HttpContext.GetTokenAsync("access_token"))!;
    }
}