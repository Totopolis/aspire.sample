using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Sample.Application;
using Sample.Application.Abstractions;
using Sample.Infrastructure.Database;
using ZiggyCreatures.Caching.Fusion;

namespace Sample.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddSampleInfrastructureOptions(
        this IServiceCollection services)
    {
        // TODO: configure options from aspire
        return services;
    }

    public static IServiceCollection AddSampleInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ATTENTION: registered in Sample.MicroService like Aspire source
        // services.AddDbContext<SampleDbContext>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        // TODO: use L2 in redis
        services.AddFusionCache(ApplicationConstants.TransactionCacheName)
            .WithMemoryCache(new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            }))
            .WithDefaultEntryOptions(opts =>
            {
                opts.Duration = TimeSpan.FromHours(1);
                opts.Size = 1;
            });

        // TODO: use time zone from IOptions<>
        services.AddSingleton<ITimeZoneApplicator, TimeZoneApplicator>();

        return services;
    }

    public static IServiceCollection AddSampleInfrastructureHostedServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHostedService<HostedService>();
        return services;
    }

    private sealed class TimeZoneApplicator : ITimeZoneApplicator
    {
        public DateTime ToZonedDatetime(Instant stamp)
        {
            var local = stamp.ToDateTimeOffset().LocalDateTime;
            return local;
        }
    }
}
