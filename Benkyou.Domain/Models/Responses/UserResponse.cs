namespace Benkyou.Domain.Models.Responses;

public class UserResponse
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Birthday { get; set; } = null!;

    public string? About { get; set; }

    public string? AvatarUrl { get; set; }

    public string Email { get; set; } = null!;
    public bool IsPublic { get; set; }
    public string Role { get; set; }
}