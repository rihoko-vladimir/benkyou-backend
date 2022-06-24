using System.ComponentModel.DataAnnotations;

namespace Auth.Api.Models.Entities;

public class Token
{
    [Key] 
    public Guid RecordId { get; set; }

    public string RefreshToken { get; set; }
    
    public Guid UserCredentialId { get; set; }
    public DateTime IssuedDateTime { get; set; } = DateTime.Now;
}