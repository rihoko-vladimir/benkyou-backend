namespace Notification.Api.Models;

public record EmailConfiguration(string Server, int Port, string Login, string Password);