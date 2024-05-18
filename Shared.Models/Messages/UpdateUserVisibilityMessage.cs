namespace Shared.Models.Messages;

public class UpdateUserVisibilityMessage
{
    public Guid UserId { get; init; }

    public bool IsVisible { get; init; }
}