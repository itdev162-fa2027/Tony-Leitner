using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly DataContext _context;

    public WeatherForecastController(DataContext context)
    {
        _context = context;
    }

    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            Summaries[Random.Shared.Next(Summaries.Length)]
        ))
        .ToArray();
    }

    [HttpPost]
    public void Post()
    {
        var weatherForecast = new Domain.WeatherForecast();
        _context.WeatherForecasts.Add(weatherForecast);
        _context.SaveChanges();
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}