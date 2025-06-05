using Sample.Domain.Transactions;

namespace Sample.Domain.Tests;

public class TransactionTests
{
    [Fact]
    public void SuccessCreateAggregate()
    {
        var errorOrTransaction = Transaction.Create(
            providedId: Guid.NewGuid(),
            Date: DateTime.MaxValue,
            amount: 100_500);

        Assert.False(errorOrTransaction.IsError);
    }
}
