using HttpSampleProject.Shared.ApplicationLayer.Requests;
using HttpSampleProject.Shared.DomainLayer.Data;
using RequestResponseFramework.Server;
using RequestResponseFramework.Shared;

namespace HttpSampleProject.Server.ApplicationLayer.RequestHandlers
{
    public class ListWeatherForecastHandler : QueryHandler<ListWeatherForecast, IList<WeatherForecast>>
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        public override Task<Response<IList<WeatherForecast>>> HandleAsync(ListWeatherForecast query)
        {
            return CreateOkTask(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToList());
        }
    }
}
