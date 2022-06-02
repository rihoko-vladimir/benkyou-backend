using Auth.Api.Models.Entities;

namespace Auth.Api.Interfaces.Repositories;

public interface IUserCredentialsRepository
{
    public Task<UserCredential> GetUserByEmailAsync(string email);

    public Task UpdateUserAsync(UserCredential userCredential);

    public Task CreateUserCredentialAsync(UserCredential userCredential);

    public Task<UserCredential> GetUserByIdAsync(Guid id);

    public Task<bool> IsUserExistsByIdAsync(Guid userId);

    public Task<bool> IsUserExistsByEmailAsync(string email);
}