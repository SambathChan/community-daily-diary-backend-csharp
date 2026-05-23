using CommunityDailyDiary.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;
using Testcontainers.MongoDb;
using Xunit;

namespace CommunityDailyDiary.Tests.Infrastructure;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer;

    public IntegrationTestWebAppFactory()
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
            services.AddSingleton(_ =>
                DB.InitAsync(
                    "test-db",
                    MongoClientSettings.FromConnectionString(_mongoDbContainer.GetConnectionString())
                ).GetAwaiter().GetResult());
        });
    }

    public async ValueTask InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        try
        {
            await base.DisposeAsync();
        }finally
        {
            await _mongoDbContainer.DisposeAsync();
        }
    }
}