using community_daily_diary_api.Modules;
using System.Reflection;

namespace community_daily_diary_api.Extensions;

public static class ApiEndPointsExtensions
{
    public static IApplicationBuilder UseMinimalApiEndPoints(this IApplicationBuilder app)
    {
        var moduleTypes = Assembly.GetExecutingAssembly().GetTypes()
        .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
        .ToList();

        if (app is not WebApplication webApp)
        {
            throw new InvalidOperationException("UseMinimalApiEndpoints can only be used with WebApplication");
        }

        foreach (var moduleType in moduleTypes)
        {
            var module = Activator.CreateInstance(moduleType) as IModule;
            var group = webApp.MapGroup("/api");
            module?.MapEndpoints(group);
        }

        return webApp;
    }
}
