namespace CommunityDailyDiary.Api.Settings
{
    public class CorsSettings
    {
        public string PolicyName { get; init; }
        public string[] Origins { get; init; }
        public string[] Headers { get; init; }
        public string[] Methods { get; init; }
    }
}
