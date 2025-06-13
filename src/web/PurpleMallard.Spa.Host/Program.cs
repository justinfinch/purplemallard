
using System.Net.Http.Json;
using Microsoft.Extensions.Hosting;
using Yarp.ReverseProxy.ServiceDiscovery;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add YARP Reverse Proxy with Aspire service discovery integration
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver(); // Add Aspire service discovery

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

// Map the reverse proxy routes
app.MapReverseProxy();

// Configure SPA - with SpaProxy, it will automatically handle this
app.MapFallbackToFile("index.html");

app.Run();
