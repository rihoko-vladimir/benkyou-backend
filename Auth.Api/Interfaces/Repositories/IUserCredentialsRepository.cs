using Auth.Api.Models.Entities;
using Fido2NetLib.Development;
using Fido2NetLib.Objects;

namespace Auth.Api.Interfaces.Repositories;

public interface IUserCredentialsRepository
{
    public Task<UserCredential> GetUserByEmailAsync(string email);

    public Task UpdateUserAsync(UserCredential userCredential);

    public Task CreateUserCredentialAsync(UserCredential userCredential);

    public Task<UserCredential> GetUserByIdAsync(Guid id);

    public Task<bool> IsUserExistsByIdAsync(Guid userId);

    public Task<bool> IsUserExistsByEmailAsync(string email);

    public Task<List<PublicKeyCredentialDescriptor>> GetUserCredentialsAsync(Guid userId);

    public Task<StoredCredential> GetCredentialById(Guid userid, byte[] clientHandle);

    public Task AddCredentialToUserAsync(Guid userId, StoredCredential storedCredential);
    Task<List<UserCredential>> GetUsersByCredentialIdAsync(byte[] argsCredentialId);
    Task<List<StoredCredential>> GetCredentialsByUserHandleAsync(Guid userId, byte[] argsUserHandle);
}