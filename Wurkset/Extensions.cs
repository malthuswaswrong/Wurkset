using Microsoft.Extensions.DependencyInjection;

namespace Wurkset;

public static class Extensions
{
    public static void AddWurkset(this IServiceCollection services, Action<WorksetRepositoryOptions> action)
    {
        services.AddSingleton<WorksetRepository>();
        services.Configure(action);
    }
}
