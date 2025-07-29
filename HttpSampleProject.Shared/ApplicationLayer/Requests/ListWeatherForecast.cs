using HttpSampleProject.Shared.DomainLayer.Data;
using RequestResponseFramework.Requests;

namespace HttpSampleProject.Shared.ApplicationLayer.Requests
{

    public record ListWeatherForecast() : QueryBase<ListWeatherForecast, IList<WeatherForecast>>;


}
