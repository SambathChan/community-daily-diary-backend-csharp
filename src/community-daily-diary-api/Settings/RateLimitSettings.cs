namespace community_daily_diary_api.Settings
{
    public class RateLimitSettings
    {
        public required string PolicyName { get; init; }
        public int PermitLimit { get; init; }
        public int Window { get; init; }
        public int SegmentsPerWindow { get; init; }
        public int QueueLimit { get; init; }
    }
}
