using System.ComponentModel.DataAnnotations;
using Fido2NetLib;
using Fido2NetLib.Development;

namespace Auth.Api.Models.Entities;

public class UserCredential
{
    [Key] public Guid Id { get; init; } = new();

    public string Email { get; init; }

    public string PasswordHash { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public bool IsAccountLocked { get; set; }

    public ICollection<Token>? Tokens { get; init; }

    [MaxLength(6)] public string? EmailConfirmationCode { get; set; }

    public List<byte[]> StoredCredentialIds { get; set; } = [];

    public List<StoredCredential> StoredCredentials { get; set; } = [];

    public Fido2User ToFidoUser()
    {
        return new Fido2User
        {
            Id = Id.ToByteArray(),
            Name = Email,
            DisplayName = Email
        };
    }
}