using Sets.Api.Models.Entities;
using Shared.Models.Models;

namespace Sets.Api.Interfaces.Repositories;

public interface ISetsRepository
{
    public Task<Result<Set>> CreateSetAsync(Set set);

    public Task<Result<Set>> PatchSetAsync(Set set);

    public Task<Result> RemoveSetAsync(Guid setId);

    public Task<Result<Set>> GetSetAsync(Guid setId);

    public Task<Result<List<Set>>> GetUserSetsAsync(Guid userId, int pageNumber, int pageSize);

    public Task<Result<List<Set>>> GetAllSetsAsync(Guid userId, int pageNumber, int pageSize, string searchQuery);

    public Task<Result> ChangeSetsVisibilityAsync(Guid userId, bool arePublic);

    public Task<int> GetAllSetsPagesCountAsync(Guid userId);
    
    public Task<int> GetUserSetsPagesCountAsync(Guid userId);
}