using Auth.Api.Interfaces.Generators;
using Auth.Api.Models.Entities;

namespace Auth.Api.Generators;

public class EmailCodeGenerator : IEmailCodeGenerator
{
    public string GenerateCode()
    {
        const string chars = "0123456789";
        var random = new Random();
        var randomCode = new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        return randomCode;
    }

    public bool VerifyCode(string token, User user)
    {
        var userCode = user.EmailConfirmationCode;
        return token.Equals(userCode);
    }
}