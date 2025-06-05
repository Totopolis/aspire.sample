var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("db");

var redis = builder.AddRedis("redis");

var apiInstance1 = AddApiInstance(1, 1831);
var apiInstance2 = AddApiInstance(2, 1832);

// Transactions source
_ = builder
    .AddProject<Projects.Sample_MicroService>("tester-1")
    .WithHttpEndpoint(port: 1833)
    .WithReference(apiInstance1)
    .WaitFor(apiInstance1)
    .WithReference(apiInstance2)
    .WaitFor(apiInstance2);

await builder
    .Build()
    .RunAsync();

IResourceBuilder<ProjectResource> AddApiInstance(int instanceNumber, int instancePort)
{
    var instance = builder
        .AddProject<Projects.Sample_MicroService>($"api-{instanceNumber}")
        .WithHttpEndpoint(port: instancePort)
        .WithUrls(context =>
        {
            var oldUrlAnnotation = context.Urls.Single();
            context.Urls.Clear();

            context.Urls.Add(new ResourceUrlAnnotation
            {
                Url = "/",
                DisplayText = "Status",
                Endpoint = oldUrlAnnotation.Endpoint
            });

            context.Urls.Add(new ResourceUrlAnnotation
            {
                Url = "/scalar",
                DisplayText = "Scalar UI",
                Endpoint = oldUrlAnnotation.Endpoint
            });
        })
        .WithReference(postgresdb)
        .WaitFor(postgresdb)
        .WithReference(redis)
        .WaitFor(redis);

    return instance;
}
