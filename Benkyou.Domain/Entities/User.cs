using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Benkyou.Domain.Entities;

public class User : IdentityUser<Guid>
{
    [Key] public override Guid Id { get; set; }

    [MaxLength(20)]
    [Required(ErrorMessage = "First Name must be set")]
    public string FirstName { get; set; } = null!;

    [MaxLength(35)]
    [Required(ErrorMessage = "Last Name must be set")]
    public string LastName { get; set; } = null!;

    public DateTime? Birthday { get; set; }

    [MaxLength(350)] public string? About { get; set; }

    public string? AvatarUrl { get; set; }

    [MaxLength(35)]
    [Required(ErrorMessage = "Email address must be set")]
    public override string Email { get; set; } = null!;

    public UserStatistic UserStatistic { get; set; } = null!;

    public ICollection<Set>? Cards { get; set; }
    
    public ICollection<Report>? Reports { get; set; }

    public string? RefreshToken { get; set; }

    [Required(ErrorMessage = "Role must be set")]
    public string Role { get; set; } = null!;

    [Required(ErrorMessage = "No terms accepted value")]
    public bool IsTermsAccepted { get; set; }

    public bool IsAccountPublic { get; set; }
    public string? EmailConfirmationCode { get; set; }
}