using Microsoft.AspNetCore.SpaServices.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

// Add SPA static files
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "../purple-mallard-spa/build";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("AllowReactDev");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSpaStaticFiles();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// API endpoints
app.MapGet("/api/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Configure SPA
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "../purple-mallard-spa";

    if (app.Environment.IsDevelopment())
    {
        // In development, proxy requests to the React dev server
        spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
    }
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
