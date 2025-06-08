using Sample.Domain.Transactions;

namespace Sample.Application.Abstractions;

public interface ITransactionRepository
{
    Task<Transaction?> GetByProvidedId(Guid providedId, CancellationToken ct);

    Task<CreateAndCommitStatus> CreateAndCommit(Transaction transaction, CancellationToken ct);

    Task<IReadOnlyList<Transaction>> ExtractBatch(int batchSize, CancellationToken ct);

    public enum CreateAndCommitStatus
    {
        Created,
        DuplicateError,
        TooManyTransactionsError
    }
}
