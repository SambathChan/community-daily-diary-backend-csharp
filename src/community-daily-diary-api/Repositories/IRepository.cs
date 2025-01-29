using MongoDB.Bson;
using MongoDB.Driver;

namespace CommunityDailyDiary.Api.Repositories;

public interface IRepository<T, TKey>
{
    Task<IReadOnlyCollection<T>> GetManyAsync(FilterDefinition<T> filterDefinition, SortDefinition<T> sortDefinition, int offset, int count);
    Task<T> GetAsync(TKey id);
    Task CreateAsync(T entity);
    Task UpdateAsync(TKey id, UpdateDefinition<T> updateDefinition);
    Task ReplaceAsync(T entity);
    Task DeleteAsync(TKey id);
}
