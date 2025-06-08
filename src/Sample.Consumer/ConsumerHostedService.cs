using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Application.Abstractions;
using Sample.Infrastructure.Database;

namespace Sample.Producer;

internal sealed class ConsumerHostedService : IHostedService
{
    // TODO: use options or aspire params
    public const int BatchSize = 20;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ConsumerHostedService> _logger;
    private readonly CancellationTokenSource _needStop;
    private volatile int _counter = 0;

    public ConsumerHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<ConsumerHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _needStop = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(async () => await ProcessingTask(_needStop.Token));

        return Task.CompletedTask;
    }

    // TODO: Dont work
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _needStop.Dispose();

        _logger.LogInformation("Processed {0} transactions", _counter);

        return Task.CompletedTask;
    }

    private async Task ProcessingTask(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var batch = await repo.ExtractBatch(BatchSize, ct);

                Interlocked.Add(ref _counter, batch.Count);

                if (_counter > 100)
                {
                    _logger.LogInformation("Processed ~100 transactions");
                    _counter = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error by extracting batch of trans");
            }

            // Unsafe Processing...
            await Task.Delay(50);
        }
    }
}
