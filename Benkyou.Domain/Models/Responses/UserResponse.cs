namespace Benkyou.Domain.Models.Responses;

public class UserResponse
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime? Birthday { get; set; }

    public string? About { get; set; } = null!;

    public string? AvatarUrl { get; set; } = null!;

    public string Email { get; set; } = null!;
}