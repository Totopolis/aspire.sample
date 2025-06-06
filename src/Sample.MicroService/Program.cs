using Sample.MicroService;

// Time to start
// using var cts = new CancellationTokenSource(millisecondsDelay: 30_000);

var builder = WebApplication.CreateBuilder(args);

builder.PreBuild();

var app = builder.Build();

await app.PostBuild(CancellationToken.None);

await app.RunAsync();
