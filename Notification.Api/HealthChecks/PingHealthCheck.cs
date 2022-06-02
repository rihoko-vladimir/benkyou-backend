using System.Net.NetworkInformation;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Notification.Api.HealthChecks;

public class PingHealthCheck : IHealthCheck
{
    private readonly string _server;

    public PingHealthCheck(string server)
    {
        _server = server;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        var pingClient = new Ping();
        bool isSuccess;
        var errorMessage = string.Empty;
        
        try
        {
            var result = await pingClient.SendPingAsync(_server);
            isSuccess = result.Status == IPStatus.Success;
        }
        catch (Exception e)
        {
            isSuccess = false;
            errorMessage = e.Source!;
        }

        return await Task.FromResult(isSuccess
            ? HealthCheckResult.Healthy("Smtp server is available")
            : HealthCheckResult.Unhealthy($"Smtp server is unavailable. Occured at : {errorMessage}"));
    }
}