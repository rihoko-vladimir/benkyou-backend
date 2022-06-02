using Auth.Api.Interfaces.Repositories;
using Auth.Api.Models.DbContext;
using Auth.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Api.Repositories;

public class UserCredentialsRepository : IUserCredentialsRepository
{
    private readonly ApplicationContext _applicationContext;

    public UserCredentialsRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<UserCredential> GetUserByEmailAsync(string email)
    {
        return (await _applicationContext.UserCredentials
            .Include(user => user.Tokens)
            .FirstOrDefaultAsync(user => user.Email == email))!;
    }

    public async Task UpdateUserAsync(UserCredential userCredential)
    {
        _applicationContext.UserCredentials.Update(userCredential);
        await _applicationContext.SaveChangesAsync();
    }

    public async Task CreateUserCredentialAsync(UserCredential userCredential)
    {
        await _applicationContext.UserCredentials.AddAsync(userCredential);
        await _applicationContext.SaveChangesAsync();
    }

    public async Task<UserCredential> GetUserByIdAsync(Guid id)
    {
        return await _applicationContext.UserCredentials
            .Include(user1 => user1.Tokens)
            .FirstAsync(user1 => user1.Id == id);
    }

    public async Task<bool> IsUserExistsByIdAsync(Guid userId)
    {
        var user = await _applicationContext.UserCredentials
            .FirstOrDefaultAsync(user => user.Id == userId);

        return user is not null;
    }

    public async Task<bool> IsUserExistsByEmailAsync(string email)
    {
        var user = await _applicationContext.UserCredentials
            .FirstOrDefaultAsync(user => user.Email == email);

        return user is not null;
    }
}