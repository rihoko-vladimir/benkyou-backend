using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Sets.Api.Integration;

public class SetsTestApplication : WebApplicationFactory<Program>
{
    private readonly IContainer _dbContainer = new ContainerBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithEnvironment("SA_PASSWORD", "nandesukaanatawa1A")
        .WithPortBinding(13755, 1433)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(13755))
        .Build();

    public SetsTestApplication()
    {
        _dbContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(collection => { collection.AddMassTransitTestHarness(); });
    }

    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _dbContainer.StopAsync();
    }
}