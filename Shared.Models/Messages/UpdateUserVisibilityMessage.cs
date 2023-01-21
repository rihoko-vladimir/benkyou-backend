namespace Shared.Models.Messages;

public class UpdateUserVisibilityMessage
{
    public Guid UserId { get; set; }

    public bool IsVisible { get; set; }
}