using FastEndpoints;
using PurpleMallard.ProductCreator.Api.Models;

namespace PurpleMallard.ProductCreator.Api.Endpoints;

public class WeatherForecastEndpoint : EndpointWithoutRequest<List<WeatherForecastResponse>>
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public override void Configure()
    {
        Get("/weatherforecast");
        // Require authorization using our default policy
        Policies("Default");
        Description(d => d
            .WithName("GetWeatherForecast")
            .WithTags("Weather")
            .Produces<List<WeatherForecastResponse>>(200, "application/json")
        );
        Summary(s => {
            s.Summary = "Get weather forecast data";
            s.Description = "Retrieves a 5-day weather forecast with random temperatures";
            s.ResponseExamples[200] = new List<WeatherForecastResponse> 
            {
                new WeatherForecastResponse 
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                    TemperatureC = 25,
                    Summary = "Warm"
                }
            };
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecastResponse
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToList();

        await SendAsync(forecast, cancellation: ct);
    }
}
