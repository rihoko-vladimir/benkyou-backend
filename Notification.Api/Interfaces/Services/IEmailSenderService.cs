using Shared.Models.Models;

namespace Notification.Api.Interfaces.Services;

public interface IEmailSenderService
{
    public Task<Result> SendAccountConfirmationCodeAsync(string userName, string emailAddress, string confirmationCode);

    public Task<Result> SendForgottenPasswordResetLinkAsync(string userName, string emailAddress,
        string passwordResetToken);
}