using Sets.Api.Models.Entities;
using Sets.Api.Models.Requests;
using Sets.Api.Models.Responses;
using Shared.Models.Models;

namespace Sets.Api.Interfaces.Services;

public interface ISetsService
{
    public Task<Result<SetResponse>> CreateSetAsync(Guid userId, SetRequest set);

    public Task<Result<SetResponse>> PatchSetAsync(Guid userId, Guid setId, Set set);

    public Task<Result> RemoveSetAsync(Guid userId, Guid setId);

    public Task<Result<PagedSetsResponse>> GetUserSetsAsync(Guid userId, int pageNumber, int pageSize);

    public Task<Result<PagedSetsResponse>> GetAllSetsAsync(Guid userId, int pageNumber, int pageSize,
        string searchQuery);

    public Task<Result<Set>> GetSetAsync(Guid setId);

    public Task<Result> ChangeSetsVisibilityAsync(Guid userId, bool arePublic);

    public Task<Result> FinishSetLearning(Guid userId, Guid setId, FinishLearningRequest finishLearningRequest);
}