using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Benkyou.Domain.Entities;

public class User : IdentityUser<Guid>
{
    [Key] public override Guid Id { get; set; }

    [MaxLength(20)] [Required] public string FirstName { get; set; } = null!;

    [MaxLength(35)] [Required] public string LastName { get; set; } = null!;

    [Required] public DateTime Birthday { get; set; }

    [MaxLength(350)] public string About { get; set; } = null!;

    [MaxLength(100)] public string AvatarUrl { get; set; } = null!;

    [MaxLength(35)] [Required] public override string Email { get; set; } = null!;

    public UserStatistic UserStatistic { get; set; } = null!;

    public ICollection<Card> Cards { get; set; } = null!;

    [Required] public string RefreshToken { get; set; } = null!;
}