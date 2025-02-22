using System.Text.Json;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly IWeatherForcastApi _weatherForcastApi;

        public Worker(ILogger<Worker> logger, IHttpClientFactory factory, IWeatherForcastApi weatherForcastApi)
        {
            _logger = logger;
            _factory = factory;
            _weatherForcastApi = weatherForcastApi;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = _factory.CreateClient("Weather");
            var weatherResponseHttpClient = await client.GetAsync("/WeatherForecast");
            var contentHttpClient = await weatherResponseHttpClient.Content.ReadAsStringAsync();
            _logger.LogInformation("HttpClient response: " + contentHttpClient);

            var weatherResponseRefit = await _weatherForcastApi.GetWeatherForcast();
            _logger.LogInformation("Refit response:" + JsonSerializer.Serialize(weatherResponseRefit.Content));

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
