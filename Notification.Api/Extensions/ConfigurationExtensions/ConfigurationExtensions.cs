using Notification.Api.Models;

namespace Notification.Api.Extensions.ConfigurationExtensions;

public static class ConfigurationExtensions
{
    public static EmailConfiguration GetEmailConfiguration(this IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection("SmtpConfiguration");
        var server = configurationSection.GetValue<string>("Server");
        var serverPort = configurationSection.GetValue<int>("ServerPort");
        var login = configurationSection.GetValue<string>("Login");
        var password = configurationSection.GetValue<string>("Password");
        return new EmailConfiguration(server, serverPort, login, password);
    }
}