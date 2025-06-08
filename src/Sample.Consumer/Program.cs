using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Hosting;
using Sample.Infrastructure.Database;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<SampleDbContext>(
    connectionName: "db",
    configureDbContextOptions: opt =>
    {
        opt.UseNpgsql(opt2 => opt2
            .UseNodaTime()
            .ExecutionStrategy(x => new NonRetryingExecutionStrategy(x)));
    });

using var host = builder.Build();

await host.RunAsync();
