using Microsoft.AspNetCore.Mvc;
using Notification.Api.Interfaces.Services;

namespace Notification.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly IEmailSenderService _emailSenderService;

    public TestController(IEmailSenderService emailSenderService)
    {
        _emailSenderService = emailSenderService;
    }

    [HttpPost]
    [Route("test")]
    public async Task<ActionResult> SendTestEmail()
    {
        var result =
            await _emailSenderService.SendAccountConfirmationCodeAsync("test name", "vovakozlouskiy@gmail.com", 123456);
        if (result.IsSuccess) return Ok();
        return BadRequest(result.Exception!.Message);
    }
}