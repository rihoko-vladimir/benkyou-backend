using Benkyou.Application.Services.Common;
using Benkyou.Domain.Templates;
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
        var emailLogin = emailSection["Login"];
        var emailServer = emailSection["Server"];
        var emailPort = emailSection["ServerPort"];
        var emailPassword = emailSection["Password"];
        emailMessage.From.Add(new MailboxAddress("Benkyou! Bot", emailLogin));
        emailMessage.Subject = "Benkyou! Confirmation code";
        emailMessage.Body = new TextPart
        {
            Text = $"Your confirmation code is {emailConfirmationCode}"
        };
        emailMessage.To.Add(MailboxAddress.Parse(emailReceiverAddress));

        using var smtpClient = new SmtpClient();

        await smtpClient.ConnectAsync(emailServer, int.Parse(emailPort!), SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(emailLogin, emailPassword);
        await smtpClient.SendAsync(emailMessage);
        await smtpClient.DisconnectAsync(true);
    }

    public async Task SendEmailResetLinkAsync(string emailAddress, string passwordResetToken, string name)
    {
        var emailMessage = new MimeMessage();
        var emailSection = _configuration.GetSection("Email");
        var emailLogin = emailSection["Login"];
        var emailServer = emailSection["Server"];
        var emailPort = emailSection["ServerPort"];
        var emailPassword = emailSection["Password"];
        emailMessage.From.Add(new MailboxAddress("Benkyou! Bot", emailLogin));
        emailMessage.Subject = "Benkyou! Password reset";
        //TODO paste link instead of token here!
        emailMessage.XPriority = XMessagePriority.High;
        var bodyBuilder = new BodyBuilder();
        var verificationLink =
            $"https://localhost:5001/api/auth/reset-password-confirm?email={emailAddress}&token={passwordResetToken}";
        bodyBuilder.HtmlBody = new EmailTemplate().GetHtmlPage(name, verificationLink);
        emailMessage.Body = bodyBuilder.ToMessageBody();
        emailMessage.To.Add(MailboxAddress.Parse(emailAddress));
        using var smtpClient = new SmtpClient();

        await smtpClient.ConnectAsync(emailServer, int.Parse(emailPort!), SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(emailLogin, emailPassword);
        await smtpClient.SendAsync(emailMessage);
        await smtpClient.DisconnectAsync(true);
    }
}