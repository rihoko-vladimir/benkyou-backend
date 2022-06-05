using System.Data;
using Microsoft.Data.SqlClient;

namespace Users.Api.Common.Helpers;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("UsersSqlServerConnectionString");
    }

    public IDbConnection CreateDbConnection()
    {
        return new SqlConnection(_connectionString);
    }
}