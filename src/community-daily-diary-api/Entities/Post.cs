using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace CommunityDailyDiary.Api.Entities;

[BsonIgnoreExtraElements]
public class Post : Entity
{
    public required string Title { get; set; }
    public required string Body { get; set; }
    public int Vote { get; set; }
    public DateTime CreatedAt { get; set; }
}
