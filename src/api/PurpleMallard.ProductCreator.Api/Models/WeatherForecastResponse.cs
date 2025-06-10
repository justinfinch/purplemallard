namespace PurpleMallard.ProductCreator.Api.Models;

/// <summary>
/// Weather forecast data
/// </summary>
public class WeatherForecastResponse
{
    /// <summary>
    /// The date of the forecast
    /// </summary>
    public DateOnly Date { get; set; }
    
    /// <summary>
    /// The temperature in Celsius
    /// </summary>
    public int TemperatureC { get; set; }
    
    /// <summary>
    /// A textual summary of the weather
    /// </summary>
    public string? Summary { get; set; }
    
    /// <summary>
    /// The temperature in Fahrenheit, calculated from Celsius
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
