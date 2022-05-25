using Auth.Api.Interfaces.Services;
using Auth.Api.Models;
using MassTransit;
using Messages.Contracts;

namespace Auth.Api.Services;

public class SenderService : ISenderService
{
    private readonly IBus _bus;

    public SenderService(IBus bus)
    {
        _bus = bus;
    }

    public async Task<Result> SendEmailCodeAsync(string emailCode, string emailAddress)
    {
        try
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri(QueueNames.EmailConfirmationQueueWithProtocol));
            await endpoint.Send(new SendEmailConfirmationCode
            {
                EmailCode = emailCode,
                EmailAddress = emailAddress
            });
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Error();
        }
    }

    public async Task<Result> SendResetPasswordAsync(string resetToken, string emailAddress)
    {
        try
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri(QueueNames.PasswordResetQueueWithProtocol));
            await endpoint.Send(new SendEmailResetLink
            {
                ResetToken = resetToken,
                EmailAddress = emailAddress
            });
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Error();
        }
    }
}