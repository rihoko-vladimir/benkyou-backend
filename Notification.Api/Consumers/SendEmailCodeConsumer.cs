using MassTransit;
using Notification.Api.Interfaces.Services;
using Serilog;
using Shared.Models;

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
        Log.Information("Received confirmation code: {Code} And email to send: {Email}", context.Message.EmailCode,
            context.Message.EmailAddress);
        await _emailSenderService.SendAccountConfirmationCodeAsync("Test", context.Message.EmailAddress,
            context.Message.EmailCode);
    }
}