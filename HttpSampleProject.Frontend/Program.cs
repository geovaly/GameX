using HttpSampleProject.Shared.ApplicationLayer.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RequestResponseFramework;
using RequestResponseFramework.Client;
using RequestResponseFramework.Client.Http;

var clientSettings = new HttpRequestClientSettings(new Uri("http://localhost:5222"));

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddRequestResponseFramework(cfg => cfg.RegisterContractsFromAssemblyContaining<ListWeatherForecast>());
        services.AddHttpRequestResponseClient(clientSettings);
    })
    .ConfigureLogging(logging =>
    {
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Information);
    })
    .Build();

var requestExecutor = host.Services.GetRequiredService<IRequestExecutor>();
var weatherForecasts = await requestExecutor.ExecuteAsync(new ListWeatherForecast());

foreach (var x in weatherForecasts)
{
    Console.WriteLine(x);
}

host.Dispose();