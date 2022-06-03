using MassTransit;
using Notification.Api.Interfaces.Services;
using Serilog;
using Shared.Models.Messages;

namespace Notification.Api.Consumers;

public class SendPasswordResetConsumer : IConsumer<SendEmailResetLinkMessage>
{
    private readonly IEmailSenderService _emailSenderService;

    public SendPasswordResetConsumer(IEmailSenderService emailSenderService)
    {
        _emailSenderService = emailSenderService;
    }

    public async Task Consume(ConsumeContext<SendEmailResetLinkMessage> context)
    {
        Log.Information("Received reset token: {Code} And email to send: {Email}", context.Message.ResetToken,
            context.Message.EmailAddress);

        await _emailSenderService.SendForgottenPasswordResetLinkAsync("Test", context.Message.EmailAddress,
            context.Message.ResetToken);
    }
}