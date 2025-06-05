using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddSampleApplicationOptions(
        this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddSampleApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ApplicationSettings>());

        return services;
    }
}
