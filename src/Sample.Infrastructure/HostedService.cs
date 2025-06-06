using Microsoft.Extensions.Hosting;

namespace Sample.Infrastructure;

internal sealed class HostedService : IHostedService
{
    public HostedService()
    {
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
