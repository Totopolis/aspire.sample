using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sample.Application.Abstractions;
using Sample.Domain.Transactions;

namespace Sample.Infrastructure.Database;

internal sealed class TransactionRepository : ITransactionRepository
{
    private readonly SampleDbContext _dbContext;

    public TransactionRepository(
        SampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ITransactionRepository.CreateAndCommitStatus> CreateAndCommit(
        Transaction transaction,
        CancellationToken ct)
    {
        _dbContext
            .Set<Transaction>()
            .Add(transaction);

        try
        {
            await _dbContext.SaveChangesAsync(ct);
            return ITransactionRepository.CreateAndCommitStatus.Created;
        }
        // DIRTY HACK: index unique error code
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            return ITransactionRepository.CreateAndCommitStatus.DuplicateError;
        }
    }

    public async Task<Transaction?> GetByProvidedId(
        Guid providedId,
        CancellationToken ct)
    {
        var entity = await _dbContext
            .Set<Transaction>()
            .AsNoTracking()
            .Where(x=>x.ProvidedId == providedId)
            .FirstOrDefaultAsync(ct);

        return entity;
    }
}
