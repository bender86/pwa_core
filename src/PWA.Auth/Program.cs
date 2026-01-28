using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;  // ? Important
using Blazored.LocalStorage;
using PWA.Auth;
using PWA.Auth.Services;
using PWA.Auth.Handlers;
using System.Text.Json;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app"); 
builder.RootComponents.Add<HeadOutlet>("head::after");
// Load configuration files based on environment
var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

// Load appsettings.Development.json if in Development
if (builder.HostEnvironment.IsDevelopment())
{
    try
    {
        using var devResponse = await http.GetAsync("appsettings.Development.json");
        if (devResponse.IsSuccessStatusCode)
        {
            using var devStream = await devResponse.Content.ReadAsStreamAsync();
            builder.Configuration.AddJsonStream(devStream);
            Console.WriteLine("? Development settings loaded");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"?? Could not load Development settings: {ex.Message}");
    }
}

// Configuration de l'API
var apiBaseUrl = builder.Configuration["ApiSettings:MaesAuthApiUrl"] ?? "https://maeshome.ddns.net/api/v1/";
var worldBetApiUrl = builder.Configuration["ApiSettings:WorldBetApiUrl"] ?? "https://maeshome.ddns.net/apiworldbet/api/v1/";

// ? Configurer les options JSON globalement pour Blazor
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNameCaseInsensitive = true;
});
// Services d'authentification
builder.Services.AddBlazoredLocalStorage(config =>
{
    // ? Configurer Blazored.LocalStorage pour ignorer la casse aussi
    config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// ? Handler enregistr� comme Scoped (important pour ILocalStorageService)
builder.Services.AddScoped<CustomAuthorizationHandler>();

// ? HttpClient nomm� "API" avec le handler
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<CustomAuthorizationHandler>();
// HttpClient pour WorldBet API
builder.Services.AddHttpClient("WorldBetAPI", client =>
{
    client.BaseAddress = new Uri(worldBetApiUrl);
})
.AddHttpMessageHandler<CustomAuthorizationHandler>();

// ? HttpClient par d�faut qui utilise le client "API" configur�
builder.Services.AddScoped(sp => 
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

builder.Services.AddAuthorizationCore();

// Services personnalis�s
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMfaService, MfaService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IApplicationConfigService, ApplicationConfigService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
//worldbet
builder.Services.AddScoped<WorldBetApiClient>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IPronosticService, PronosticService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();

builder.Services.AddMudServices();

await builder.Build().RunAsync();