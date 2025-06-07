var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var db = postgres.AddDatabase("db");

var redis = builder.AddRedis("redis");

var migrator = builder.AddProject<Projects.Sample_Migrator>("migrator")
    .WithReference(db)
    .WaitFor(db);

// TODO: use .WithReplicas(2)
var apiInstance1 = AddApiInstance(1, 1831);
var apiInstance2 = AddApiInstance(2, 1832);

// Producer generate transactions throught api
_ = builder
    .AddProject<Projects.Sample_Producer>("producer-1")
    .WithReference(apiInstance1)
    .WaitFor(apiInstance1)
    .WithReference(apiInstance2)
    .WaitFor(apiInstance2)
    .WithExplicitStart();

// Consumer consume from db directly
_ = builder
    .AddProject<Projects.Sample_Consumer>("consumer-1")
    .WithReference(migrator)
    .WaitFor(migrator)
    .WithExplicitStart();

await builder
    .Build()
    .RunAsync();

IResourceBuilder<ProjectResource> AddApiInstance(
    int instanceNumber,
    int instancePort)
{
    var instance = builder
        .AddProject<Projects.Sample_MicroService>($"api-{instanceNumber}")
        .WithHttpEndpoint(
            port: instancePort,
            name: $"api-{instanceNumber}-endpoint")
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
        .WithReference(db)
        .WaitFor(db)
        .WaitForCompletion(migrator)
        .WithReference(redis)
        .WaitFor(redis);

    return instance;
}
