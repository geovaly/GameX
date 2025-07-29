using HttpSampleProject.Shared.ApplicationLayer.Requests;
using HttpSampleProject.Shared.DomainLayer.Data;
using Microsoft.AspNetCore.Mvc;
using RequestResponseFramework;
using RequestResponseFramework.Json;
using RequestResponseFramework.Server;
using RequestResponseFramework.Server.Http;

namespace HttpSampleProject.Backend.ApiLayer
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IServerRequestExecutor serverRequestExecutor,
        IJsonSerializerOptionsProvider jsonSerializerOptionsProvider)
        : RequestControllerBase(logger, serverRequestExecutor, jsonSerializerOptionsProvider)
    {


        [HttpGet(Name = "GetWeatherForecast")]
        [ProducesResponseType(typeof(Ok<IList<WeatherForecast>>), StatusCodes.Status200OK)]
        public Task<ContentResult> Get() => Execute(new ListWeatherForecast());


    }
}
