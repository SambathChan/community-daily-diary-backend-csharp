using CommunityDailyDiary.Api.Extensions;
using CommunityDailyDiary.Api.Settings;
using Microsoft.Extensions.Caching.Hybrid;

namespace CommunityDailyDiary.Api;

public class Program
{
    public static void Main(string[] args)
    {        
        var builder = WebApplication.CreateBuilder(args);

        var corsSettings = builder.Configuration.GetSection(nameof(CorsSettings)).Get<CorsSettings>();

        builder.Services.EnableCors(corsSettings)
                        .EnableRateLimit();

        builder.Services.AddMongoDbEntities();

        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddHybridCache(options =>
        {
            options.MaximumKeyLength = 256;

            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30),
                LocalCacheExpiration = TimeSpan.FromMinutes(30)
            };
        });

        builder.Services.AddValidation();
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors(corsSettings.PolicyName);
        app.UseRateLimiter();
        app.UseAuthorization();
        app.UseMinimalApiEndPoints();

        app.Run();
    }
}
