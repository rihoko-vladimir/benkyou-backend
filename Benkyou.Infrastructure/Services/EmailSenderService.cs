using Benkyou.Application.Services.Common;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Benkyou.Infrastructure.Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly IConfiguration _configuration;

    public EmailSenderService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailConfirmationCodeAsync(string emailConfirmationCode, string emailReceiverAddress)
    {
        var emailMessage = new MimeMessage();
        var emailSection = _configuration.GetSection("Email");
        var emailAddress = emailSection["Login"];
        var emailServer = emailSection["Server"];
        var emailPort = emailSection["ServerPort"];
        var emailPassword = emailSection["Password"];
        emailMessage.From.Add(new MailboxAddress("Benkyou! Bot", emailAddress));
        emailMessage.Subject = "Benkyou! Confirmation code";
        emailMessage.Body = new TextPart
        {
            Text = $"Your confirmation code is {emailConfirmationCode}"
        };
        emailMessage.To.Add(MailboxAddress.Parse(emailReceiverAddress));

        using var smtpClient = new SmtpClient();

        await smtpClient.ConnectAsync(emailServer, int.Parse(emailPort!), SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(emailAddress, emailPassword);
        await smtpClient.SendAsync(emailMessage);
        await smtpClient.DisconnectAsync(true);
    }
}