﻿using community_daily_diary_api.Entities;
using community_daily_diary_api.Repositories;
using community_daily_diary_api.Settings;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace community_daily_diary_api.Extensions;

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

    public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
    {
        services.AddSingleton<IRepository<T>, MongoRepository<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<T>(database, collectionName);
        });
        return services;
    }
}
