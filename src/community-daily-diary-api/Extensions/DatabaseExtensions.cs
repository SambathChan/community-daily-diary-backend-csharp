using CommunityDailyDiary.Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Entities;

namespace CommunityDailyDiary.Api.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddMongoDbEntities(this IServiceCollection services)
    {
        services.AddOptions<MongoDbSettings>()
            .BindConfiguration(nameof(MongoDbSettings))
            .ValidateOnStart();

        services.AddSingleton(serviceProvider =>
        {
            var mongoDbSettings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;

            return DB.InitAsync(
                mongoDbSettings.DatabaseName,
                MongoClientSettings.FromConnectionString(mongoDbSettings.ConnectionString)
            ).GetAwaiter().GetResult();
        });

        return services;
    }
}
