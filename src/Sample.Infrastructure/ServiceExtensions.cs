using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddSampleInfrastructureOptions(
        this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddSampleInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // It is scoped service
        // services.AddDbContext<SampleDbContext>();

        // services.AddScoped<IUnitOfWork, UnitOfWork>();
        // services.AddScoped<ITransactionRepository, TransactionRepository>();

        return services;
    }

    public static IServiceCollection AddSampleInfrastructureHostedServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // services.AddHostedService<SampleCleanerService>();
        return services;
    }
}
