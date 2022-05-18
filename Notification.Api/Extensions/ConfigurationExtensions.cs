using Notification.Api.Models;

namespace Notification.Api.Extensions;

public static class ConfigurationExtensions
{
    public static EmailConfiguration GetEmailConfiguration(this IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection("Email");
        var server = configurationSection["Server"];
        var serverPort = int.Parse(configurationSection["ServerPort"]);
        var login = configurationSection["Login"];
        var password = configurationSection["Password"];
        return new EmailConfiguration(server, serverPort, login, password);
    }
}