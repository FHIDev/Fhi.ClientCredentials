# Whats new in 2.0

## Breaking changes

If you use HttpClientFactory, use `AddDefaultAuthHandler()` instead of adding a `HttpAuthHandler`. Refactor this:

```csharp
services.AddHttpClient(api.Name, c =>
{
    c.BaseAddress = clientCredentialsConfiguration.UriToApiByName(api.Name);
})
.AddHttpMessageHandler<HttpAuthHandler>();
```
to this:

```csharp

var apiConfiguration = clientCredentialsConfiguration.GetApi(api.Name);
services.AddHttpClient(api.Name, c =>
{
    c.BaseAddress = new Uri(apiConfiguration.Url);
})
.AddDefaultAuthHandler(apiConfiguration);
```

## New features

Scopes and DPoP-configuration can now be set per API:

```json
"ClientCredentialsConfiguration": {
  "Apis": [
    {
      "Name": "IGrunndataClient",
      "Url": "https://localhost:5001",
      "Scope": "fhi:grunndata.personoppslagapi/sysvak", // <-- new! (you might have had this here before, but it was never used)
      "UseDpop": true // <-- new! (this is false by default)
    }
  ],
```

If no scope is set per API, it will default back to the scopes list set in `ClientCredentialsConfiguration`. Make sure your API calls get the correct scopes after upgrading.

# Client Credentials Usage

## Configuration file section

1. Add the following configuration section to your appsettings.json files, and populate it appropriately.

```json
  "ClientCredentialsConfiguration": {
    "clientName": "",
    "authority": "https://helseid-sts.test.nhn.no", //sample of using HelseID test
    "clientId": "",
    "scopes": [ ],
    "privateJwk": "",
    "Apis": [
      {
        "Name": "", // Tip:  Use nameof(YourService)
        "Url": "",
        "Scope": ""
      }
    ],
    "refreshTokenAfterMinutes":  8  // Set approx 20% less than lifetime of access token (defaults to 8 minutes if omitted).
  }
```

## Client Credentials using Keypairs

1. Add package 'Fhi.ClientCredentialsKeypairs' to your project

2. In your `Program.cs` file, or if older `Startup.cs`, add the following code section (for the outgoing interfaces):

```cs
    var clientCredentialsConfiguration = services.AddClientCredentialsKeypairs(Configuration);
    var apiConfiguration = clientCredentialsConfiguration.GetApi(nameof(YourService));
    services.AddHttpClient(nameof(YourService), c =>
    {
       c.Timeout = new TimeSpan(0, 0, 0, 10);
       c.BaseAddress = new Uri(apiConfiguration.Url);
    })
    .AddDefaultAuthHandler(apiConfiguration)
    .AddTypedClient(c => RestService.For<IExternalApi>(c, new RefitSettings
    {
       ContentSerializer = new SystemTextJsonContentSerializer(services.DefaultJsonSerializationOptions())
    }));
```
replacing `YourService` with the service you have done for accessing the external api, and replace `IExternalApi` with the Refit interface for whatever external api you want to access.

For usages of Refit that uses an interface (in this example `IMyService` is the interface that Refit will implement), the code would look something like this:

```cs
services
    .AddRefitClient<IMyService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(apiConfiguration.Url);
    })
    .AddDefaultAuthHandler(apiConfiguration);
```

The `Configuration` property is the injected IConfiguration property from the Startup.cs file.

If you don't use Refit, you can just skip the last part, and get the named client from the injected HttpFactory in your service. It will still have the authenticationhandler, so you don't need to do anything more there to get the bearer token. It will be added automatically.




## Client Credentials using Client Secrets

If you want to disable the authorization for some reason, you can add another property named `Enable` to the ClientCredentialsConfiguration, it is default true.

2. Add package `Fhi.ClientCredentialsUsingSecrets` to your project
3. In your `Program.cs` file, create an instance of the `ClientCredentialsSetup` class using an `IConfiguration` parameter.
4. Using the created instance call the method `ConfigureServices`.

## Calling endpoints that does not required authentication

In some cases we might wish to call an API before we are authenticated (health endpoints, kodeverk, etc..).

To make the HttpAuthHandler not add authentication headers to a single request you can add an Option
to the request with the key name "Anonymous":

```
var request = new HttpRequestMessage();
request.Options.TryAdd("Anonymous", "");
```

or in Refit:

```
[Get("/info")]
Task<string> GetInfo([Property("Anonymous")] string anonymous = "");
```
