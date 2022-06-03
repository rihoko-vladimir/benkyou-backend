using Auth.Api.Interfaces.Services;
using Auth.Api.Models;
using MassTransit;
using Serilog;
using Shared.Models.Messages;
using Shared.Models.QueueNames;

namespace Auth.Api.Services;

public class SenderService : ISenderService
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public SenderService(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task<Result> SendEmailCodeMessageAsync(string emailCode, string emailAddress)
    {
        try
        {
            var endpoint =
                await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{QueueNames.EmailConfirmationQueue}"));

            Log.Information("Sending confirmation {EmailCode} to {Email} via message broker", emailCode, emailAddress);

            await endpoint.Send(new SendEmailConfirmationCodeMessage
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

    public async Task<Result> SendResetPasswordMessageAsync(string resetToken, string emailAddress)
    {
        try
        {
            var endpoint =
                await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{QueueNames.PasswordResetQueue}"));

            Log.Information("Sending reset token {Reset} to {Email} via message broker", resetToken, emailAddress);

            await endpoint.Send(new SendEmailResetLinkMessage
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