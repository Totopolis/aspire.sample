using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Infrastructure.Database;

var builder = Host.CreateApplicationBuilder(args);

builder.AddNpgsqlDbContext<SampleDbContext>(
    connectionName: "db",
    configureDbContextOptions: opt =>
    {
        opt.UseNpgsql(opt2 => opt2.UseNodaTime());
    });

using var host = builder.Build();

using var scope = host.Services.CreateScope();
using var db = scope.ServiceProvider.GetRequiredService<SampleDbContext>();
await db.Database.EnsureCreatedAsync();
