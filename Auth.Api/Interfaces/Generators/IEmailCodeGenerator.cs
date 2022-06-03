using Auth.Api.Models.Entities;

namespace Auth.Api.Interfaces.Generators;

public interface IEmailCodeGenerator
{
    public string GenerateCode();

    public bool VerifyCode(string token, UserCredential userCredential);
}