using System.ComponentModel.DataAnnotations;

namespace Auth.Api.Models;

public class User
{
    [Key] public Guid Id { get; set; } = new();

    public string Email { get; set; }

    public string PasswordHash { get; set; }
    
    public bool IsEmailConfirmed { get; set; }

    [MaxLength(6)] public string? EmailConfirmationCode { get; set; }
}