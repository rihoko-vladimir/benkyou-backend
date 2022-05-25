using MassTransit;
using Messages.Contracts;
using Notification.Api.Interfaces.Services;

namespace Notification.Api.Consumers;

public class SendPasswordResetConsumer : IConsumer<SendEmailResetLink>
{
    private readonly IEmailSenderService _emailSenderService;

    public SendPasswordResetConsumer(IEmailSenderService emailSenderService)
    {
        _emailSenderService = emailSenderService;
    }

    public async Task Consume(ConsumeContext<SendEmailResetLink> context)
    {
        await _emailSenderService.SendForgottenPasswordResetLinkAsync("Test", context.Message.EmailAddress,
            context.Message.ResetToken);
    }
}