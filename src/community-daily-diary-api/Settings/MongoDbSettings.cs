namespace community_daily_diary_api.Settings;

public class MongoDbSettings
{
    public required string Host { get; init; }
    public int Port { get; init; }
    public required string DatabaseName { get; init; }
    public string ConnectionString => $"mongodb://{Host}:{Port}";
}