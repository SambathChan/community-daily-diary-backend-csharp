using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CommunityDailyDiary.Api.Entities;

[BsonIgnoreExtraElements]
public class Post : IEntity
{
    public ObjectId Id { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public int Vote { get; set; }
    public DateTime CreatedAt { get; set; }
}
