namespace Benkyou.Application.Repositories;

public interface IUserStatisticsRepository
{
    public bool SetSetsCreated(Guid userId, int setsCreated);
    public bool SetLastTimeStudied(Guid userId, DateTime lastTimeStudied);
    public bool SetTotalTimeSpent(Guid userId, int totalSecondsSpent);
    public int GetTotalSetsCreated(Guid userId);
    public DateTime GetLastTimeStudied(Guid userId);
    public int GetTotalSecondsSpent(Guid userId);
    public Task SaveChangesAsync();
}