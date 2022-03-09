using Benkyou.Domain.Entities;

namespace Benkyou.Application.Repositories;

public interface ICardsRepository
{
    public Guid CreateCard(string cardName, string cardDescription, ICollection<Kanji> kanjis, Guid userId);
    public bool ModifyCard(Guid cardId, Card modifiedCard);
    public bool RemoveCard(Guid cardId);
    public ICollection<Card> GetAllCards(Guid userId);
    public Card GetCard(Guid cardId);
    public Task SaveChangesAsync();
}