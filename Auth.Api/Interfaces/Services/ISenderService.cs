using Auth.Api.Models;
using Shared.Models.Models;

namespace Auth.Api.Interfaces.Services;

public interface ISenderService
{
    public Task<Result> SendEmailCodeMessageAsync(string emailCode, string emailAddress);

    public Task<Result> SendResetPasswordMessageAsync(string resetToken, string emailAddress);
}