using Benkyou.Domain.Entities;
using Benkyou.Domain.Models;

namespace Benkyou.Application.Repositories;

public interface ISetsRepository
{
    public Task<Guid> CreateCardAsync(CreateSetRequest setRequest, Guid userId);
    public Task ModifyCardAsync(Guid cardId, Card modifiedCard);
    public Task RemoveCardAsync(Guid cardId);
    public Task<List<CardResponse>> GetAllCardsAsync(Guid userId);
    public Task<CardResponse> GetCardAsync(Guid cardId);
    public Task SaveChangesAsync();
}