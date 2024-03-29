namespace Notification.Api.Interfaces.Generators;

public interface IEmailTemplateGenerator
{
    public Task<string> GetEmailCodeMailAsync(string userFirstName, string emailCode);

    public Task<string> GetForgottenPasswordMailAsync(string userFirstName, string url);
}