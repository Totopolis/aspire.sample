using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Sample.Producer;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: (retryAttempt, context) =>
        {
            // Exp delay: 2, 4, 8
            var delay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
            var jitter = TimeSpan.FromSeconds(new Random().NextDouble() * 1.5);
            return delay + jitter;
        },
        onRetry: (_, _, _, _) => { });

var baseUrl1 = builder.Configuration.GetValue<string>("services:api-1:api-1-endpoint:0");
builder.Services
    .AddHttpClient("client-api-1", (client) =>
    {
        client.BaseAddress = new Uri(baseUrl1!);
    })
    .AddPolicyHandler(retryPolicy);

var baseUrl2 = builder.Configuration.GetValue<string>("services:api-2:api-2-endpoint:0");
builder.Services
    .AddHttpClient("client-api-2", (client) =>
    {
        client.BaseAddress = new Uri(baseUrl2!);
    })
    .AddPolicyHandler(retryPolicy);

builder.Services.AddHostedService<ProducerHostedService>();

using var host = builder.Build();

await host.RunAsync();

var xx = 1;