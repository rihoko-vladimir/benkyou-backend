using Auth.Api.Interfaces.Services;
using MassTransit;
using Serilog;
using Shared.Models.Messages;
using Shared.Models.Models;
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
        Log.Information("Sending confirmation {EmailCode} to {Email} via message broker", emailCode, emailAddress);

        var result = await SendMessage(new Uri($"queue:{QueueNames.EmailConfirmationQueue}"),
            new SendEmailConfirmationCodeMessage
            {
                EmailCode = emailCode,
                EmailAddress = emailAddress
            });

        return result;
    }


    public async Task<Result> SendResetPasswordMessageAsync(string resetToken, string emailAddress)
    {
        Log.Information("Sending reset token {Reset} to {Email} via message broker", resetToken, emailAddress);

        var result = await SendMessage(new Uri($"queue:{QueueNames.PasswordResetQueue}"),
            new SendEmailResetLinkMessage
            {
                ResetToken = resetToken,
                EmailAddress = emailAddress
            });

        return result;
    }

    public async Task<Result> SendRegistrationMessageAsync(Guid userId, string firstName, string lastName,
        string userName, bool isTermsAccepted)
    {
        Log.Information("Sending registered user info: {Id}, {FirstName}, {LastName}, {UserName} via message broker",
            userId, firstName, lastName, userName);

        var message = new RegisterUserMessage
        {
            FirstName = firstName,
            LastName = lastName,
            UserId = userId,
            UserName = userName,
            IsTermsAccepted = isTermsAccepted
        };

        await SendMessage(new Uri($"queue:{QueueNames.RegistrationTimeQueue}"),
            message);

        var result = await SendMessage(new Uri($"queue:{QueueNames.RegistrationQueue}"),
            message);

        return result;
    }

    private async Task<Result> SendMessage<T>(Uri endpointUri, T message)
    {
        try
        {
            var endpoint =
                await _sendEndpointProvider.GetSendEndpoint(endpointUri);

            Log.Information("Sending to {Uri}", endpointUri.ToString());

            await endpoint.Send(message!);

            return Result.Success();
        }
        catch (Exception e)
        {
            Log.Error(
                "An error occured while attempting to send a message. Exception: {Type}, Message: {Message}",
                e.GetType().FullName, e.Message);

            return Result.Error();
        }
    }
}