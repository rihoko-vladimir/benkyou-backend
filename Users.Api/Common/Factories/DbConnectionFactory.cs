using System.Data.Common;
using Microsoft.Data.SqlClient;
using Users.Api.Interfaces.Factories;

namespace Users.Api.Common.Factories;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("UsersSqlServerConnectionString");
    }

    public DbConnection GetConnection()
    {
        var connection = new SqlConnection(_connectionString);

        return connection;
    }
}