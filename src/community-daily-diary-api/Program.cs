using CommunityDailyDiary.Api.Entities;
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

        builder.Services.AddMongo()
                        .AddMongoRepository<Post>();

        //Try Hybrid Caching
        builder.Services.AddDistributedMemoryCache();

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        builder.Services.AddHybridCache(options =>
        {
            options.MaximumKeyLength = 256;

            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30),
                LocalCacheExpiration = TimeSpan.FromMinutes(30)
            };
        });
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
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
