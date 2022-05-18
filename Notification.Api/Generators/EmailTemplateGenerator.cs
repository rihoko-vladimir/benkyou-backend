using System.Text;
using Notification.Api.Interfaces.Generators;
using Stubble.Core.Builders;

namespace Notification.Api.Generators;

public class EmailTemplateGenerator : IEmailTemplateGenerator
{
    public async Task<string> GetRenderedEmailAsync(string filePath, object data)
    {
        var stubble = new StubbleBuilder().Build();
        using var streamReader = new StreamReader(filePath, Encoding.UTF8);
        return await stubble.RenderAsync(await streamReader.ReadToEndAsync(), data);
    }

    public async Task<string> GetEmailCodeMailAsync(string userFirstName, int emailCode)
    {
        var templateFile = $"{Environment.CurrentDirectory}/EmailTemplates/EmailCodeTemplate.mustache";
        var dataObject = new {name = userFirstName, confirmationCode = emailCode};
        var result = await GetRenderedEmailAsync(templateFile, dataObject);
        return result;
    }

    public async Task<string> GetForgottenPasswordMailAsync(string userFirstName, string url)
    {
        var templateFile = $"{Environment.CurrentDirectory}/EmailTemplates/EmailForgottenPasswordTemplate.mustache";
        var dataObject = new {name = userFirstName, resetLink = url};
        var result = await GetRenderedEmailAsync(templateFile, dataObject);
        return result;
    }
}