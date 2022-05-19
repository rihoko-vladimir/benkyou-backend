using System.Text;
using Notification.Api.Interfaces.Generators;
using Stubble.Core.Builders;

namespace Notification.Api.Generators;

public class EmailTemplateGenerator : IEmailTemplateGenerator
{
    private readonly ILogger<EmailTemplateGenerator> _logger;

    public EmailTemplateGenerator(ILogger<EmailTemplateGenerator> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetRenderedEmailAsync(string filePath, object data)
    {
        var stubble = new StubbleBuilder().Build();
        _logger.LogDebug("Rendering template from path {TemplatePath}", filePath);
        try
        {
            var streamReader = new StreamReader(filePath, Encoding.UTF8);
            var renderedString = await stubble.RenderAsync(await streamReader.ReadToEndAsync(), data);
            if (renderedString is not null) return renderedString;
            _logger.LogWarning("Rendered template is null");
            return "";
        }
        catch (Exception e)
        {
            _logger.LogCritical("Exception {ExceptionType} with message {ExceptionMessage} was thrown while rendering",
                e.GetType().FullName, e.Message);
            return "";
        }
    }

    public async Task<string> GetEmailCodeMailAsync(string userFirstName, int emailCode)
    {
        var templateFilePath = $"{Environment.CurrentDirectory}/EmailTemplates/EmailCodeTemplate.mustache";
        var dataObject = new {name = userFirstName, confirmationCode = emailCode};
        var result = await GetRenderedEmailAsync(templateFilePath, dataObject);
        return result;
    }

    public async Task<string> GetForgottenPasswordMailAsync(string userFirstName, string url)
    {
        var templateFilePath = $"{Environment.CurrentDirectory}/EmailTemplates/EmailForgottenPasswordTemplate.mustache";
        var dataObject = new {name = userFirstName, resetLink = url};
        var result = await GetRenderedEmailAsync(templateFilePath, dataObject);
        return result;
    }
}