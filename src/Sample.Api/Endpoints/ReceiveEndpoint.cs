using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using Sample.Contracts.ReceiveTransaction;
using static Sample.Api.Endpoints.ReceiveEndpoint;

namespace Sample.Api.Endpoints;

public sealed class ReceiveEndpoint :
    Endpoint<ReceiveRequest, ReceiveResponse>
{
    private readonly ILogger<ReceiveEndpoint> _logger;
    private readonly IMediator _mediator;

    public ReceiveEndpoint(
        ILogger<ReceiveEndpoint> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/v1/Transaction");
        AllowAnonymous();

        _logger.LogDebug("ReceiveEndpoint configurated");
    }

    public override async Task HandleAsync(
        ReceiveRequest request,
        CancellationToken ct)
    {
        // a long long time ago, austrlipithecus used automapper
        var command = new ReceiveCommand(
            ProvidedId: request.Id,
            ProvidedDate: request.TransactionDate,
            ProvidedAmount: request.Amount);

        var errorOrResponse = await _mediator.Send(command, ct);
        var response = errorOrResponse.ValueOrThrow();

        await SendAsync(
            response: new ReceiveResponse(InsertDateTime: response.Created),
            cancellation: ct);
    }

    public record ReceiveRequest(
        Guid Id,
        DateTime TransactionDate,
        decimal Amount);

    public record ReceiveResponse(
        DateTime InsertDateTime);
}

/*
{
  "id": "7632C024-9C42-4F37-9DE0-FF1CC8F92218",
  "transactionDate": "2017-07-21T17:32:28Z",
  "amount": 100500
} 
*/