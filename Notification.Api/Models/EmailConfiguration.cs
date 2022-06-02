namespace Notification.Api.Models;

public record EmailConfiguration
{
    public const string Key = "EmailConfiguration";
    
    public string ApiKey { get; set; }
    
    public string Source { get; set; }
    
    public string SourceName { get; set; }
}