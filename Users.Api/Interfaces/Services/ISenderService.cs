using Shared.Models.Models;

namespace Users.Api.Interfaces.Services;

public interface ISenderService
{
    public Task<Result> SendUpdateVisibilityMessage(Guid userId, bool isVisible);
}