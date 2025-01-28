using MongoDB.Bson;

namespace CommunityDailyDiary.Api.Entities;

public interface IEntity
{
    ObjectId Id { get; set; }
}
