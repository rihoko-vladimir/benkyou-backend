using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Users.Api.HealthChecks;

public class DbHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public DbHealthCheck(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("UsersSqlServerConnectionString");
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        bool isHealthy;
        try
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            isHealthy = true;
            await connection.DisposeAsync();
        }
        catch (Exception)
        {
            isHealthy = false;
        }

        return isHealthy ? HealthCheckResult.Healthy("Ready") : HealthCheckResult.Unhealthy("Connection error");
    }
}