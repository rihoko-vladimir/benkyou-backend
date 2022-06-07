using Users.Api.Models.Entities;
using Users.Api.Models.Requests;

namespace Users.Api.Interfaces.Repositories;

public interface IUserInfoRepository
{
    public Task UpdateUserInfoAsync(UpdateUserInfoRequest userInformation, Guid id);

    public Task<UserInformation> GetUserInfoAsync(Guid userId);

    public Task CreateUserAsync(UserInformation userInformation);

    public Task UpdateUserAvatarUrl(string avatarUrl, Guid userId);
}