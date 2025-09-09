using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrganizerPRO.Infrastructure;
public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        return services;
    }
}
