using CommunityDailyDiary.Api.Settings;
using CommunityDailyDiary.Api.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Xunit;
using Microsoft.Extensions.Options;

namespace CommunityDailyDiary.Tests.Extensions;

public class SecurityExtensionsTests
{
    [Fact]
    public void EnableCors_ConfiguresCorrectPolicy()
    {
        // Arrange
        var services = new ServiceCollection();
        var corsSettings = new CorsSettings
        {
            PolicyName = "TestPolicy",
            Origins = ["http://localhost:4200"],
            Methods = ["GET", "POST"],
            Headers = ["Content-Type"]
        };

        // Act
        services.EnableCors(corsSettings);
        var serviceProvider = services.BuildServiceProvider();
        var corsOptions = serviceProvider.GetRequiredService<IOptions<CorsOptions>>();

        // Assert
        var policy = corsOptions.Value.GetPolicy(corsSettings.PolicyName);
        Assert.NotNull(policy);
        Assert.Contains(corsSettings.Origins[0], policy.Origins);
        Assert.Contains(corsSettings.Methods[0], policy.Methods);
        Assert.Contains(corsSettings.Headers[0], policy.Headers);
    }

    [Fact]
    public void EnableRateLimit_ConfiguresRateLimiter()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"RateLimitSettings:PolicyName", "TestPolicy"},
                {"RateLimitSettings:PermitLimit", "100"},
                {"RateLimitSettings:Window", "300"},
                {"RateLimitSettings:SegmentsPerWindow", "10"},
                {"RateLimitSettings:QueueLimit", "2"}
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.EnableRateLimit();
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<RateLimitSettings>>();

        // Assert
        Assert.NotNull(options);
        Assert.Equal("TestPolicy", options.Value.PolicyName);
        Assert.Equal(100, options.Value.PermitLimit);
        Assert.Equal(300, options.Value.Window);
        Assert.Equal(10, options.Value.SegmentsPerWindow);
        Assert.Equal(2, options.Value.QueueLimit);
    }
}