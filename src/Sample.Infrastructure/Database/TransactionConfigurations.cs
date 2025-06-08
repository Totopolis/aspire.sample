using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Domain.Transactions;

namespace Sample.Infrastructure.Database;

internal sealed class TransactionConfigurations : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .ToTable("transaction")
            .HasKey(x => x.Id)
            .HasName("transaction_id");

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id")
            .HasConversion(id => id.Value, val => TransactionId.From(val));

        builder
            .Property(x => x.ProvidedId)
            .HasColumnName("provided_id")
            .IsRequired();

        builder
            .Property(x => x.ProvidedDate)
            .HasColumnName("provided_date")
            .HasColumnType(SampleDbContext.TimestampType)
            .IsRequired();

        builder
            .HasIndex(x => x.ProvidedDate)
            .HasDatabaseName("idx_provided_date")
            .IsUnique(true);

        builder
            .Property(x => x.ProvidedAmount)
            .HasColumnName("provided_amount")
            .HasConversion(x => x.Amount, val => NodaMoney.Money.FromDecimal(val))
            .HasColumnType(SampleDbContext.MoneyType)
            .IsRequired();

        builder
            .Property(x => x.Created)
            .HasColumnName("created")
            .HasColumnType(SampleDbContext.TimestampType)
            .IsRequired();
    }
}
