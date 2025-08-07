using HttpSampleProject.Shared.DomainLayer.Data;
using RequestResponseFramework.Shared.Requests;

namespace HttpSampleProject.Shared.ApplicationLayer.Requests
{

    public record ListWeatherForecast() : QueryBase<ListWeatherForecast, IList<WeatherForecast>>;


}
