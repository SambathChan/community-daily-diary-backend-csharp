﻿using community_daily_diary_api.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Search;

namespace community_daily_diary_api.Repositories
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task CreateAsync(T entity)
        {
            if(entity is null)
            {
                throw new ArgumentNullException();
            }

            await dbCollection.InsertOneAsync(entity);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }

        public async Task<IReadOnlyCollection<T>> GetManyAsync(FilterDefinition<T> filter, int offset, int count)
        {
            return await dbCollection.Find(filter ?? filterBuilder.Empty).Skip(offset).Limit(count).ToListAsync();
        }

        public async Task<T> GetAsync(ObjectId id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(ObjectId id, UpdateDefinition<T> updateDefinition)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, id);
            await dbCollection.UpdateOneAsync(filter, updateDefinition);
        }

        public async Task ReplaceAsync(T entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException();
            }

            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }
    }
}
