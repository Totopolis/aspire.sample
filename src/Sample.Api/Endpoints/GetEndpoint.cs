using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using Sample.Contracts.Get;
using static Sample.Api.Endpoints.GetEndpoint;

namespace Sample.Api.Endpoints;

public sealed class GetEndpoint :
    EndpointWithoutRequest<GetResponse>
{
    private readonly ILogger<GetEndpoint> _logger;
    private readonly IMediator _mediator;

    public GetEndpoint(
        ILogger<GetEndpoint> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/v1/Transaction");
        AllowAnonymous();

        _logger.LogDebug("GetEndpoint configurated");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Query<Guid>("id");
        var query = new GetQuery(ProvidedId: id);

        var errorOrResponse = await _mediator.Send(query, ct);
        var response = errorOrResponse.ValueOrThrow();

        await SendAsync(
            response: new GetResponse(
                Id: response.ProvidedId,
                TransactionDate: response.ProvidedDate,
                Amount: response.ProvidedAmount),
            cancellation: ct);
    }

    public record GetResponse(
        Guid Id,
        DateTime TransactionDate,
        decimal Amount);
}
