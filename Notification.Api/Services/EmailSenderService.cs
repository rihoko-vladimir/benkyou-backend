using Notification.Api.Interfaces.Generators;
using Notification.Api.Interfaces.Services;
using Notification.Api.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;
using Shared.Models.Models;

namespace Notification.Api.Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly EmailConfiguration _emailConfiguration;
    private readonly IEmailTemplateGenerator _templateGenerator;

    public EmailSenderService(EmailConfiguration emailConfiguration, IEmailTemplateGenerator templateGenerator)
    {
        _emailConfiguration = emailConfiguration;
        _templateGenerator = templateGenerator;
    }

    public async Task<Result> SendAccountConfirmationCodeAsync(string userName, string emailAddress,
        string confirmationCode)
    {
        var from = new EmailAddress(_emailConfiguration.Source, _emailConfiguration.SourceName);
        var subject = "Benkyou! Confirmation code";
        var to = new EmailAddress(emailAddress, userName);
        var mailString = await _templateGenerator.GetEmailCodeMailAsync(userName, confirmationCode);
        var message = new SendGridMessage
        {
            From = from,
            Subject = subject,
            HtmlContent = mailString
        };
        message.AddTo(to);

        Log.Information("Sending confirmation code {ConfirmationCode} to {Destination}", confirmationCode,
            emailAddress);

        return await SendEmailAsync(message);
    }

    public async Task<Result> SendForgottenPasswordResetLinkAsync(string userName, string emailAddress,
        string passwordResetToken)
    {
        var from = new EmailAddress(_emailConfiguration.Source, _emailConfiguration.SourceName);
        var subject = "Benkyou! Password reset";
        var to = new EmailAddress(emailAddress, userName);
        var mailString =
            await _templateGenerator.GetForgottenPasswordMailAsync(userName,
                $"http://localhost:4200/auth/forgot-password/new-password?token={passwordResetToken}&email={emailAddress}");
        var message = new SendGridMessage
        {
            From = from,
            Subject = subject,
            HtmlContent = mailString
        };
        message.AddTo(to);

        Log.Information("Sending reset link with token {Token} to {Destination}", passwordResetToken,
            emailAddress);

        return await SendEmailAsync(message);
    }

    private async Task<Result> SendEmailAsync(SendGridMessage emailMessage)
    {
        try
        {
            var client = new SendGridClient(_emailConfiguration.ApiKey);

            var result = await client.SendEmailAsync(emailMessage);

            Log.Debug("Sending email message: {Message}", emailMessage.ToString());

            Log.Information("Sent successfully");

            return result.IsSuccessStatusCode ? Result.Success() : Result.Error();
        }
        catch (Exception e)
        {
            Log.Error("An exception {Type} was thrown while trying to send an email message: {EmailMessage}",
                e.GetType().FullName, emailMessage);

            return Result.Error(e);
        }
    }
}