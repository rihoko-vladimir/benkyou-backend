using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Statistics.Api.Integration;

public class StatisticsTestApplication : WebApplicationFactory<Program>
{
    private readonly TestcontainersContainer _dbContainer = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("mongo")
        .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "test_rihoko")
        .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "testtesttesttest1A")
        .WithPortBinding(30341,27017)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(30341))
        .Build();
    
    public StatisticsTestApplication()
    {
        _dbContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(collection =>
        {
            collection.AddMassTransitTestHarness();
        });
    }

    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _dbContainer.CleanUpAsync();
        await _dbContainer.StopAsync();
    }
}