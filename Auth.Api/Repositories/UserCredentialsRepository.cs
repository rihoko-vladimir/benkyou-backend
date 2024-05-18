using Auth.Api.Interfaces.Repositories;
using Auth.Api.Models.DbContext;
using Auth.Api.Models.Entities;
using Fido2NetLib.Development;
using Fido2NetLib.Objects;
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
        var user = await _applicationContext.UserCredentials
            .Include(user => user.Tokens)
            .FirstOrDefaultAsync(user => user.Email.Equals(email));

        return user;
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
        var user = await _applicationContext.UserCredentials
            .Include(user1 => user1.Tokens)
            .FirstAsync(user1 => user1.Id == id);

        return user;
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
            .FirstOrDefaultAsync(user => user.Email.Equals(email));

        return user is not null;
    }

    public async Task<List<PublicKeyCredentialDescriptor>> GetUserCredentialsAsync(Guid userId)
    {
        var user = await _applicationContext.UserCredentials
            .Include(userCredential => userCredential.StoredCredentials)!
            .ThenInclude(storedCredential => storedCredential.Descriptor)
            .FirstOrDefaultAsync(user => user.Id.Equals(userId));

        return user?.StoredCredentials.Select(credential => credential.Descriptor).ToList() ?? [];
    }

    public async Task<StoredCredential> GetCredentialById(Guid userid, byte[] id)
    {
        var user = await _applicationContext.UserCredentials
            .Include(userCredential => userCredential.StoredCredentials)!
            .ThenInclude(storedCredential => storedCredential.Descriptor)
            .FirstAsync(user => user.Id.Equals(userid));

        return user.StoredCredentials.First(credential => credential.UserId.Equals(id));
    }

    public async Task AddCredentialToUserAsync(Guid userId, StoredCredential storedCredential)
    {
        (await _applicationContext.UserCredentials.FirstAsync(credential => credential.Id.Equals(userId))).StoredCredentials.Add(storedCredential);
        (await _applicationContext.UserCredentials.FirstAsync(credential => credential.Id.Equals(userId))).StoredCredentialIds.Add(storedCredential.UserId);
        
        await _applicationContext.SaveChangesAsync();
    }

    public async Task<List<UserCredential>> GetUsersByCredentialIdAsync(byte[] argsCredentialId)
    {
        var users = await _applicationContext.UserCredentials
            .Include(userCredential => userCredential.StoredCredentials)!
            .ThenInclude(storedCredential => storedCredential.Descriptor)
            .Where(user => user.StoredCredentialIds.Contains(argsCredentialId))
            .ToListAsync();

        return users;
    }

    public async Task<List<StoredCredential>> GetCredentialsByUserHandleAsync(Guid userId, byte[] argsUserHandle)
    {
        var user = await _applicationContext.UserCredentials
            .Include(userCredential => userCredential.StoredCredentials)!
            .ThenInclude(storedCredential => storedCredential.Descriptor)
            .FirstAsync(user => user.StoredCredentials.Any(credential => credential.UserHandle.Equals(argsUserHandle)));

        return user.StoredCredentials.ToList();
    }
}