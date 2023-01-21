using MassTransit;
using Serilog;
using Sets.Api.Interfaces.Services;
using Shared.Models.Messages;

namespace Sets.Api.Consumers;

public class UpdateAccountVisibilityConsumer : IConsumer<UpdateUserVisibilityMessage>
{
    private readonly ISetsService _setsService;

    public UpdateAccountVisibilityConsumer(ISetsService setsService)
    {
        _setsService = setsService;
    }

    public async Task Consume(ConsumeContext<UpdateUserVisibilityMessage> context)
    {
        var message = context.Message;
        Log.Information("Received visibility change message: {UserId}, {IsVisible}", message.UserId, message.IsVisible);

        await _setsService.ChangeSetsVisibilityAsync(message.UserId, message.IsVisible);
    }
}