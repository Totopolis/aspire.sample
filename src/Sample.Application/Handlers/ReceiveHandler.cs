using ErrorOr;
using MediatR;
using NodaTime.Extensions;
using Sample.Application.Abstractions;
using Sample.Application.Diagnostics;
using Sample.Contracts.Receive;
using Sample.Contracts.ReceiveTransaction;
using Sample.Domain.Transactions;
using ZiggyCreatures.Caching.Fusion;

namespace Sample.Application.Handlers;

public sealed class ReceiveHandler :
    IRequestHandler<ReceiveCommand, ErrorOr<ReceiveCommandResult>>
{
    private readonly TimeProvider _timeProvider;
    private readonly IFusionCache _cache;
    private readonly ITransactionRepository _repository;
    private readonly ITimeZoneApplicator _converter;

    public ReceiveHandler(
        TimeProvider timeProvider,
        IFusionCacheProvider cacheProvider,
        ITransactionRepository repository,
        ITimeZoneApplicator converter)
    {
        _timeProvider = timeProvider;
        _cache = cacheProvider.GetCache(ApplicationConstants.TransactionCacheName);
        _repository = repository;
        _converter = converter;
    }

    public async Task<ErrorOr<ReceiveCommandResult>> Handle(
        ReceiveCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check cache
        var mayBeEntry = await _cache.TryGetAsync<Transaction>(
            key: request.ProvidedId.ToString("N"),
            token: cancellationToken);

        if (mayBeEntry.HasValue)
        {
            return new ReceiveCommandResult(
                Created: _converter.ToZonedDatetime(mayBeEntry.Value.Created));
        }

        // 2. Create
        var errorOrTransaction = Transaction.Create(
            providedId: request.ProvidedId,
            providedDate: request.ProvidedDate.ToInstant(),
            providedAmount: request.ProvidedAmount,
            now: _timeProvider.GetCurrentInstant());

        if (errorOrTransaction.IsError)
        {
            return errorOrTransaction.Errors;
        }

        // 3. Try save
        var status = await _repository.CreateAndCommit(
            transaction: errorOrTransaction.Value,
            ct: cancellationToken);

        if (status == ITransactionRepository.CreateAndCommitStatus.TooManyTransactionsError)
        {
            return ApplicationErrors.TooManyTransactions;
        }

        if (status == ITransactionRepository.CreateAndCommitStatus.DuplicateError)
        {
            var dublicate = await _repository.GetByProvidedId(
                providedId: request.ProvidedId,
                ct: cancellationToken);

            if (dublicate is null)
            {
                return ApplicationErrors.TransactionAlreadyProcessed;
            }

            return new ReceiveCommandResult(
                Created: _converter.ToZonedDatetime(dublicate.Created));
        }

        // 4. Cache update
        await _cache.SetAsync(
            key: request.ProvidedId.ToString("N"),
            value: errorOrTransaction.Value,
            token: cancellationToken);

        return new ReceiveCommandResult(
            Created: _converter.ToZonedDatetime(
                errorOrTransaction.Value.Created));
    }
}
