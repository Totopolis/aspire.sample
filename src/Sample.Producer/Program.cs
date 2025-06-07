using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

using var host = builder.Build();

// TODO: http storm
await host.StartAsync();
