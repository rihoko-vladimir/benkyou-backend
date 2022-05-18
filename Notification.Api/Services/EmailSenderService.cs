using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Notification.Api.Extensions;
using Notification.Api.Interfaces.Generators;
using Notification.Api.Interfaces.Services;
using Notification.Api.Models;

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

    public async Task<Result> SendEmailAsync(MimeMessage emailMessage)
    {
        var smtpClient = new SmtpClient();
        try
        {
            await smtpClient.ConnectAsync(_emailConfiguration.Server, _emailConfiguration.Port,
                SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_emailConfiguration.Login, _emailConfiguration.Password);
            await smtpClient.SendAsync(emailMessage);
            await smtpClient.DisconnectAsync(true);
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Error(e);
        }
        finally
        {
            smtpClient.Dispose();
        }
    }

    public async Task<Result> SendAccountConfirmationCodeAsync(string userName, string emailAddress,
        int confirmationCode)
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

        message.To.Add(MailboxAddress.Parse(emailAddress));
        return await SendEmailAsync(message);
    }

    public async Task<Result> SendForgottenPasswordResetLinkAsync(string userName, string emailAddress,
        string passwordResetToken)
    {
        return await Task.FromResult(Result.Success());
    }
}