namespace Auth.Api.Models.Configuration;

public class RabbitMQConfiguration
{
    public const string Key = "RabbitMQConfiguration";
    public string Host { get; set; }
    public string VirtualHost { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}