using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Notification.Api.Extensions.ConfigurationExtensions;
using Notification.Api.Interfaces.Generators;
using Notification.Api.Interfaces.Services;
using Notification.Api.Models;
using Serilog;

namespace Notification.Api.Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly EmailConfiguration _emailConfiguration;
    private readonly IEmailTemplateGenerator _templateGenerator;

    public EmailSenderService(IConfiguration configuration, IEmailTemplateGenerator templateGenerator)
    {
        _emailConfiguration = configuration.GetEmailConfiguration();
        _templateGenerator = templateGenerator;
    }

    public async Task<Result> SendAccountConfirmationCodeAsync(string userName, string emailAddress,
        string confirmationCode)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Benkyou! Bot", "botbenkyou@gmail.com"));
        message.Subject = "Benkyou! Confirmation code";
        var mailString = await _templateGenerator.GetEmailCodeMailAsync(userName, confirmationCode);
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = mailString
        };
        message.Body = bodyBuilder.ToMessageBody();
        message.XPriority = XMessagePriority.High;
        message.To.Add(MailboxAddress.Parse(emailAddress));

        Log.Information("Sending confirmation code {ConfirmationCode} to {Destination}", confirmationCode,
            emailAddress);

        return await SendEmailAsync(message);
    }

    public async Task<Result> SendForgottenPasswordResetLinkAsync(string userName, string emailAddress,
        string passwordResetToken)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Benkyou! Bot", "botbenkyou@gmail.com"));
        message.Subject = "Benkyou! Password reset";
        var mailString =
            await _templateGenerator.GetForgottenPasswordMailAsync(userName, $"https://okok.ok/{passwordResetToken}");
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = mailString
        };
        message.Body = bodyBuilder.ToMessageBody();
        message.XPriority = XMessagePriority.High;
        message.To.Add(MailboxAddress.Parse(emailAddress));

        Log.Information("Sending reset link with token {Token} to {Destination}", passwordResetToken,
            emailAddress);

        return await SendEmailAsync(message);
    }

    private async Task<Result> SendEmailAsync(MimeMessage emailMessage)
    {
        try
        {
            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(_emailConfiguration.Server, _emailConfiguration.ServerPort,
                SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_emailConfiguration.Login, _emailConfiguration.Password);
            Log.Debug("Sending email message: {Message}", emailMessage.ToString());

            await smtpClient.SendAsync(emailMessage);
            await smtpClient.DisconnectAsync(true);

            Log.Information("Sent successfully");

            return Result.Success();
        }
        catch (Exception e)
        {
            Log.Error("An exception {Type} was thrown while trying to send an email message: {EmailMessage}",
                e.GetType().FullName, emailMessage);

            return Result.Error(e);
        }
    }
}