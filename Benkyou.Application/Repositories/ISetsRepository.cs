using Benkyou.Domain.Entities;
using Benkyou.Domain.Models;

namespace Benkyou.Application.Repositories;

public interface ISetsRepository
{
    public Task<Guid> CreateSetAsync(CreateSetRequest setRequest, Guid userId);
    public Task ModifySetNameAsync(Guid cardId, string newName, Guid userId);
    public Task ModifySetDescriptionAsync(Guid cardId, string newDescription, Guid userId);
    public Task ModifySetKanjiListAsync(Guid cardId, List<KanjiRequest> kanjiList, Guid userId);
    public Task RemoveSetAsync(Guid cardId, Guid userId);
    public Task<List<CardResponse>> GetAllSetsAsync(Guid userId);
    public Task<CardResponse> GetSetAsync(Guid cardId);
    public Task SaveChangesAsync();
}