using CommunityDailyDiary.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Xunit;

namespace CommunityDailyDiary.Tests.Infrastructure;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer;
    public IMongoDatabase Database => Services.GetRequiredService<IMongoDatabase>();

    public IntegrationTestFactory()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .WithName($"test-mongo-{Guid.NewGuid()}")
            .WithPortBinding(27017, true)
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(serviceProvider =>
            {
                var client = new MongoClient(_mongoDbContainer.GetConnectionString());
                return client.GetDatabase("test-db");
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _mongoDbContainer.DisposeAsync();
    }
}