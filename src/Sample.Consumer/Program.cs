using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Application.Abstractions;
using Sample.Infrastructure.Database;
using Sample.Producer;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<SampleDbContext>(
    connectionName: "db",
    configureDbContextOptions: opt =>
    {
        opt.ConfigureWarnings(opt2 => opt2
            .Log((RelationalEventId.CommandExecuted, LogLevel.Trace)));

        opt.UseNpgsql(opt2 => opt2
            .UseNodaTime()
            .ExecutionStrategy(x => new NonRetryingExecutionStrategy(x)));
    });

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddHostedService<ConsumerHostedService>();

using var host = builder.Build();

await host.RunAsync();
