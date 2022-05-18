namespace Notification.Api.Interfaces.Generators;

public interface IEmailTemplateGenerator
{
    public Task<string> GetRenderedEmailAsync(string filePath, object data);

    public Task<string> GetEmailCodeMailAsync(string userFirstName, int emailCode);

    public Task<string> GetForgottenPasswordMailAsync(string userFirstName, string url);
}