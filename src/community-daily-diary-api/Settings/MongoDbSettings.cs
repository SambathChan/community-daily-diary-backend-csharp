namespace CommunityDailyDiary.Api.Settings;

public class MongoDbSettings
{
    public required string ConnectionString { get; init; }
    public required string DatabaseName { get; init; }
}