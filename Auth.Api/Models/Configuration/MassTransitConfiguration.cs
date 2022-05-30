namespace Auth.Api.Models.Configuration;

public record MassTransitConfiguration(
    string Type,
    string Host = "",
    string VirtualHost = "",
    string UserName = "",
    string Password = "",
    string ConnectionString = "");

public static class MassTransitType
{
    public const string RabbitMq = "RabbitMQ";
    public const string AzureServiceBus = "AzureServiceBus";
}