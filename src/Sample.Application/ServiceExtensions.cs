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

    // TODO: implement created transaction metric like a:
    // https://pedrocons.com/how-to-create-custom-metrics-in-net-using-opentelemetry-net-aspire-dashboard-step-by-step-guide/
    public static IServiceCollection AddSampleApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg
            .RegisterServicesFromAssemblyContaining<ReceiveHandler>());

        return services;
    }
}
