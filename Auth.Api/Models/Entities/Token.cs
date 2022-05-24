namespace Auth.Api.Models.Entities;

public class Token
{
    public string RefreshToken = null!;
    public Guid UserId { get; set; }

    public DateTime IssuedDateTime { get; set; } = DateTime.Now;
}