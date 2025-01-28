using MongoDB.Bson;
using MongoDB.Driver;

namespace CommunityDailyDiary.Api.Repositories;

public interface IRepository<T>
{
    Task<IReadOnlyCollection<T>> GetManyAsync(FilterDefinition<T> filter, SortDefinition<T> sort, int offset, int count);
    Task<T> GetAsync(ObjectId id);
    Task CreateAsync(T entity);
    Task UpdateAsync(ObjectId id, UpdateDefinition<T> updateDefinition);
    Task ReplaceAsync(T entity);
    Task DeleteAsync(ObjectId id);
}
