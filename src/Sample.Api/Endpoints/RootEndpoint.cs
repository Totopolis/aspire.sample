using FastEndpoints;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using static Sample.Api.Endpoints.RootEndpoint;

namespace Sample.Api.Endpoints;

public sealed class RootEndpoint : EndpointWithoutRequest<RootResponse>
{
    private readonly ILogger<RootEndpoint> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    public RootEndpoint(
        ILogger<RootEndpoint> logger,
        IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        _logger.LogWarning("Root endpoint handler executing");

        var assembly = Assembly.GetEntryAssembly()!;
        string fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion!;

        await SendAsync(new(
            Promt: "Hello funs!",
            Application: _hostEnvironment.ApplicationName,
            Environment: _hostEnvironment.EnvironmentName,
            Version: fileVersion));
    }

    public record RootResponse(
        string Promt,
        string Application,
        string Environment,
        string Version);
}
