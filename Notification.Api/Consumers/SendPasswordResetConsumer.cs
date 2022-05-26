using MassTransit;
using Messages.Contracts;
using Notification.Api.Interfaces.Services;
using Serilog;

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
        Log.Information("Received reset token: {Code} And email to send: {Email}", context.Message.ResetToken,
            context.Message.EmailAddress);
        await _emailSenderService.SendForgottenPasswordResetLinkAsync("Test", context.Message.EmailAddress,
            context.Message.ResetToken);
    }
}