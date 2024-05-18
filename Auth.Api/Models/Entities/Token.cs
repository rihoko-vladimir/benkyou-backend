using System.ComponentModel.DataAnnotations;

namespace Auth.Api.Models.Entities;

public class Token
{
    [Key] public Guid RecordId { get; init; }

    public string RefreshToken { get; init; }

    public Guid UserCredentialId { get; init; }
    public DateTime IssuedDateTime { get; init; } = DateTime.Now;
}