using MassTransit;
using Messages.Contracts;
using Notification.Api.Interfaces.Services;

namespace Notification.Api.Consumers;

public class SendEmailCodeConsumer : IConsumer<SendEmailConfirmationCode>
{
    private readonly IEmailSenderService _emailSenderService;

    public SendEmailCodeConsumer(IEmailSenderService emailSenderService)
    {
        _emailSenderService = emailSenderService;
    }

    public async Task Consume(ConsumeContext<SendEmailConfirmationCode> context)
    {
        await _emailSenderService.SendAccountConfirmationCodeAsync("Test", context.Message.EmailAddress,
            context.Message.EmailCode);
    }
}