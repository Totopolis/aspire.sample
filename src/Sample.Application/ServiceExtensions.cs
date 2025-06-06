using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.Application.Handlers;

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
        services.AddMediatR(cfg => cfg
            .RegisterServicesFromAssemblyContaining<ReceiveHandler>());

        return services;
    }
}
