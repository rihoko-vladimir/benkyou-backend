using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Auth.Api.Integration;

public class AuthTestApplication : WebApplicationFactory<Program>
{
    private readonly TestcontainersContainer _dbContainer = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithEnvironment("SA_PASSWORD", "nandesukaanatawa1A")
        .WithPortBinding(13752, 1433)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(13752))
        .Build();

    public AuthTestApplication()
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
        await _dbContainer.CleanUpAsync();
        await _dbContainer.StopAsync();
    }
}