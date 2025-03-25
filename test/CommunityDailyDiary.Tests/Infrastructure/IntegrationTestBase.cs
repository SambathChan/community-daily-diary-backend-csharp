using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Xunit;

namespace CommunityDailyDiary.Tests.Infrastructure
{
    public abstract class IntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>,
      IDisposable
    {
        private readonly IServiceScope _scope;
        protected readonly IMongoDatabase _database;
        protected readonly HttpClient _client;

        protected IntegrationTestBase(IntegrationTestWebAppFactory factory)
        {
            _scope = factory.Services.CreateScope();
            _database = _scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
            _client = factory.CreateClient();
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
