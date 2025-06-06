using Microsoft.EntityFrameworkCore;

namespace Sample.Infrastructure.Database;

public class SampleDbContext : DbContext
{
    public const string TimestampType = "timestamp with time zone";
    public const string MoneyType = "numeric(18,8)";

    public SampleDbContext(DbContextOptions<SampleDbContext> options)
        : base(options)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.RegisterAllInVogenEfCoreConverters();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
