namespace CommunityDailyDiary.Tests.Infrastructure;

public static class TestSettings
{
    public const string TestDatabaseName = "test-db";
    public const string TestCollectionName = "posts";
    
    public static class Messages
    {
        public const string PostNotFound = "Post not found";
        public const string InvalidObjectId = "Invalid ObjectId format";
    }
}