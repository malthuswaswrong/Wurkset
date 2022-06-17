using Microsoft.Extensions.DependencyInjection;

namespace Wurkset;

public static class Extensions
{
    public static string ToPath(this long value)
    {
        var tmp = value.ToString().ToCharArray();
        return string.Join('/', tmp);
    }
    public static void AddWurkset(this IServiceCollection services, Action<WorksetRepositoryOptions> action)
    {
        services.AddSingleton<WorksetRepository>();
        services.Configure(action);
    }
}
