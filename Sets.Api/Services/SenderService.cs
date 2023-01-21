using MassTransit;
using Serilog;
using Sets.Api.Interfaces.Services;
using Shared.Models.Messages;
using Shared.Models.Models;
using Shared.Models.QueueNames;

namespace Sets.Api.Services;

public class SenderService : ISenderService
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public SenderService(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task<Result> SendLearnFinishResultMessage(Guid userId, Guid setId, DateTime startDateTime,
        DateTime endDateTime, KanjiResult[] kanjiResults)
    {
        Log.Information("Sending set result message: {SetId}, {UserId} via message broker", setId, userId);

        try
        {
            var endpointUri = new Uri($"queue:{QueueNames.FinishSetLearningQueue}");
            var endpoint =
                await _sendEndpointProvider.GetSendEndpoint(endpointUri);

            var message = new FinishLearningMessage
            {
                SetId = setId,
                UserId = userId,
                EndDateTime = endDateTime,
                StartDateTime = startDateTime,
                KanjiResults = kanjiResults
            };

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