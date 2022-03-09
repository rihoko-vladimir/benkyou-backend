using Benkyou.Application.Repositories;
using Benkyou.Domain.Database;

namespace Benkyou.Infrastructure.Repositories;

public class UserStatisticsRepository : IUserStatisticsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserStatisticsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool SetSetsCreated(Guid userId, int setsCreated)
    {
        throw new NotImplementedException();
    }

    public bool SetLastTimeStudied(Guid userId, DateTime lastTimeStudied)
    {
        throw new NotImplementedException();
    }

    public bool SetTotalTimeSpent(Guid userId, int totalSecondsSpent)
    {
        throw new NotImplementedException();
    }

    public int GetTotalSetsCreated(Guid userId)
    {
        throw new NotImplementedException();
    }

    public DateTime GetLastTimeStudied(Guid userId)
    {
        throw new NotImplementedException();
    }

    public int GetTotalSecondsSpent(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}