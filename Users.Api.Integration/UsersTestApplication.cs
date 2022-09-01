using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Users.Api.Integration;

public class UsersTestApplication : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestcontainersContainer _dbContainer = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("rihoko/benkyou_users_test")
        .WithPortBinding(15220,1433)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(15220))
        .Build();
    
    private readonly TestcontainersContainer _blobContainer = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("mcr.microsoft.com/azure-storage/azurite")
        .WithPortBinding(10123,10000)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(10123))
        .Build();

    public UsersTestApplication()
    {
        _dbContainer.StartAsync();
        _blobContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(collection =>
        {
            collection.AddMassTransitTestHarness();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public override async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await _dbContainer.CleanUpAsync();
        await _dbContainer.StopAsync();
        await _blobContainer.CleanUpAsync();
        await _blobContainer.StopAsync();
    }
}