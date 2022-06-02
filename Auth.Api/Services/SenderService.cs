using Auth.Api.Interfaces.Services;
using Auth.Api.Models;
using MassTransit;
using Serilog;
using Shared.Models;

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
            Log.Information("Sending confirmation {EmailCode} to {Email} via message broker", emailCode, emailAddress);
            await endpoint.Send(new SendEmailConfirmationCode
            {
                EmailCode = emailCode,
                EmailAddress = emailAddress
            });
            return Result.Success();
        }
        catch (Exception e)
        {
            Log.Error(
                "An error occured while attempting to send confirmation code. Exception: {Type}, Message: {Message}",
                e.GetType().FullName, e.Message);
            return Result.Error();
        }
    }

    public async Task<Result> SendResetPasswordAsync(string resetToken, string emailAddress)
    {
        try
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri(QueueNames.PasswordResetQueueWithProtocol));
            Log.Information("Sending reset token {Reset} to {Email} via message broker", resetToken, emailAddress);
            await endpoint.Send(new SendEmailResetLink
            {
                ResetToken = resetToken,
                EmailAddress = emailAddress
            });
            return Result.Success();
        }
        catch (Exception e)
        {
            Log.Error(
                "An error occured while attempting to send password reset token. Exception: {Type}, Message: {Message}",
                e.GetType().FullName, e.Message);
            return Result.Error();
        }
    }
}