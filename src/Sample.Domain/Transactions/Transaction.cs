using ErrorOr;
using NodaMoney;
using NodaTime;

namespace Sample.Domain.Transactions;

public sealed class Transaction
{
    private Transaction()
    {
    }

    public required TransactionId Id { get; init; }

    public required Instant Date { get; init; }

    public required Money Amount { get; init; }

    public static ErrorOr<Transaction> Create(
        Guid providedId,
        DateTime Date,
        decimal amount)
    {
        var id = TransactionId.From(providedId);
        return new Transaction
        {
            Id = id,
            Date = Instant.MinValue,
            Amount = Money.FromDouble(0)
        };
    }
}
