namespace Notification.Api.Models;

public record MassTransitConfiguration
{
    public const string Key = "MassTransitConfiguration";
    public string Host { get; set; }
    public string VirtualHost { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string AzureServiceBusConnectionString { get; set; }
}