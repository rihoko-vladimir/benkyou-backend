using Benkyou.Domain.Models;
using Benkyou.Domain.Models.Requests;
using Benkyou.Domain.Models.Responses;

namespace Benkyou.Application.Repositories;

public interface ISetsRepository
{
    public Task<Result<Guid>> CreateSetAsync(CreateSetRequest setRequest, Guid userId);

    public Task<Result> ModifySetAsync(ModifySetRequest modifyRequest, Guid userId);
    
    public Task<Result> RemoveSetAsync(Guid cardId, Guid userId);
    public Task<Result<List<SetResponse>>> GetAllSetsAsync(Guid userId);
    public Task<Result<SetResponse>> GetSetAsync(Guid cardId);
    public Task SaveChangesAsync();
}