
using System.Net.Http.Json;
using Microsoft.Extensions.Hosting;
using Yarp.ReverseProxy.ServiceDiscovery;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Add CORS policy for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy => 
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173") // Allow both localhost and IP address
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Allow credentials (cookies, auth headers)
    });
});

// Add YARP Reverse Proxy with Aspire service discovery integration
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver(); // Add Aspire service discovery

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowReactDev");
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

// Map the reverse proxy routes
app.MapReverseProxy();

// Configure SPA - with SpaProxy, it will automatically handle this
app.MapFallbackToFile("index.html");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
