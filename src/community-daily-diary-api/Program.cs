using CommunityDailyDiary.Api.Entities;
using CommunityDailyDiary.Api.Extensions;
using CommunityDailyDiary.Api.Settings;

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
                        .AddMongoRepository<Post>("Posts");

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
