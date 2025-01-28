using CommunityDailyDiary.Api.Settings;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace CommunityDailyDiary.Api.Extensions
{
    public static class SecurityExtensions
    {
        public static IServiceCollection EnableCors(this IServiceCollection services, CorsSettings corsSettings)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: corsSettings.PolicyName,
                                  policy =>
                                  {
                                      policy.WithOrigins(corsSettings.Origins)
                                            .WithMethods("GET", "POST", "PATCH")
                                            .WithHeaders("content-type");
                                  });
            });

            return services;
        }

        public static IServiceCollection EnableRateLimit(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var rateLimitSettings = configuration.GetSection(nameof(RateLimitSettings)).Get<RateLimitSettings>();

            services.Configure<RateLimitSettings>(configuration.GetSection(nameof(RateLimitSettings)));

            services.AddRateLimiter(_ => _
                .AddSlidingWindowLimiter(policyName: rateLimitSettings.PolicyName, options =>
                {
                    options.PermitLimit = rateLimitSettings.PermitLimit;
                    options.Window = TimeSpan.FromSeconds(rateLimitSettings.Window);
                    options.SegmentsPerWindow = rateLimitSettings.SegmentsPerWindow;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = rateLimitSettings.QueueLimit;
                }));

            return services;
        }
    }
}
