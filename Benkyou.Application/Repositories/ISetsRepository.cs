using Benkyou.Domain.Models;

namespace Benkyou.Application.Repositories;

public interface ISetsRepository
{
    public Task<Result<Guid>> CreateSetAsync(CreateSetRequest setRequest, Guid userId);
    public Task<Result> ModifySetNameAsync(Guid cardId, string newName, Guid userId);
    public Task<Result> ModifySetDescriptionAsync(Guid cardId, string newDescription, Guid userId);
    public Task<Result> ModifySetKanjiListAsync(Guid cardId, List<KanjiRequest> kanjiList, Guid userId);
    public Task<Result> RemoveSetAsync(Guid cardId, Guid userId);
    public Task<Result<List<CardResponse>>> GetAllSetsAsync(Guid userId);
    public Task<Result<CardResponse>> GetSetAsync(Guid cardId);
    public Task SaveChangesAsync();
}