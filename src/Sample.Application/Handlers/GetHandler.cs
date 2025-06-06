using ErrorOr;
using MediatR;
using Sample.Application.Abstractions;
using Sample.Application.Diagnostics;
using Sample.Contracts.Get;
using Sample.Domain.Transactions;
using ZiggyCreatures.Caching.Fusion;

namespace Sample.Application.Handlers;

public sealed class GetHandler :
    IRequestHandler<GetQuery, ErrorOr<GetQueryResult>>
{
    private readonly IFusionCache _cache;
    private readonly ITransactionRepository _repository;
    private readonly ITimeZoneApplicator _converter;

    public GetHandler(
        IFusionCacheProvider cacheProvider,
        ITransactionRepository repository,
        ITimeZoneApplicator converter)
    {
        _cache = cacheProvider.GetCache(ApplicationConstants.TransactionCacheName);
        _repository = repository;
        _converter = converter;
    }

    public async Task<ErrorOr<GetQueryResult>> Handle(
        GetQuery request,
        CancellationToken cancellationToken)
    {
        // Check cache
        var mayBeEntry = await _cache.TryGetAsync<Transaction>(
            key: request.ProvidedId.ToString("N"),
            token: cancellationToken);

        if (mayBeEntry.HasValue)
        {
            return new GetQueryResult(
                ProvidedId: mayBeEntry.Value.ProvidedId,
                ProvidedDate: _converter.ToZonedDatetime(mayBeEntry.Value.ProvidedDate),
                ProvidedAmount: mayBeEntry.Value.ProvidedAmount.Amount);
        }

        // Check db
        var entity = await _repository.GetByProvidedId(
            providedId: request.ProvidedId,
            ct: cancellationToken);

        if (entity is null)
        {
            return ApplicationErrors.TransactionNotFound;
        }

        return new GetQueryResult(
            ProvidedId: entity.ProvidedId,
            ProvidedDate: _converter.ToZonedDatetime(entity.ProvidedDate),
            ProvidedAmount: entity.ProvidedAmount.Amount);
    }
}
