using Benkyou.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Benkyou.Infrastructure.TokenProviders;

public class EmailCodeTokenProvider : IUserTwoFactorTokenProvider<User>
{
    public async Task<string> GenerateAsync(string purpose, UserManager<User> manager, User user)
    {
        if (purpose != UserManager<User>.ConfirmEmailTokenPurpose)
            throw new ArgumentException("This provider can only generate email codes");
        const string chars = "0123456789";
        var random = new Random();
        var randomCode = new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        user.EmailConfirmationCode = randomCode;
        await manager.UpdateAsync(user);
        return randomCode;
    }

    public async Task<bool> ValidateAsync(string purpose, string token, UserManager<User> manager, User user)
    {
        if (purpose != UserManager<User>.ConfirmEmailTokenPurpose)
            throw new ArgumentException("This provider can only verify email codes");
        var userCode = user.EmailConfirmationCode;
        if (!token.Equals(userCode)) return false;
        user.EmailConfirmationCode = null;
        user.EmailConfirmed = true;
        user.LockoutEnabled = false;
        await manager.UpdateAsync(user);
        return true;
    }

    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<User> manager, User user)
    {
        return Task.FromResult(false);
    }
}