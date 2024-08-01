using Microsoft.AspNetCore.Mvc;
using WebApplication5.Attributes;
using WebApplication5.DependencyInjection;

namespace WebApplication5.Controllers
{
    
    public class WeatherForecastController : BaseController
    {
        private readonly IClassA _classA;
        private readonly IClassB _classB;

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IClassA classA, IClassB classB)
        {
            _logger = logger;
            _classA = classA;
            _classB = classB;   
        }

        [HttpGet(Name = "GetWeatherForecast")]
        [ExcusionAttribute("Test1")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}