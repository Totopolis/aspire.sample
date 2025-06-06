using NodaTime;
using Sample.Domain.Diagnostics;
using Sample.Domain.Transactions;

namespace Sample.Domain.Tests;

public class TransactionTests
{
    [Fact]
    public void SuccessCreateAggregate()
    {
        var now = DateTimeOffset.UtcNow;
        var past = now - TimeSpan.FromHours(1);

        var errorOrTransaction = Transaction.Create(
            providedId: Guid.NewGuid(),
            providedDate: Instant.FromDateTimeOffset(past),
            providedAmount: 100_500,
            now: Instant.FromDateTimeOffset(now));

        Assert.False(errorOrTransaction.IsError);
    }

    [Fact]
    public void FailedCreateIncorrectDate()
    {
        var now = DateTimeOffset.UtcNow;
        // Set incorrect past
        var past = now + TimeSpan.FromHours(1);

        var errorOrTransaction = Transaction.Create(
            providedId: Guid.NewGuid(),
            providedDate: Instant.FromDateTimeOffset(past),
            providedAmount: 100_500,
            now: Instant.FromDateTimeOffset(now));

        Assert.True(errorOrTransaction.IsError);
        Assert.Single(errorOrTransaction.Errors);
        Assert.Equal(DomainErrors.IncorrectDate, errorOrTransaction.FirstError);
    }

    [Fact]
    public void FailedCreateIncorrectAmount()
    {
        var now = DateTimeOffset.UtcNow;
        var past = now - TimeSpan.FromHours(1);

        var errorOrTransaction = Transaction.Create(
            providedId: Guid.NewGuid(),
            providedDate: Instant.FromDateTimeOffset(past),
            // Set incorrect amount
            providedAmount: -100_500,
            now: Instant.FromDateTimeOffset(now));

        Assert.True(errorOrTransaction.IsError);
        Assert.Single(errorOrTransaction.Errors);
        Assert.Equal(DomainErrors.IncorrectAmount, errorOrTransaction.FirstError);
    }
}
