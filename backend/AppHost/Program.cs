var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.api>("apiservice", "../src/api")
    .WithReference(cache);

builder.AddNpmApp("frontend", "../../frontend", "dev")
    .WithHttpEndpoint(port: 8888, isProxied: true)
    .WithReference(apiService);

builder.Build().Run();