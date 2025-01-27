using MongoDB.Bson;
using MongoDB.Driver;

namespace community_daily_diary_api.Repositories;

public interface IRepository<T>
{
    Task<IReadOnlyCollection<T>> GetManyAsync(FilterDefinition<T> filter, int offset, int count);
    Task<T> GetAsync(ObjectId id);
    Task CreateAsync(T entity);
    Task UpdateAsync(ObjectId id, UpdateDefinition<T> updateDefinition);
    Task ReplaceAsync(T entity);
    Task DeleteAsync(ObjectId id);
}
