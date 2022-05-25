using System.Text;
using Notification.Api.Interfaces.Generators;
using Serilog;
using Stubble.Core.Builders;

namespace Notification.Api.Generators;

public class EmailTemplateGenerator : IEmailTemplateGenerator
{
    public async Task<string> GetEmailCodeMailAsync(string userFirstName, string emailCode)
    {
        var templateFilePath = $"{Environment.CurrentDirectory}/Resources/EmailTemplates/EmailCodeTemplate.mustache";
        var dataObject = new {name = userFirstName, confirmationCode = emailCode};
        var result = await GetRenderedEmailAsync(templateFilePath, dataObject);
        return result;
    }

    public async Task<string> GetForgottenPasswordMailAsync(string userFirstName, string url)
    {
        var templateFilePath =
            $"{Environment.CurrentDirectory}/Resources/EmailTemplates/EmailForgottenPasswordTemplate.mustache";
        var dataObject = new {name = userFirstName, resetLink = url};
        var result = await GetRenderedEmailAsync(templateFilePath, dataObject);
        return result;
    }

    private async Task<string> GetRenderedEmailAsync(string filePath, object data)
    {
        var stubble = new StubbleBuilder().Build();
        Log.Debug("Rendering template from path {TemplatePath}", filePath);
        try
        {
            var streamReader = new StreamReader(filePath, Encoding.UTF8);
            var renderedString = await stubble.RenderAsync(await streamReader.ReadToEndAsync(), data);
            if (renderedString is not null) return renderedString;
            Log.Warning("Rendered template is null");
            return string.Empty;
        }
        catch (Exception e)
        {
            Log.Error("Exception {ExceptionType} with message {ExceptionMessage} was thrown while rendering",
                e.GetType().FullName, e.Message);
            return string.Empty;
        }
    }
}