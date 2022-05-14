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
    public Task<Result<int>> GetAllSetsPageCountAsync(Guid userId, int pageSize);

    public Task<Result<int>> GetAllSetsByQueryPageCountAsync(Guid userId, int pageNumber, int pageSize,
        string searchQuery);

    public Task<Result<List<SetResponse>>> GetAllSetsByPageAsync(Guid userId, int pageNumber, int pageSize);

    public Task<Result<List<SetResponse>>> GetAllSetsAsync();

    public Task<Result<List<SetResponse>>> GetSetsByQueryAsync(Guid userId, string searchQuery, int pageNumber,
        int pageSize);

    public Task<Result<SetResponse>> GetSetAsync(Guid cardId);
    public Task SaveChangesAsync();
}