using Yarp.ReverseProxy.Configuration;

RouteConfig[] routes = [
    new RouteConfig
    {
        RouteId = "Route1",
        ClusterId = "default",
        Match = new RouteMatch { Path = "{**catch-all}" }
    },
    new RouteConfig
    {
        RouteId = "Route2",
        ClusterId = "api",
        Match = new RouteMatch { Path = "/api/{*any}" }
    },
];

ClusterConfig[] clusters = [
    new ClusterConfig
    {
        ClusterId = "default",
        Destinations = new Dictionary<string, DestinationConfig>
        {
            { "destination1", new DestinationConfig { Address = "http://frontend" } },
        }
    },
    new ClusterConfig
    {
        ClusterId = "api",
        Destinations = new Dictionary<string, DestinationConfig>
        {
            { "destination2", new DestinationConfig { Address =  "http://webapi" } },
        }
    },
];

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddReverseProxy()
    .LoadFromMemory(routes, clusters)
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapReverseProxy();

app.Run();

