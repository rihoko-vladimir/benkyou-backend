using Benkyou.Domain.Entities;
using Benkyou.Domain.Models;

namespace Benkyou.Application.Repositories;

public interface ISetsRepository
{
    public Task<Guid> CreateSetAsync(CreateSetRequest setRequest, Guid userId);
    public Task ModifySetAsync(Guid cardId, Card modifiedCard);
    public Task RemoveSetAsync(Guid cardId, Guid userId);
    public Task<List<CardResponse>> GetAllSetsAsync(Guid userId);
    public Task<CardResponse> GetSetAsync(Guid cardId);
    public Task SaveChangesAsync();
}