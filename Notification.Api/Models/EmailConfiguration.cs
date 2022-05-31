namespace Notification.Api.Models;

public record EmailConfiguration
{
    public string Server { get; set; }
    public int ServerPort { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}