using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Sample.Application.Abstractions;
using Sample.Domain.Transactions;

namespace Sample.Infrastructure.Database;

internal sealed class TransactionRepository : ITransactionRepository
{
    /// <summary>
    /// TODO: Consider a flexible upper limit
    /// </summary>
    public const int Limit = 100;

    private readonly ILogger<TransactionRepository> _logger;
    private readonly SampleDbContext _dbContext;

    public TransactionRepository(
        ILogger<TransactionRepository> logger,
        SampleDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<ITransactionRepository.CreateAndCommitStatus> CreateAndCommit(
        Transaction transaction,
        CancellationToken ct)
    {
        using var pgTransaction = await _dbContext.Database.BeginTransactionAsync(
            isolationLevel: System.Data.IsolationLevel.ReadCommitted,
            cancellationToken: ct);

        await _dbContext.Database.ExecuteSqlRawAsync(
                sql: "LOCK TABLE transaction IN SHARE ROW EXCLUSIVE MODE",
                cancellationToken: ct);

        var count = await _dbContext.Set<Transaction>().CountAsync(ct);
        if (count >= Limit)
        {
            if (count > Limit)
            {
                _logger.LogCritical("Incredibly, there are too many transactions");
            }

            return ITransactionRepository.CreateAndCommitStatus.TooManyTransactionsError;
        }

        try
        {
            _dbContext
                .Set<Transaction>()
                .Add(transaction);

            await _dbContext.SaveChangesAsync(ct);

            await pgTransaction.CommitAsync(ct);

            return ITransactionRepository.CreateAndCommitStatus.Created;
        }
        // DIRTY HACK: index unique error code
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException pe &&
            pe.SqlState == "23505")
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
