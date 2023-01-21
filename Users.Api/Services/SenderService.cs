using MassTransit;
using Serilog;
using Shared.Models.Messages;
using Shared.Models.Models;
using Shared.Models.QueueNames;
using Users.Api.Interfaces.Services;

namespace Users.Api.Services;

public class SenderService : ISenderService
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public SenderService(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task<Result> SendUpdateVisibilityMessage(Guid userId, bool isVisible)
    {
        try
        {
            var endpoint =
                await _sendEndpointProvider.GetSendEndpoint(
                    new Uri($"queue:{QueueNames.AccountVisibilityChangeQueue}"));

            var message = new UpdateUserVisibilityMessage
            {
                IsVisible = isVisible,
                UserId = userId
            };

            Log.Information("Sending to {Uri}", endpoint.ToString());

            await endpoint.Send(message);

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