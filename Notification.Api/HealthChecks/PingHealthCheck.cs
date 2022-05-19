using System.Net.NetworkInformation;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Notification.Api.Extensions;

namespace Notification.Api.HealthChecks;

public class PingHealthCheck : IHealthCheck
{
    private readonly string _server;

    public PingHealthCheck(string server)
    {
        _server = server;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var pingClient = new Ping();
        var result = await pingClient.SendPingAsync(_server);
        var isSuccess = result.Status == IPStatus.Success;
        return await Task.FromResult(isSuccess ? HealthCheckResult.Healthy("Smtp server is available") : HealthCheckResult.Unhealthy("Smtp server is unavailable"));
    }
}