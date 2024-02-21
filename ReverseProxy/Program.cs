using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;


var builder = WebApplication.CreateBuilder(args);

var webApiHost = builder.Configuration.GetValue<string>("Endpoint:WebAPI");

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
    new RouteConfig
    {
        RouteId = "Route3",
        ClusterId = "sse",
        Match = new RouteMatch { Path = "/sse/{*any}" }
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
    new ClusterConfig
    {
        ClusterId = "sse",
        // HttpRequest = new ForwarderRequestConfig() { VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher },
        Destinations = new Dictionary<string, DestinationConfig>
        {
            { "destination3", new DestinationConfig { Address =  "http://webapi" } },
        }
    },
];


builder.AddServiceDefaults();

builder.Services.AddReverseProxy()
    .LoadFromMemory(routes, clusters)
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapReverseProxy();

app.Run();

