using CommunityDailyDiary.Api.Settings;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
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
                                            .WithMethods(corsSettings.Methods)
                                            .WithHeaders(corsSettings.Headers);
                                  });
            });

            return services;
        }

        public static IServiceCollection EnableRateLimit(this IServiceCollection services)
        {
            services.AddOptions<RateLimitSettings>()
                .BindConfiguration(nameof(RateLimitSettings))
                .ValidateOnStart();

            var serviceProvider = services.BuildServiceProvider();
            var rateLimitSettings = serviceProvider.GetRequiredService<IOptions<RateLimitSettings>>().Value;

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
