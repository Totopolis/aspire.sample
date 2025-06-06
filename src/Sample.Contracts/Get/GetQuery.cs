using ErrorOr;
using MediatR;

namespace Sample.Contracts.Get;

public sealed record GetQuery(
    Guid ProvidedId) : IRequest<ErrorOr<GetQueryResult>>;
