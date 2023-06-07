using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Users.Api.Integration;

public class UsersTestApplication : IAsyncLifetime
{
    private readonly IContainer _blobContainer = new ContainerBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite")
        .WithPortBinding(10123, 10000)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(10123))
        .Build();

    private readonly IContainer _dbContainer = new ContainerBuilder()
        .WithImage("rihoko/benkyou_users_test")
        .WithPortBinding(15220, 1433)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(15220))
        .Build();

    public UsersTestApplication()
    {
        _dbContainer.StartAsync();
        _blobContainer.StartAsync();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }
}