using Shared.Models.Models;
using Users.Api.Models;

namespace Users.Api.Interfaces.Repositories;

public interface IUserInfoRepository
{
    public Task UpdateUserInfoAsync(UserInformation userInformation);

    public Task<UserInformation> GetUserInfoAsync(Guid userId);

    public Task CreateUserAsync(UserInformation userInformation);
}