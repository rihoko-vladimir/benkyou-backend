using MassTransit;
using Shared.Models.Messages;

namespace Users.Api.Consumers;

public class RegisterUserMessageConsumer : IConsumer<RegisterUserMessage>
{
    public RegisterUserMessageConsumer()
    {
    }

    public async Task Consume(ConsumeContext<RegisterUserMessage> context)
    {
        
    }
}