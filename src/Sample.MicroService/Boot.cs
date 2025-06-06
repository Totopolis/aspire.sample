using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using NodaMoney;
using Sample.Api;
using Sample.Api.Common;
using Sample.Application;
using Sample.Infrastructure;
using Sample.Infrastructure.Database;
using Scalar.AspNetCore;
using System.Globalization;

namespace Sample.MicroService;

public static class Boot
{
    public static void PreBuild(this WebApplicationBuilder builder)
    {
        builder.AddServiceDefaults();

        SetupMoneyAccuracy();

        // https://fast-endpoints.com/docs/configuration-settings#specify-json-serializer-options
        builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.WriteIndented = true);

        builder.Services
            .AddSingleton<TimeProvider>(x => TimeProvider.System);

        // Options
        builder.Services
            .AddSampleApplicationOptions()
            .AddSampleInfrastructureOptions();

        // Services
        builder.Services
            .AddSampleApplicationServices(builder.Configuration)
            .AddSampleInfrastructureServices(builder.Configuration)
            .AddSampleApiServices(builder.Configuration);

        // Background services and startupers
        builder.Services
            .AddSampleInfrastructureHostedServices(builder.Configuration);

        builder.Services.AddOpenApi();

        builder.AddNpgsqlDbContext<SampleDbContext>(
            connectionName: "db",
            configureDbContextOptions: opt =>
            {
                opt.UseNpgsql(opt2 => opt2.UseNodaTime());
            });

        /*builder.Services.AddDbContext<SampleDbContext>(opt =>
        {
            var connectionString = builder.Configuration.GetConnectionString("db");

            if (connectionString is null)
            {
                opt.UseNpgsql(opt2 => opt2.UseNodaTime());
            }
            else
            {
                opt.UseNpgsql(
                    connectionString: connectionString,
                    opt2 => opt2.UseNodaTime());
            }
        });

        builder.EnrichNpgsqlDbContext<SampleDbContext>();*/
    }

    public static async Task PostBuild(
        this WebApplication app,
        CancellationToken ct)
    {
        // app.UseExceptionHandler();
        app.UseFastEndpoints();
        app.UseSampleExceptionHandler();

        app.MapOpenApi();
        app.MapScalarApiReference();

        app.MapDefaultEndpoints();

        await Task.CompletedTask;
    }

    private static void SetupMoneyAccuracy()
    {
        // throw exception if not registred
        _ = CurrencyInfo.FromCode("RUB");

        var myCurrency = CurrencyInfo.Create("RUB") with
        {
            MinorUnit = MinorUnit.Eight
        };

        CurrencyInfo.Unregister("RUB");
        CurrencyInfo.Register(myCurrency);

        // TODO: check, thats culture exists
        var russianCulture = new CultureInfo("ru-RU");
        // Numeric formats, dates and currencies
        CultureInfo.DefaultThreadCurrentCulture = russianCulture;
    }
}
