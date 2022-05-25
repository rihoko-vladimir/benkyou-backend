using Auth.Api.Models;

namespace Auth.Api.Interfaces.Services;

public interface ISenderService
{
    public Task<Result> SendEmailCodeAsync(string emailCode, string emailAddress);

    public Task<Result> SendResetPasswordAsync(string resetToken, string emailAddress);
}