using Fhi.ClientCredentials.Refit;
using Fhi.ClientCredentialsKeypairs;
using Refit;
using WorkerService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var clientCredentialsConfiguration = builder.Services.AddClientCredentialsKeypairs(builder.Configuration);

//Using HttpClient
builder.Services.AddHttpClient("Weather", c =>
{
    c.BaseAddress = new Uri("https://localhost:7084");
})
.AddDefaultAuthHandler(new Api()
{
    Name = "API",
    Url = "https://localhost:7084"
});

//Using Refit
builder.Services.AddClientCredentialsRefitBuilder(clientCredentialsConfiguration)
    .AddRefitClient<IWeatherForcastApi>();


var host = builder.Build();
host.Run();

public record WeatherForcastModel(DateOnly Date, int TemperatureC, int TemperatureF, string? Summary);
public interface IWeatherForcastApi
{
    [Get("/weatherforecast")]
    Task<ApiResponse<IEnumerable<WeatherForcastModel>>> GetWeatherForcast();
}
