using System.ComponentModel.DataAnnotations;

namespace Auth.Api.Models.Entities;

public class UserCredential
{
    [Key] 
    public Guid Id { get; set; } = new();

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public ICollection<Token> Tokens { get; set; }

    [MaxLength(6)] 
    public string EmailConfirmationCode { get; set; }
}