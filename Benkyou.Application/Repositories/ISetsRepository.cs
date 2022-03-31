using Benkyou.Domain.Models;
using Benkyou.Domain.Models.Requests;
using Benkyou.Domain.Models.Responses;

namespace Benkyou.Application.Repositories;

public interface ISetsRepository
{
    public Task<Result<SetResponse>> CreateSetAsync(CreateSetRequest setRequest, Guid userId);
    public Task<Result> ModifySetAsync(ModifySetRequest modifyRequest, Guid userId);
    public Task<Result> RemoveSetAsync(Guid cardId, Guid userId);
    public Task<Result<List<SetResponse>>> GetUserSetsAsync(Guid userId);
    public Task<Result<int>> GetAllSetsPageCount(int pageSize);
    public Task<Result<List<SetResponse>>> GetAllSetsByPageAsync(int pageNumber, int pageSize);
    public Task<Result<List<SetResponse>>> GetSetsByQuery(string searchQuery, int pageNumber, int pageSize);
    public Task<Result<SetResponse>> GetSetAsync(Guid cardId);
    public Task SaveChangesAsync();
}