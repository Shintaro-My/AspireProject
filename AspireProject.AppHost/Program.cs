
var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.WebApi>("webapi");

var frontend = builder.AddNpmApp(name: "frontend", workingDirectory: "../frontend", scriptName: "dev")
     .WithServiceBinding(hostPort: 3000, scheme: "http", env: "PORT")
     .WithReference(api);

builder.AddProject<Projects.ReverseProxy>("reverseproxy")
    .WithReference(frontend)
    .WithReference(api);

builder.Build().Run();
