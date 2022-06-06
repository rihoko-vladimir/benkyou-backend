namespace Shared.Models.Models.Configurations;

public class RabbitMqConfiguration
{
    public const string Key = "RabbitMQConfiguration";
    public string Host { get; set; }
    public string VirtualHost { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}