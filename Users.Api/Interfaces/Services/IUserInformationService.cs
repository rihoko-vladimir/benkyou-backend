using Microsoft.AspNetCore.JsonPatch;
using Shared.Models.Models;
using Users.Api.Models;
using Users.Api.Models.Requests;

namespace Users.Api.Interfaces.Services;

public interface IUserInformationService
{
    public Task<Result> UpdateUserInfo(JsonPatchDocument<UpdateUserInfoRequest> updateRequest, Guid userId);

    public Task<Result> UpdateUserAvatar(IFormFile file, Guid userId);
}