using Auth.Api.Models;

namespace Auth.Api.TokenProviders;

public class EmailCodeTokenProvider
{
    public string Generate()
    {
        const string chars = "0123456789";
        var random = new Random();
        var randomCode = new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        return randomCode;
    }

    public bool Validate(string token, User user)
    {
        var userCode = user.EmailConfirmationCode;
        return token.Equals(userCode);
    }
}