namespace Sample.Contracts.Get;

public sealed record GetQueryResult(
    Guid ProvidedId,
    DateTime ProvidedDate,
    decimal ProvidedAmount);
