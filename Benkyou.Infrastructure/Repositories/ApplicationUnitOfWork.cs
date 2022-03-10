using Benkyou.Application.Repositories;

namespace Benkyou.Infrastructure.Repositories;

public class ApplicationUnitOfWork
{
    public ApplicationUnitOfWork(ISetsRepository setsRepository, IUserStatisticsRepository userStatisticsRepository)
    {
        SetsRepository = setsRepository;
        UserStatisticsRepository = userStatisticsRepository;
    }

    public ISetsRepository SetsRepository { get; }
    public IUserStatisticsRepository UserStatisticsRepository { get; }
}