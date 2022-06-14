namespace Users.Api.Models.Configurations;

public class AzureBlobConfiguration
{
    public const string Key = "AzureBlobConfiguration";
    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
}