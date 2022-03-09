using Benkyou.Application.Repositories;

namespace Benkyou.Infrastructure.Repositories;

public class ApplicationUnitOfWork
{
    public ApplicationUnitOfWork(ICardsRepository cardsRepository, IUserStatisticsRepository userStatisticsRepository)
    {
        CardsRepository = cardsRepository;
        UserStatisticsRepository = userStatisticsRepository;
    }

    public ICardsRepository CardsRepository { get; }
    public IUserStatisticsRepository UserStatisticsRepository { get; }
}