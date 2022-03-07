using Benkyou.Application.Repositories;
using Benkyou.Domain.Database;
using Benkyou.Domain.Entities;

namespace Benkyou.Infrastructure.Repositories;

public class CardsRepository : ICardsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CardsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Guid CreateCard(string cardName, string cardDescription, ICollection<Kanji> kanjis, Guid userId)
    {
        throw new NotImplementedException();
    }

    public bool ModifyCard(Guid cardId, Card modifiedCard)
    {
        throw new NotImplementedException();
    }

    public bool RemoveCard(Guid cardId)
    {
        throw new NotImplementedException();
    }

    public ICollection<Card> GetAllCards(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Card GetCard(Guid cardId)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}