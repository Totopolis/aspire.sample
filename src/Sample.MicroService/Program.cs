using Sample.MicroService;

var builder = WebApplication.CreateBuilder(args);

builder.PreBuild();

var app = builder.Build();

await app.PostBuild();

await app.RunAsync();
