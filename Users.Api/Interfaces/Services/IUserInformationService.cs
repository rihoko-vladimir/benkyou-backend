using Microsoft.AspNetCore.JsonPatch;
using Shared.Models.Models;
using Users.Api.Models;
using Users.Api.Models.Entities;
using Users.Api.Models.Requests;

namespace Users.Api.Interfaces.Services;

public interface IUserInformationService
{
    public Task<Result> UpdateUserInfoAsync(JsonPatchDocument<UpdateUserInfoRequest> updateRequest, Guid userId);

    public Task<Result> UpdateUserAvatarAsync(IFormFile file, Guid userId);

    public Task<Result> CreateUserAsync(UserInformation userInformation);
}