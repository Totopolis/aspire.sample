using ErrorOr;
using MediatR;
using Sample.Contracts.Receive;

namespace Sample.Contracts.ReceiveTransaction;

public sealed record ReceiveCommand(
    Guid ProvidedId,
    DateTime ProvidedDate,
    decimal ProvidedAmount) : IRequest<ErrorOr<ReceiveCommandResult>>;
