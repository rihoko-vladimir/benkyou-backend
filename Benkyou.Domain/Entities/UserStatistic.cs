namespace Benkyou.Domain.Entities;

public class UserStatistic
{
    public int CardsCreatedCount { get; set; }

    public DateTime LastTimeStudied { get; set; }

    public int TotalTimeSpent { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}