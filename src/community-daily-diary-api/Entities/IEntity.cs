using MongoDB.Bson;

namespace community_daily_diary_api.Entities;

public interface IEntity
{
    ObjectId Id { get; set; }
}
