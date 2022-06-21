using System.Data.Common;

namespace Users.Api.Interfaces.Factories;

public interface IDbConnectionFactory
{
    public DbConnection GetConnection();
}