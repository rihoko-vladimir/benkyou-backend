using MassTransit;
using Serilog;
using Shared.Models.Messages;
using Statistics.Api.Interfaces.Services;

namespace Statistics.Api.Consumers;

public class UserRegisterMessageConsumer : IConsumer<RegisterUserMessage>
{
    private readonly IStatisticsService _statisticsService;

    public UserRegisterMessageConsumer(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public async Task Consume(ConsumeContext<RegisterUserMessage> context)
    {
        try
        {
            var message = context.Message;
            var currentDateTime = DateTime.Now;

            Log.Information("Received user {UserId} registration message with date {RegistrationDate}", message.UserId,
                currentDateTime);

            await _statisticsService.SetRegistrationDate(message.UserId, currentDateTime);
        }
        catch (Exception)
        {
            Log.Error("An error occured while processing registration message for user {UserId}",
                context.Message.UserId);
        }
    }
}