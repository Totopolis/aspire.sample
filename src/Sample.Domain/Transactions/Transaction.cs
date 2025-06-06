using ErrorOr;
using NodaMoney;
using NodaTime;
using Sample.Domain.Diagnostics;

namespace Sample.Domain.Transactions;

public sealed class Transaction
{
    private Transaction()
    {
    }

    public required TransactionId Id { get; init; }

    public required Guid ProvidedId { get; init; }

    public required Instant ProvidedDate { get; init; }

    public required Money ProvidedAmount { get; init; }

    public required Instant Created { get; init; }

    public static ErrorOr<Transaction> Create(
        Guid providedId,
        Instant providedDate,
        decimal providedAmount,
        Instant now)
    {
        if (providedDate == Instant.MinValue ||
            providedDate == Instant.MaxValue ||
            providedDate > now )
        {
            return DomainErrors.IncorrectDate;
        }

        if (providedAmount == decimal.MinValue ||
            providedAmount == decimal.MaxValue ||
            providedAmount <= 0)
        {
            return DomainErrors.IncorrectAmount;
        }

        var id = TransactionId.From(Guid.CreateVersion7());
        return new Transaction
        {
            Id = id,
            ProvidedId = providedId,
            ProvidedDate = providedDate,
            ProvidedAmount = Money.FromDecimal(providedAmount),
            Created = now
        };
    }
}
