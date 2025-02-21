using Fhi.ClientCredentialsKeypairs;
using WorkerService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var clientCredentialsConfiguration = builder.Services.AddClientCredentialsKeypairs(builder.Configuration);
builder.Services.AddHttpClient("Weather", c =>
{
    c.BaseAddress = new Uri("https://localhost:7084");
})
.AddDefaultAuthHandler(new Api()
{
    Name = "API",
    Url = "https://localhost:7084"
});


var host = builder.Build();
host.Run();
