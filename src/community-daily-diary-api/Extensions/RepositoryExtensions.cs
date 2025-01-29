using CommunityDailyDiary.Api.Entities;
using CommunityDailyDiary.Api.Repositories;
using CommunityDailyDiary.Api.Settings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace CommunityDailyDiary.Api.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        ConventionRegistry.Register("MongoDBConvention", new ConventionPack
        {
            new IgnoreIfNullConvention(false),
            new CamelCaseElementNameConvention(),
        }, _ => true);

        services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
            return mongoClient.GetDatabase(mongoDbSettings.DatabaseName);
        });

        return services;
    }

    public static IServiceCollection AddMongoRepository<T, TKey>(this IServiceCollection services, string collectionName) where T : IEntity
    {
        services.AddSingleton<IRepository<T, ObjectId>, MongoRepository<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<T>(database, collectionName);
        });
        return services;
    }
}
