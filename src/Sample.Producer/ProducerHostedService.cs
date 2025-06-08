using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Threading.Channels;

namespace Sample.Producer;

internal sealed class ProducerHostedService : IHostedService
{
    private readonly ILogger<ProducerHostedService> _logger;

    private readonly HttpClient _client1;
    private readonly HttpClient _client2;
    private readonly Channel<Transaction> _channel;

    private readonly CancellationTokenSource _needStop;

    private volatile int _counter = 0;

    public ProducerHostedService(
        ILogger<ProducerHostedService> logger,
        IHttpClientFactory factory)
    {
        _logger = logger;

        _client1 = factory.CreateClient("client-api-1");
        _client2 = factory.CreateClient("client-api-2");

        _channel = Channel.CreateBounded<Transaction>(
            new BoundedChannelOptions(2 * 100 + 100)
            {
                SingleWriter = true,
                SingleReader = false,
                FullMode = BoundedChannelFullMode.Wait,
            });

        _needStop = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(async () => await GeneratorTask(_needStop.Token));
        _ = Task.Run(async () => await SenderTask(_client1, _needStop.Token));
        _ = Task.Run(async () => await SenderTask(_client2, _needStop.Token));

        return Task.CompletedTask;
    }

    // TODO: Dont work
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _needStop.Dispose();

        _client1.Dispose();
        _client2.Dispose();

        _logger.LogInformation("Sended {0} transactions", _counter);

        return Task.CompletedTask;
    }

    private async Task GeneratorTask(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var amount = Random.Shared.NextDouble() * 10_000;
            var trans = new Transaction
            {
                Id = Guid.CreateVersion7(),
                TransactionDate = DateTime.UtcNow,
                Amount = (decimal)amount
            };

            // Postgres NodaTime.Instant is 1us granularity
            await Task.Delay(1);

            await _channel.Writer.WriteAsync(
                item: trans,
                cancellationToken: ct);

            if (_counter > 100)
            {
                _logger.LogInformation($"Sended ~100 transactions");
                _counter = 0;
            }
        }
    }

    private async Task SenderTask(HttpClient client, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await foreach (var item in _channel.Reader.ReadAllAsync(ct))
            {
                try
                {
                    _ = await client.PostAsJsonAsync(
                        requestUri: "/api/v1/Transaction",
                        value: item,
                        cancellationToken: ct);

                    Interlocked.Increment(ref _counter);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Source transaction is LOST");
                    // Guard delay on Polly Policy
                }
            }
        }
    }
}
